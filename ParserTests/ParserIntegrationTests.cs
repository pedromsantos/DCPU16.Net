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

namespace ParserTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Lexer;
    using Lexer.Tokens;

    using Model;
    using Model.Operands;

    using NUnit.Framework;

    using Parser;

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
            var parser = new Parser(lexer);

            parser.Parse();

            var statment = parser.Statments.First();

            Assert.That(statment.Menemonic == "SET");
            Assert.That(statment.Opcode == (int)BasicOpcode.OpSet);
            Assert.That(statment.OperandA is RegisterOperand);
            Assert.That(statment.OperandA.RegisterValue == (int)RegisterIdentifier.RegI);
            Assert.That(statment.OperandB is NextWordOperand);
            Assert.That(statment.OperandB.NextWord == 10);
        }

        [Test]
        public void ParseWhenCalledWithSetMemoryAddressWithLiteralGenertesCorrectStatments()
        {
            const string Code = "SET [0x1000], 0x20";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser(lexer);

            parser.Parse();

            var statment = parser.Statments.First();

            Assert.That(statment.Menemonic == "SET");
            Assert.That(statment.Opcode == (int)BasicOpcode.OpSet);
            Assert.That(statment.OperandA is IndirectNextWordOperand);
            Assert.That(statment.OperandA.NextWord == 4096);
            Assert.That(statment.OperandB is NextWordOperand);
            Assert.That(statment.OperandB.NextWord == 32);
        }

        [Test]
        public void ParseWhenCalledWithSetPcRegisterWithLabelRefGenertesCorrectStatments()
        {
            const string Code = "SET PC, crash";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser(lexer);

            parser.Parse();

            var statment = parser.Statments.First();

            Assert.That(statment.Menemonic == "SET");
            Assert.That(statment.Opcode == (int)BasicOpcode.OpSet);
            Assert.That(statment.OperandA is ProgramCounterOperand);
            Assert.That(statment.OperandB is NextWordOperand);
            Assert.That(statment.OperandB.Label == "crash");
            Assert.That(statment.OperandB.NextWord == 0);
        }

        [Test]
        public void ParseWhenCalledWithJSRandLabelRefGenertesCorrectStatments()
        {
            const string Code = "JSR testsub";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser(lexer);

            parser.Parse();

            var statment = parser.Statments.First();

            Assert.That(statment.Menemonic == "JSR");
            Assert.That(statment.Opcode == (int)NonBasicOpcode.OpJsr);
            Assert.That(statment.OperandA is NextWordOperand);
            Assert.That(statment.OperandA.Label == "testsub");
            Assert.That(statment.OperandA.NextWord == 0);
        }

        [Test]
        public void ParseWhenCalledWithInvalidInstructionThrows()
        {
            const string Code = "JSM testsub";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser(lexer);

            Assert.Throws<Exception>(() => parser.Parse());
        }

        [Test]
        public void ParseWhenCalledWithInvalidOperandThrowss()
        {
            const string Code = "JSM \"testsub\"";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser(lexer);

            Assert.Throws<Exception>(() => parser.Parse());
        }

        [Test]
        public void ParseWhenCalledWithUnclosedBracketThrows()
        {
            const string Code = "SET [0x1000, 0x20";
            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser(lexer);

            Assert.Throws<Exception>(() => parser.Parse());
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
            var parser = new Parser(lexer);

            parser.Parse();

            var statments = parser.Statments;

            Assert.That(statments.Count == 17);
        }
    }
}
