// --------------------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 Pedro Santos @pedromsantos
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.
// --------------------------------------------------------------------------------------------------------------------

namespace AssemblerTests
{
    using System.Collections.Generic;
    using System.IO;

    using Assembler;

    using Lexer;
    using Lexer.Tokens;

    using NUnit.Framework;

    [TestFixture]
    public class AssemblesIntegrationTests
    {
        private IList<TokenBase> matchers;

        [SetUp]
        public void Setup()
        {
            this.matchers = new List<TokenBase>
                {
                    new WhiteSpaceToken(),
                    new CommentToken(),
                    new LabelToken(),
                    new HexToken(),
                    new DecimalToken(),
                    new PluslToken(),
                    new CommaToken(),
                    new OpenBracketToken(),
                    new CloseBracketToken(),
                    new InstructionToken(),
                    new RegisterToken(),
                    new StringToken(),
                    new LabelReferenceToken()
                };
        }

        [Test]
        public void AssembleStatmentsWhenCalledWithSetRegisterWithDecimalLiteralGenertesCorrectProgram()
        {
            const string Code = "SET I, 10";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser.Parser(lexer);

            var statments = parser.Parse();

            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            Assert.That(program.Count, Is.EqualTo(1));
            Assert.That(program[0], Is.EqualTo(0xA861));
        }

        [Test]
        public void AssembleStatmentsWhenCalledWithSetRegisterWithHexLiteralGenertesCorrectProgram()
        {
            const string Code = "SET A, 0x30";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser.Parser(lexer);

            var statments = parser.Parse();

            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            Assert.That(program.Count, Is.EqualTo(2));
            Assert.That(program[0], Is.EqualTo(0x7C01));
            Assert.That(program[1], Is.EqualTo(0x0030));
        }

        [Test]
        public void AssembleStatmentsWhenCalledWithSetAddressWithHexLiteralGenertesCorrectProgramm()
        {
            const string Code = "SET [0x1000], 0x20";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser.Parser(lexer);

            var statments = parser.Parse();

            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            Assert.That(program.Count, Is.EqualTo(3));
            Assert.That(program[0], Is.EqualTo(0x7DE1));
            Assert.That(program[1], Is.EqualTo(0x1000));
            Assert.That(program[2], Is.EqualTo(0x0020));
        }

        [Test]
        public void AssembleStatmentsWhenCalledWithhSetOffsetAddressWithAddresssGenertesCorrectProgram()
        {
            const string Code = "SET [0x2000+I], [A]";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser.Parser(lexer);

            var statments = parser.Parse();

            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            Assert.That(program.Count, Is.EqualTo(2));
            Assert.That(program[0], Is.EqualTo(0x2161));
            Assert.That(program[1], Is.EqualTo(0x2000));
        }

        [Test]
        public void AssembleStatmentsWhenCalledWithDatGenertesCorrectProgram()
        {
            const string Code = @"DAT 0x10, 0x20, 0x30, 0x40, 0x50
                                 SET I, 0";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser.Parser(lexer);

            var statments = parser.Parse();

            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            Assert.That(program.Count, Is.EqualTo(6));
            Assert.That(program[0], Is.EqualTo(0x10));
            Assert.That(program[1], Is.EqualTo(0x20));
            Assert.That(program[2], Is.EqualTo(0x30));
            Assert.That(program[3], Is.EqualTo(0x40));
            Assert.That(program[4], Is.EqualTo(0x50));
        }

        [Test]
        public void AssembleStatmentsWhenCalledWithNotchSampleGeneratesCorrectProgram()
        {
            const string Code = @"  ;Try some basic stuff
                                    SET A, 0x30              ; 7c01 0030
                                    SET [0x1000], 0x20       ; 7de1 1000 0020
                                    SUB A, [0x1000]          ; 7803 1000
                                    IFN A, 0x10              ; c00d
                                    SET PC, crash            ; 7dc1 001a [*]

                                    ; Do a loopy thing
                                    SET I, 10                ; a861
                                    SET A, 0x2000            ; 7c01 2000
                                    :loop         SET [0x2000+I], [A]      ; 2161 2000
                                    SUB I, 1                 ; 8463
                                    IFN I, 0                 ; 806d
                                    SET PC, loop             ; 7dc1 000d [*]

                                    ; Call a subroutine
                                    SET X, 0x4               ; 9031
                                    JSR testsub              ; 7c10 0018 [*]
                                    SET PC, crash            ; 7dc1 001a [*]

                                    :testsub      SHL X, 4   ; 9037
                                    SET PC, POP              ; 61c1

                                    ; Hang forever. X should now be 0x40 if everything went right.
                                    :crash        SET PC, crash            ; 7dc1 001a [*]

                                    ; [*]: Note that these can be one word shorter and one cycle faster by using the short form (0x00-0x1f) of literals,\n\
                                    ;      but my assembler doesn't support short form labels yet.";

            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser.Parser(lexer);

            parser.Parse();

            var statments = parser.Statments;

            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            Assert.That(program.Count, Is.EqualTo(28));

            var expectedInstruction = new[]
                {
                    0x7c01, 0x0030, 0x7de1, 0x1000, 0x0020, 0x7803, 0x1000, 0xc00d, 
                    0x7dc1, 0x001a, 0xa861, 0x7c01, 0x2000, 0x2161, 0x2000, 0x8463,
                    0x806d, 0x7dc1, 0x000d, 0x9031, 0x7c10, 0x0018, 0x7dc1, 0x001a, 
                    0x9037, 0x61c1, 0x7dc1, 0x001a
                };

            for (var i = 0; i < 28; i++)
            {
                Assert.That(program[i], Is.EqualTo(expectedInstruction[i]));
            }
        }

        [Test]
        public void AssembleStatmentsWhenCalledWithHelloWorldSampleGeneratesCorrectProgram()
        {
            const string Code = @"
; Assembler test for DCPU
; by Markus Persson

             set a, 0xbeef                        ; Assign 0xbeef to register a
             set [0x1000], a                      ; Assign memory at 0x1000 to value of register a
             ifn a, [0x1000]                      ; Compare value of register a to memory at 0x1000 ..
                 set PC, end                      ; .. and jump to end if they don't match

             set i, 0                             ; Init loop counter, for clarity
:nextchar    ife [data+i], 0                      ; If the character is 0 ..
                 set PC, end                      ; .. jump to the end
             set [0x8000+i], [data+i]             ; Video ram starts at 0x8000, copy char there
             add i, 1                             ; Increase loop counter
             set PC, nextchar                     ; Loop
  
:data        dat ""Hello world!"", 0              ; Zero terminated string

:end         SET A, 1                             ; Freeze the CPU forever";

            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser.Parser(lexer);

            parser.Parse();

            var statments = parser.Statments;

            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var expectedInstruction = new[]
                {
                    0x7c01, 0xbeef, 0x01e1, 0x1000, 0x780d, 0x1000, 0x7dc1, 0x0021, 
                    0x8061, 0x816c, 0x0013, 0x7dc1, 0x0021, 0x5961, 0x8000, 0x0013, 
                    0x8462, 0x7dc1, 0x0009, 0x0022, 0x0048, 0x0065, 0x006c, 0x006c, 
                    0x006f, 0x0077, 0x006f, 0x0072, 0x006c, 0x0064, 0x0021, 0x0022, 
                    0x0000, 0x8401
                };

            Assert.That(program.Count, Is.EqualTo(expectedInstruction.Length));
            
            for (var i = 0; i < 28; i++)
            {
                Assert.That(program[i], Is.EqualTo(expectedInstruction[i]));
            }
        }
    }
}
