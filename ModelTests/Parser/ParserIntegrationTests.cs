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

namespace Model.ParserTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Lexer;
    using Lexer.Tokens;

    using Model;
    using Model.Operands;
    using Model.Parser;

    using NUnit.Framework;

    [TestFixture]
    public class ParserIntegrationTests
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
        public void ParseWhenCalledWithSetRegisterWithDecimalLiteralGenertesCorrectStatments()
        {
            const string Code = "SET I, 10";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var indirectOperandFactory = new IndirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory, indirectOperandFactory);

            parser.Parse();

            var statment = parser.Statments.First();

            Assert.That(statment.Menemonic, Is.EqualTo("SET"));
            Assert.That(statment.Opcode, Is.EqualTo((int)BasicOpcode.OpSet));
            Assert.That(statment.OperandA is RegisterOperand);
            Assert.That(statment.OperandA.RegisterValue, Is.EqualTo((int)RegisterIdentifier.RegI));
            Assert.That(statment.OperandB is NextWordOperand);
            Assert.That(statment.OperandB.NextWord, Is.EqualTo(10));
        }

        [Test]
        public void ParseWhenCalledWithSetRegisterWithHexLiteralGenertesCorrectStatments()
        {
            const string Code = "SET X, 0x4";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var indirectOperandFactory = new IndirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory, indirectOperandFactory);

            parser.Parse();

            var statment = parser.Statments.First();

            Assert.That(statment.Menemonic, Is.EqualTo("SET"));
            Assert.That(statment.Opcode, Is.EqualTo((int)BasicOpcode.OpSet));
            Assert.That(statment.OperandA is RegisterOperand);
            Assert.That(statment.OperandA.RegisterValue, Is.EqualTo((int)RegisterIdentifier.RegX));
            Assert.That(statment.OperandB is NextWordOperand);
            Assert.That(statment.OperandB.NextWord, Is.EqualTo(4));
        }

        [Test]
        public void ParseWhenCalledWithSetMemoryAddressWithLiteralGenertesCorrectStatments()
        {
            const string Code = "SET [0x1000], 0x20";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var indirectOperandFactory = new IndirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory, indirectOperandFactory);

            parser.Parse();

            var statment = parser.Statments.First();

            Assert.That(statment.Menemonic, Is.EqualTo("SET"));
            Assert.That(statment.Opcode, Is.EqualTo((int)BasicOpcode.OpSet));
            Assert.That(statment.OperandA is IndirectNextWordOperand);
            Assert.That(statment.OperandA.NextWord, Is.EqualTo(4096));
            Assert.That(statment.OperandB is NextWordOperand);
            Assert.That(statment.OperandB.NextWord, Is.EqualTo(32));
        }

        [Test]
        public void ParseWhenCalledWithSetPcRegisterWithLabelRefGenertesCorrectStatments()
        {
            const string Code = "SET PC, crash";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var indirectOperandFactory = new IndirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory, indirectOperandFactory);

            parser.Parse();

            var statment = parser.Statments.First();

            Assert.That(statment.Menemonic, Is.EqualTo("SET"));
            Assert.That(statment.Opcode, Is.EqualTo((int)BasicOpcode.OpSet));
            Assert.That(statment.OperandA is ProgramCounterOperand);
            Assert.That(statment.OperandB is NextWordOperand);
            Assert.That(statment.OperandB.Label, Is.EqualTo("crash"));
            Assert.That(statment.OperandB.NextWord, Is.EqualTo(0));
        }

        [Test]
        public void ParseWhenCalledWithJsRandLabelRefGenertesCorrectStatments()
        {
            const string Code = "JSR testsub";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var indirectOperandFactory = new IndirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory, indirectOperandFactory);

            parser.Parse();

            var statment = parser.Statments.First();

            Assert.That(statment.Menemonic, Is.EqualTo("JSR"));
            Assert.That(statment.Opcode, Is.EqualTo(0));
            Assert.That(statment.OperandA is NextWordOperand);
            Assert.That(statment.OperandA.Label, Is.EqualTo("testsub"));
            Assert.That(statment.OperandA.NextWord, Is.EqualTo(0));
        }

        [Test]
        public void ParseWhenCalledWithInvalidInstructionThrows()
        {
            const string Code = "JSM testsub";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var indirectOperandFactory = new IndirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory, indirectOperandFactory);

            Assert.Throws<Exception>(() => parser.Parse());
        }

        [Test]
        public void ParseWhenCalledWithInvalidOperandThrowss()
        {
            const string Code = "JSM \"testsub\"";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var indirectOperandFactory = new IndirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory, indirectOperandFactory);

            Assert.Throws<Exception>(() => parser.Parse());
        }

        [Test]
        public void ParseWhenCalledWithUnclosedBracketThrows()
        {
            const string Code = "SET [0x1000, 0x20";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var indirectOperandFactory = new IndirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory, indirectOperandFactory);

            Assert.Throws<Exception>(() => parser.Parse());
        }

        [Test]
        public void ParseWhenCalledWithDatGenertesCorrectStatments()
        {
            const string Code = @"DAT 0x10, 0x20, 0x30, 0x40, 0x50
                                 SET I, 0";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var indirectOperandFactory = new IndirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory, indirectOperandFactory);

            parser.Parse();

            var statment = parser.Statments.First();
            Assert.That(statment.Menemonic, Is.EqualTo("DAT"));
            Assert.That(statment.Opcode, Is.EqualTo(-1));
            Assert.That(statment.OperandA, Is.Null);
            Assert.That(statment.OperandB, Is.Null);
            Assert.That(statment.Label, Is.Null);
            Assert.That(statment.Dat[0], Is.EqualTo(0x10));
            Assert.That(statment.Dat[1], Is.EqualTo(0x20));
            Assert.That(statment.Dat[2], Is.EqualTo(0x30));
            Assert.That(statment.Dat[3], Is.EqualTo(0x40));
            Assert.That(statment.Dat[4], Is.EqualTo(0x50));
        }

        [Test]
        public void ParseWhenCalledWithDatContainingAStringGenertesCorrectStatments()
        {
            const string Code = @"DAT 0x10, ""Hello"", 0x20, 0x30, 0x40, 0x50
                                 SET I, 0";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var indirectOperandFactory = new IndirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory, indirectOperandFactory);

            parser.Parse();

            var statment = parser.Statments.First();
            Assert.That(statment.Menemonic, Is.EqualTo("DAT"));
            Assert.That(statment.Opcode, Is.EqualTo(-1));
            Assert.That(statment.OperandA, Is.Null);
            Assert.That(statment.OperandB, Is.Null);
            Assert.That(statment.Label, Is.Null);
            Assert.That(statment.Dat[0], Is.EqualTo(0x10));
            Assert.That(statment.Dat[1], Is.EqualTo((ushort)'"'));
            Assert.That(statment.Dat[2], Is.EqualTo((ushort)'H'));
            Assert.That(statment.Dat[3], Is.EqualTo((ushort)'e'));
            Assert.That(statment.Dat[4], Is.EqualTo((ushort)'l'));
            Assert.That(statment.Dat[5], Is.EqualTo((ushort)'l'));
            Assert.That(statment.Dat[6], Is.EqualTo((ushort)'o'));
            Assert.That(statment.Dat[7], Is.EqualTo((ushort)'"'));
            Assert.That(statment.Dat[8], Is.EqualTo(0x20));
            Assert.That(statment.Dat[9], Is.EqualTo(0x30));
            Assert.That(statment.Dat[10], Is.EqualTo(0x40));
            Assert.That(statment.Dat[11], Is.EqualTo(0x50));
        }

        [Test]
        public void ParseWhenCalledWithNotchSampleGeneratesCorrectNumberOfStatments()
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
            var directOperandFactory = new DirectOperandFactory();
            var indirectOperandFactory = new IndirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory, indirectOperandFactory);

            parser.Parse();

            var statments = parser.Statments;

            Assert.That(statments.Count, Is.EqualTo(17));
        }

        [Test]
        public void ParseWhenCalledWithHelloWorldSampleGeneratesCorrectStatments()
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
            var directOperandFactory = new DirectOperandFactory();
            var indirectOperandFactory = new IndirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory, indirectOperandFactory);

            parser.Parse();

            var statments = parser.Statments;

            Assert.That(statments.Count, Is.EqualTo(12));

            var statment5 = parser.Statments[5]; // ife [data+i], 0

            Assert.That(statment5.Menemonic, Is.EqualTo("IFE"));
            Assert.That(statment5.Opcode, Is.EqualTo((int)BasicOpcode.OpIfe));
            Assert.That(statment5.OperandA, Is.InstanceOf(typeof(IndirectNextWordOffsetOperand)));
            Assert.That(statment5.OperandA.Label, Is.EqualTo("data"));
            Assert.That(statment5.OperandA.RegisterValue, Is.EqualTo((int)RegisterIdentifier.RegI));
            Assert.That(statment5.OperandB, Is.InstanceOf(typeof(NextWordOperand)));
            Assert.That(statment5.OperandB.NextWord, Is.EqualTo(0));

            var statment7 = parser.Statments[7]; // set [0x8000+i], [data+i]

            Assert.That(statment7.Menemonic, Is.EqualTo("SET"));
            Assert.That(statment7.Opcode, Is.EqualTo((int)BasicOpcode.OpSet));
            Assert.That(statment7.OperandA, Is.InstanceOf(typeof(IndirectNextWordOffsetOperand)));
            Assert.That(statment7.OperandA.NextWord, Is.EqualTo(0x8000));
            Assert.That(statment7.OperandA.RegisterValue, Is.EqualTo((int)RegisterIdentifier.RegI));
            Assert.That(statment7.OperandB, Is.InstanceOf(typeof(IndirectNextWordOffsetOperand)));
            Assert.That(statment7.OperandB.Label, Is.EqualTo("data"));
            Assert.That(statment7.OperandB.RegisterValue, Is.EqualTo((int)RegisterIdentifier.RegI));
        }
    }
}
