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

namespace CPUTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Assembler;

    using CPU;
    using CPU.Instructions;

    using Lexer;
    using Lexer.Tokens;

    using Model;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class InstructionIntegrationTests
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
        [TestCase((ushort)0x7c01, typeof(SetInstruction))] // SET A 
        [TestCase((ushort)0x7de1, typeof(SetInstruction))] // SET [0xxxxx] 
        [TestCase((ushort)0xA861, typeof(SetInstruction))] // SET I, 10
        [TestCase((ushort)0x9031, typeof(SetInstruction))] // SET X, 0x4
        [TestCase((ushort)0x8463, typeof(SubInstruction))] // SUB I, 1
        [TestCase((ushort)0x8463, typeof(SubInstruction))] // SUB I, 1
        [TestCase((ushort)0x7803, typeof(SubInstruction))] // SUB A,  [0xxxxx]
        [TestCase((ushort)0x9037, typeof(ShlInstruction))] // SHL X, 4
        public void BuildWhenCalledForRawInstructionBuildsExpectedInstructionInstance(ushort rawInstruction, Type expectedInstruction)
        {
            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();

            var builder = new InstructionBuilder(cpu, operandFactory);

            var instruction = builder.Build(rawInstruction);

            Assert.That(instruction, Is.InstanceOf(expectedInstruction));
        }

        [Test]
        public void InstructionBuilderWhenCalledWithNotchSampleGeneratesCorrectNumberOfInstructions()
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
                                    :crash        SET PC, crash            ; 7dc1 001a [*]";

            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser.Parser(lexer);
            parser.Parse();
            var statments = parser.Statments;
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new Mock<ICentralProcessingUnitStateOperations>();
            var operandFactory = new Mock<IInstructionOperandFactory>();
            var builder = new InstructionBuilder(cpu.Object, operandFactory.Object);

            var instructions = program.Select(builder.Build)
                .Where(instruction => instruction != null).ToList();

            Assert.That(instructions.Count, Is.EqualTo(22));
        }

        [Test]
        [TestCase("SET A, 10", (ushort)RegisterIdentifier.RegA, 10)]
        [TestCase("SET B, 10", (ushort)RegisterIdentifier.RegB, 10)]
        [TestCase("SET C, 10", (ushort)RegisterIdentifier.RegC, 10)]
        [TestCase("SET I, 10", (ushort)RegisterIdentifier.RegI, 10)]
        [TestCase("SET J, 10", (ushort)RegisterIdentifier.RegJ, 10)]
        [TestCase("SET X, 10", (ushort)RegisterIdentifier.RegX, 10)]
        [TestCase("SET Y, 10", (ushort)RegisterIdentifier.RegY, 10)]
        [TestCase("SET Z, 10", (ushort)RegisterIdentifier.RegZ, 10)]
        public void ExecuteWhenCalledWithSetRegisterWithDecimalLiteralSetsCorrectRegisterValue(string code, ushort registerAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser.Parser(lexer);
            
            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);
            
			cpu.LoadProgram(program);
            var instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.ReadGeneralPursoseRegisterValue(registerAddress), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("SET A, 0x10", (ushort)RegisterIdentifier.RegA, 0x10)]
        [TestCase("SET B, 0x10", (ushort)RegisterIdentifier.RegB, 0x10)]
        [TestCase("SET C, 0x10", (ushort)RegisterIdentifier.RegC, 0x10)]
        [TestCase("SET I, 0x10", (ushort)RegisterIdentifier.RegI, 0x10)]
        [TestCase("SET J, 0x10", (ushort)RegisterIdentifier.RegJ, 0x10)]
        [TestCase("SET X, 0x10", (ushort)RegisterIdentifier.RegX, 0x10)]
        [TestCase("SET Y, 0x10", (ushort)RegisterIdentifier.RegY, 0x10)]
        [TestCase("SET Z, 0x10", (ushort)RegisterIdentifier.RegZ, 0x10)]
        public void ExecuteWhenCalledWithSetRegisterWithHexLiteralSetsCorrectRegisterValue(string code, ushort registerAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser.Parser(lexer);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

			cpu.LoadProgram(program);
            var instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.ReadGeneralPursoseRegisterValue(registerAddress), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("SET A, 0x30", (ushort)RegisterIdentifier.RegA, 0x30)]
        [TestCase("SET B, 0x30", (ushort)RegisterIdentifier.RegB, 0x30)]
        [TestCase("SET C, 0x30", (ushort)RegisterIdentifier.RegC, 0x30)]
        [TestCase("SET I, 0x30", (ushort)RegisterIdentifier.RegI, 0x30)]
        [TestCase("SET J, 0x30", (ushort)RegisterIdentifier.RegJ, 0x30)]
        [TestCase("SET X, 0x30", (ushort)RegisterIdentifier.RegX, 0x30)]
        [TestCase("SET Y, 0x30", (ushort)RegisterIdentifier.RegY, 0x30)]
        [TestCase("SET Z, 0x30", (ushort)RegisterIdentifier.RegZ, 0x30)]
        public void ExecuteWhenCalledWithSetRegisterWithHexNextWordSetsCorrectRegisterValue(string code, ushort registerAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser.Parser(lexer);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.ReadGeneralPursoseRegisterValue(registerAddress), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("SET [A], 0x10", (ushort)0x0, 0x10)]
        [TestCase("SET [B], 0x10", (ushort)0x0, 0x10)]
        [TestCase("SET [C], 0x10", (ushort)0x0, 0x10)]
        [TestCase("SET [I], 0x10", (ushort)0x0, 0x10)]
        [TestCase("SET [J], 0x10", (ushort)0x0, 0x10)]
        [TestCase("SET [X], 0x10", (ushort)0x0, 0x10)]
        [TestCase("SET [Y], 0x10", (ushort)0x0, 0x10)]
        [TestCase("SET [Z], 0x10", (ushort)0x0, 0x10)]
        public void ExecuteWhenCalledWithSetIndirectRegisterMemoryAddressWithLiteralSetsCorrectMemoryValue(string code, ushort memoryAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser.Parser(lexer);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

			cpu.LoadProgram(program);
            var instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.ReadMemoryValueAtAddress(memoryAddress), Is.EqualTo(expectedValue));
        }

		[Test]
        [TestCase("SET [0x1000], 0x10", (ushort)0x1000, 0x10)]
        public void ExecuteWhenCalledWithSetMemoryAddressWithLiteralSetsCorrectMemoryValue(string code, ushort memoryAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser.Parser(lexer);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

			cpu.LoadProgram(program);
            var instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.ReadMemoryValueAtAddress(memoryAddress), Is.EqualTo(expectedValue));
        }

		[Test]
        [TestCase("SET [0x1000], 0x30", (ushort)0x1000, 0x30)]
        public void ExecuteWhenCalledWithSetMemoryAddressWithNextWordSetsCorrectMemoryValue(string code, ushort memoryAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser.Parser(lexer);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

			cpu.LoadProgram(program);
            var instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.ReadMemoryValueAtAddress(memoryAddress), Is.EqualTo(expectedValue));
        }
    }
}
