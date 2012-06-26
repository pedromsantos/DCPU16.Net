﻿// --------------------------------------------------------------------------------------------------------------------
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

namespace CPUTests
{
    using System.Collections.Generic;
    using System.IO;

    using Assembler;

    using CPU;

    using Lexer;
    using Lexer.Tokens;

    using Model;

    using NUnit.Framework;

    using Parser;

    [TestFixture]
    public class CentralProcessingUnitTests
    {
        private IList<TokenBase> matchers;

        [Test]
        public void ExecuteNextInstructionWhenCalledFiresInstructionWillExecuteIfWired()
        {
            var receivedEvents = new List<ushort>();

            var reader = new StringReader("SET PUSH, 0x10");
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser(lexer);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var operandFactory = new InstructionOperandFactory();
            var cpu = new CentralProcessingUnit(operandFactory);
            cpu.InstructionWillExecute += receivedEvents.Add;
            cpu.LoadProgram(program);
            cpu.ExecuteNextInstruction();

            Assert.That(receivedEvents[0] == program[0]);
        }

        [Test]
        public void ExecuteNextInstructionWhenCalledFiresInstructionDidExecuteIfWired()
        {
            var receivedEvents = new List<ushort>();

            var reader = new StringReader("SET PUSH, 0x10");
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser(lexer);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var operandFactory = new InstructionOperandFactory();
            var cpu = new CentralProcessingUnit(operandFactory);
            cpu.InstructionDidExecute += receivedEvents.Add;
            cpu.LoadProgram(program);
            cpu.ExecuteNextInstruction();

            Assert.That(receivedEvents[0] == program[0]);
        }

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
    }
}
