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

    using Parser;

    [TestFixture]
    public class InstructionIntegrationTests
    {
        private IList<TokenBase> matchers;

        [Test]
        [TestCase((ushort)0x7c01, typeof(SetInstruction))] // SET A 
        [TestCase((ushort)0x7de1, typeof(SetInstruction))] // SET [0xxxxx] 
        [TestCase((ushort)0xA861, typeof(SetInstruction))] // SET I, 10
        [TestCase((ushort)0x9031, typeof(SetInstruction))] // SET X, 0x4
        [TestCase((ushort)0x8463, typeof(SubInstruction))] // SUB I, 1
        [TestCase((ushort)0x8463, typeof(SubInstruction))] // SUB I, 1
        [TestCase((ushort)0x7803, typeof(SubInstruction))] // SUB A,  [0xxxxx]
        [TestCase((ushort)0x9037, typeof(ShlInstruction))] // SHL X, 4
        public void BuildWhenCalledForRawInstructionBuildsExpectedInstructionInstance(
            ushort rawInstruction, Type expectedInstruction)
        {
            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();

            var builder = new InstructionBuilder(cpu, operandFactory);

            var instruction = builder.Build(rawInstruction);

            Assert.That(instruction, Is.InstanceOf(expectedInstruction));
        }

        [Test]
        [TestCase("ADD A, 0x10", (ushort)RegisterIdentifier.RegA, 0x10)]
        [TestCase("ADD B, 0x10", (ushort)RegisterIdentifier.RegB, 0x10)]
        [TestCase("ADD C, 0x10", (ushort)RegisterIdentifier.RegC, 0x10)]
        [TestCase("ADD I, 0x10", (ushort)RegisterIdentifier.RegI, 0x10)]
        [TestCase("ADD J, 0x10", (ushort)RegisterIdentifier.RegJ, 0x10)]
        [TestCase("ADD X, 0x10", (ushort)RegisterIdentifier.RegX, 0x10)]
        [TestCase("ADD Y, 0x10", (ushort)RegisterIdentifier.RegY, 0x10)]
        [TestCase("ADD Z, 0x10", (ushort)RegisterIdentifier.RegZ, 0x10)]
        public void ExecuteWhenCalledWithAddLiteralToRegisterValueSetsCorrectRegisterValue(
            string code, ushort registerAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

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

        [TestCase("SET A, 0x4\nADD A, 0xFFFF", 0x0001)]
        [TestCase("SET A, 0x4\nSUB A, 0xFFFF", 0xFFFF)]
        [TestCase("SET A, 0x4\nMUL A, 0xFFFF", (ushort)((0x4 >> 16) & 0xFFFF))]
        [TestCase("SET A, 0x4\nDIV A, 0xFFFF", (ushort)(((0x4 << 16) / 0xFFFF) & 0xFFFF))]
        public void ExecuteWhenCalledAndOperationOverflowsSetsOverflowRegister(string code, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction1 = builder.Build(program[0]);
            var instruction2 = builder.Build(program[1]);
            instruction1.Execute();
            cpu.IncrementProgramCounter();
            instruction2.Execute();

            Assert.That(cpu.Overflow, Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("SET A, 0x4\nDIV A, 0x2", (ushort)RegisterIdentifier.RegA, 0x4 / 0x2)]
        [TestCase("SET B, 0x4\nDIV B, 0x2", (ushort)RegisterIdentifier.RegB, 0x4 / 0x2)]
        [TestCase("SET C, 0x4\nDIV C, 0x2", (ushort)RegisterIdentifier.RegC, 0x4 / 0x2)]
        [TestCase("SET I, 0x4\nDIV I, 0x2", (ushort)RegisterIdentifier.RegI, 0x4 / 0x2)]
        [TestCase("SET J, 0x4\nDIV J, 0x2", (ushort)RegisterIdentifier.RegJ, 0x4 / 0x2)]
        [TestCase("SET X, 0x4\nDIV X, 0x2", (ushort)RegisterIdentifier.RegX, 0x4 / 0x2)]
        [TestCase("SET Y, 0x4\nDIV Y, 0x2", (ushort)RegisterIdentifier.RegY, 0x4 / 0x2)]
        [TestCase("SET Z, 0x4\nDIV Z, 0x2", (ushort)RegisterIdentifier.RegZ, 0x4 / 0x2)]
        public void ExecuteWhenCalledWithDivLiteralWithRegisterValueSetsCorrectRegisterValue(
            string code, ushort registerAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction1 = builder.Build(program[0]);
            var instruction2 = builder.Build(program[1]);
            instruction1.Execute();
            instruction2.Execute();

            Assert.That(cpu.ReadGeneralPursoseRegisterValue(registerAddress), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("SET A, 0x5\nMOD A, 0x2", (ushort)RegisterIdentifier.RegA, 0x5 % 0x2)]
        [TestCase("SET B, 0x5\nMOD B, 0x2", (ushort)RegisterIdentifier.RegB, 0x5 % 0x2)]
        [TestCase("SET C, 0x5\nMOD C, 0x2", (ushort)RegisterIdentifier.RegC, 0x5 % 0x2)]
        [TestCase("SET I, 0x5\nMOD I, 0x2", (ushort)RegisterIdentifier.RegI, 0x5 % 0x2)]
        [TestCase("SET J, 0x5\nMOD J, 0x2", (ushort)RegisterIdentifier.RegJ, 0x5 % 0x2)]
        [TestCase("SET X, 0x5\nMOD X, 0x2", (ushort)RegisterIdentifier.RegX, 0x5 % 0x2)]
        [TestCase("SET Y, 0x5\nMOD Y, 0x2", (ushort)RegisterIdentifier.RegY, 0x5 % 0x2)]
        [TestCase("SET Z, 0x5\nMOD Z, 0x2", (ushort)RegisterIdentifier.RegZ, 0x5 % 0x2)]
        public void ExecuteWhenCalledWithModLiteralWithRegisterValueSetsCorrectRegisterValue(
            string code, ushort registerAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            Instruction instruction1 = builder.Build(program[0]);
            Instruction instruction2 = builder.Build(program[1]);
            instruction1.Execute();
            instruction2.Execute();

            Assert.That(cpu.ReadGeneralPursoseRegisterValue(registerAddress), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("SET A, 0x2\nMUL A, 0x2", (ushort)RegisterIdentifier.RegA, 0x2 * 0x2)]
        [TestCase("SET B, 0x2\nMUL B, 0x2", (ushort)RegisterIdentifier.RegB, 0x2 * 0x2)]
        [TestCase("SET C, 0x2\nMUL C, 0x2", (ushort)RegisterIdentifier.RegC, 0x2 * 0x2)]
        [TestCase("SET I, 0x2\nMUL I, 0x2", (ushort)RegisterIdentifier.RegI, 0x2 * 0x2)]
        [TestCase("SET J, 0x2\nMUL J, 0x2", (ushort)RegisterIdentifier.RegJ, 0x2 * 0x2)]
        [TestCase("SET X, 0x2\nMUL X, 0x2", (ushort)RegisterIdentifier.RegX, 0x2 * 0x2)]
        [TestCase("SET Y, 0x2\nMUL Y, 0x2", (ushort)RegisterIdentifier.RegY, 0x2 * 0x2)]
        [TestCase("SET Z, 0x2\nMUL Z, 0x2", (ushort)RegisterIdentifier.RegZ, 0x2 * 0x2)]
        public void ExecuteWhenCalledWithMulLiteralWithRegisterValueSetsCorrectRegisterValue(
            string code, ushort registerAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction1 = builder.Build(program[0]);
            var instruction2 = builder.Build(program[1]);
            instruction1.Execute();
            instruction2.Execute();

            Assert.That(cpu.ReadGeneralPursoseRegisterValue(registerAddress), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("SET A, 0x2\nSHL A, 0x2", (ushort)RegisterIdentifier.RegA, 0x2 << 0x2)]
        [TestCase("SET B, 0x2\nSHL B, 0x2", (ushort)RegisterIdentifier.RegB, 0x2 << 0x2)]
        [TestCase("SET C, 0x2\nSHL C, 0x2", (ushort)RegisterIdentifier.RegC, 0x2 << 0x2)]
        [TestCase("SET I, 0x2\nSHL I, 0x2", (ushort)RegisterIdentifier.RegI, 0x2 << 0x2)]
        [TestCase("SET J, 0x2\nSHL J, 0x2", (ushort)RegisterIdentifier.RegJ, 0x2 << 0x2)]
        [TestCase("SET X, 0x2\nSHL X, 0x2", (ushort)RegisterIdentifier.RegX, 0x2 << 0x2)]
        [TestCase("SET Y, 0x2\nSHL Y, 0x2", (ushort)RegisterIdentifier.RegY, 0x2 << 0x2)]
        [TestCase("SET Z, 0x2\nSHL Z, 0x2", (ushort)RegisterIdentifier.RegZ, 0x2 << 0x2)]
        public void ExecuteWhenCalledWithShlLiteralWithRegisterValueSetsCorrectRegisterValue(
            string code, ushort registerAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction1 = builder.Build(program[0]);
            var instruction2 = builder.Build(program[1]);
            instruction1.Execute();
            instruction2.Execute();

            Assert.That(cpu.ReadGeneralPursoseRegisterValue(registerAddress), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("SET A, 0x2\nSHR A, 0x2", (ushort)RegisterIdentifier.RegA, 0x2 >> 0x2)]
        [TestCase("SET B, 0x2\nSHR B, 0x2", (ushort)RegisterIdentifier.RegB, 0x2 >> 0x2)]
        [TestCase("SET C, 0x2\nSHR C, 0x2", (ushort)RegisterIdentifier.RegC, 0x2 >> 0x2)]
        [TestCase("SET I, 0x2\nSHR I, 0x2", (ushort)RegisterIdentifier.RegI, 0x2 >> 0x2)]
        [TestCase("SET J, 0x2\nSHR J, 0x2", (ushort)RegisterIdentifier.RegJ, 0x2 >> 0x2)]
        [TestCase("SET X, 0x2\nSHR X, 0x2", (ushort)RegisterIdentifier.RegX, 0x2 >> 0x2)]
        [TestCase("SET Y, 0x2\nSHR Y, 0x2", (ushort)RegisterIdentifier.RegY, 0x2 >> 0x2)]
        [TestCase("SET Z, 0x2\nSHR Z, 0x2", (ushort)RegisterIdentifier.RegZ, 0x2 >> 0x2)]
        public void ExecuteWhenCalledWithShrLiteralWithRegisterValueSetsCorrectRegisterValue(
            string code, ushort registerAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction1 = builder.Build(program[0]);
            var instruction2 = builder.Build(program[1]);
            instruction1.Execute();
            instruction2.Execute();

            Assert.That(cpu.ReadGeneralPursoseRegisterValue(registerAddress), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("SET A, 0x1\nAND A, 0x1", (ushort)RegisterIdentifier.RegA, 0x1)]
        [TestCase("SET B, 0x1\nAND B, 0x0", (ushort)RegisterIdentifier.RegB, 0x0)]
        [TestCase("SET C, 0x1\nAND C, 0x1", (ushort)RegisterIdentifier.RegC, 0x1)]
        [TestCase("SET I, 0x1\nAND I, 0x0", (ushort)RegisterIdentifier.RegI, 0x0)]
        [TestCase("SET J, 0x1\nAND J, 0x1", (ushort)RegisterIdentifier.RegJ, 0x1)]
        [TestCase("SET X, 0x1\nAND X, 0x0", (ushort)RegisterIdentifier.RegX, 0x0)]
        [TestCase("SET Y, 0x1\nAND Y, 0x1", (ushort)RegisterIdentifier.RegY, 0x1)]
        [TestCase("SET Z, 0x1\nAND Z, 0x0", (ushort)RegisterIdentifier.RegZ, 0x0)]
        public void ExecuteWhenCalledWithAndLiteralWithRegisterValueSetsCorrectRegisterValue(
            string code, ushort registerAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction1 = builder.Build(program[0]);
            var instruction2 = builder.Build(program[1]);
            instruction1.Execute();
            instruction2.Execute();

            Assert.That(cpu.ReadGeneralPursoseRegisterValue(registerAddress), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("SET A, 0x1\nBOR A, 0x1", (ushort)RegisterIdentifier.RegA, 0x1)]
        [TestCase("SET B, 0x1\nBOR B, 0x0", (ushort)RegisterIdentifier.RegB, 0x1)]
        [TestCase("SET C, 0x1\nBOR C, 0x1", (ushort)RegisterIdentifier.RegC, 0x1)]
        [TestCase("SET I, 0x1\nBOR I, 0x0", (ushort)RegisterIdentifier.RegI, 0x1)]
        [TestCase("SET J, 0x1\nBOR J, 0x1", (ushort)RegisterIdentifier.RegJ, 0x1)]
        [TestCase("SET X, 0x1\nBOR X, 0x0", (ushort)RegisterIdentifier.RegX, 0x1)]
        [TestCase("SET Y, 0x1\nBOR Y, 0x1", (ushort)RegisterIdentifier.RegY, 0x1)]
        [TestCase("SET Z, 0x1\nBOR Z, 0x0", (ushort)RegisterIdentifier.RegZ, 0x1)]
        public void ExecuteWhenCalledWithBorLiteralWithRegisterValueSetsCorrectRegisterValue(
            string code, ushort registerAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction1 = builder.Build(program[0]);
            var instruction2 = builder.Build(program[1]);
            instruction1.Execute();
            instruction2.Execute();

            Assert.That(cpu.ReadGeneralPursoseRegisterValue(registerAddress), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("SET A, 0x1\nXOR A, 0x1", (ushort)RegisterIdentifier.RegA, 0x0)]
        [TestCase("SET B, 0x1\nXOR B, 0x0", (ushort)RegisterIdentifier.RegB, 0x1)]
        [TestCase("SET C, 0x1\nXOR C, 0x1", (ushort)RegisterIdentifier.RegC, 0x0)]
        [TestCase("SET I, 0x1\nXOR I, 0x0", (ushort)RegisterIdentifier.RegI, 0x1)]
        [TestCase("SET J, 0x1\nXOR J, 0x1", (ushort)RegisterIdentifier.RegJ, 0x0)]
        [TestCase("SET X, 0x1\nXOR X, 0x0", (ushort)RegisterIdentifier.RegX, 0x1)]
        [TestCase("SET Y, 0x1\nXOR Y, 0x1", (ushort)RegisterIdentifier.RegY, 0x0)]
        [TestCase("SET Z, 0x1\nXOR Z, 0x0", (ushort)RegisterIdentifier.RegZ, 0x1)]
        public void ExecuteWhenCalledWithXorLiteralWithRegisterValueSetsCorrectRegisterValue(string code, ushort registerAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction1 = builder.Build(program[0]);
            var instruction2 = builder.Build(program[1]);
            instruction1.Execute();
            instruction2.Execute();

            Assert.That(cpu.ReadGeneralPursoseRegisterValue(registerAddress), Is.EqualTo(expectedValue));
        }

        [Test]
        public void ExecuteWhenCalledWithPopIncrementsStackPointer()
        {
            const string Code = @"SET PUSH, 0x10
								  SET I, POP";

            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            Instruction instruction1 = builder.Build(program[0]);
            Instruction instruction2 = builder.Build(program[1]);
            instruction1.Execute();
            instruction2.Execute();

            Assert.That(cpu.StackPointer, Is.EqualTo(0x0));
        }

        [Test]
        public void ExecuteWhenCalledWithPeekDoesNotChangeStackPointer()
        {
            const string Code = @"SET PUSH, 0x10
								  SET I, PEEK";

            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            Instruction instruction1 = builder.Build(program[0]);
            Instruction instruction2 = builder.Build(program[1]);
            instruction1.Execute();
            instruction2.Execute();

            Assert.That(cpu.StackPointer, Is.EqualTo(ushort.MaxValue));
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
        public void ExecuteWhenCalledWithSetIndirectRegisterMemoryAddressWithLiteralSetsCorrectMemoryValue(
            string code, ushort memoryAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            Instruction instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.ReadMemoryValueAtAddress(memoryAddress), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("SET [0x1000], 0x10", (ushort)0x1000, 0x10)]
        public void ExecuteWhenCalledWithSetMemoryAddressWithLiteralSetsCorrectMemoryValue(
            string code, ushort memoryAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            Instruction instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.ReadMemoryValueAtAddress(memoryAddress), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("SET [0x1000], 0x30", (ushort)0x1000, 0x30)]
        public void ExecuteWhenCalledWithSetMemoryAddressWithNextWordSetsCorrectMemoryValue(
            string code, ushort memoryAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

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
        [TestCase("SET [0x1000+I], 0x10", (ushort)(0x1000 + 0x0), 0x10)]
        public void ExecuteWhenCalledWithSetOffsetMemoryAddressWithLiteralSetsCorrectMemoryValue(
            string code, ushort memoryAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

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
        [TestCase("SET [0x1000+I], 0x30", (ushort)(0x1000 + 0x0), 0x30)]
        public void ExecuteWhenCalledWithSetOffsetMemoryAddressWithNextWordSetsCorrectMemoryValue(
            string code, ushort memoryAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

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
        public void ExecuteWhenCalledWithSetOverflowRegisterWithLiteralSetsCorrectOverflowRegisterValue()
        {
            var reader = new StringReader("SET O, 0x10");
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.Overflow, Is.EqualTo(0x10));
        }

        [Test]
        public void ExecuteWhenCalledWithSetProgramCounterWithLiteralSetsCorrectProgramCounterValue()
        {
            var reader = new StringReader("SET PC, 0x10");
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.ProgramCounter, Is.EqualTo(0x10));
        }

        [Test]
        public void ExecuteWhenCalledWithSetPushWithLiteralDecrementsStackPointerValue()
        {
            var reader = new StringReader("SET PUSH, 0x10");
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.StackPointer, Is.EqualTo(ushort.MaxValue));
        }

        [Test]
        public void ExecuteWhenCalledWithSetPushWithLiteralSetsCorrectMemoryValue()
        {
            var reader = new StringReader("SET PUSH, 0x10");
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.ReadMemoryValueAtAddress(cpu.StackPointer), Is.EqualTo(0x10));
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
        public void ExecuteWhenCalledWithSetRegisterWithDecimalLiteralSetsCorrectRegisterValue(
            string code, ushort registerAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

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
        public void ExecuteWhenCalledWithSetRegisterWithHexLiteralSetsCorrectRegisterValue(
            string code, ushort registerAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

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
        public void ExecuteWhenCalledWithSetRegisterWithHexNextWordSetsCorrectRegisterValue(
            string code, ushort registerAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

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
        public void ExecuteWhenCalledWithSetRegisterWithPopSetsCorrectRegistryValue()
        {
            const string Code = @"SET PUSH, 0x10
								  SET I, POP";

            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction1 = builder.Build(program[0]);
            var instruction2 = builder.Build(program[1]);
            instruction1.Execute();
            instruction2.Execute();

            Assert.That(cpu.ReadGeneralPursoseRegisterValue((ushort)RegisterIdentifier.RegI), Is.EqualTo(0x10));
        }

        [Test]
        public void ExecuteWhenCalledWithSetStackPointerWithLiteralSetsCorrectStackPointerValue()
        {
            var reader = new StringReader("SET SP, 0x10");
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.StackPointer, Is.EqualTo(0x10));
        }

        [Test]
        [TestCase("SET A, 0xF\nSUB A, 0x1", (ushort)RegisterIdentifier.RegA, 0xF - 0x1)]
        [TestCase("SET B, 0xF\nSUB B, 0x1", (ushort)RegisterIdentifier.RegB, 0xF - 0x1)]
        [TestCase("SET C, 0xF\nSUB C, 0x1", (ushort)RegisterIdentifier.RegC, 0xF - 0x1)]
        [TestCase("SET I, 0xF\nSUB I, 0x1", (ushort)RegisterIdentifier.RegI, 0xF - 0x1)]
        [TestCase("SET J, 0xF\nSUB J, 0x1", (ushort)RegisterIdentifier.RegJ, 0xF - 0x1)]
        [TestCase("SET X, 0xF\nSUB X, 0x1", (ushort)RegisterIdentifier.RegX, 0xF - 0x1)]
        [TestCase("SET Y, 0xF\nSUB Y, 0x1", (ushort)RegisterIdentifier.RegY, 0xF - 0x1)]
        [TestCase("SET Z, 0xF\nSUB Z, 0x1", (ushort)RegisterIdentifier.RegZ, 0xF - 0x1)]
        public void ExecuteWhenCalledWithSubLiteralToRegisterValueSetsCorrectRegisterValue(
            string code, ushort registerAddress, int expectedValue)
        {
            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction1 = builder.Build(program[0]);
            var instruction2 = builder.Build(program[1]);
            instruction1.Execute();
            instruction2.Execute();

            Assert.That(cpu.ReadGeneralPursoseRegisterValue(registerAddress), Is.EqualTo(expectedValue));
        }

        [Test]
        public void ExecuteWhenCalledWithIfEqualsAndResultIsFalseSetsIgnoreNextInstruction()
        {
            var reader = new StringReader("IFE A, 1");
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.IgnoreNextInstruction, Is.EqualTo(true));
        }

        [Test]
        public void ExecuteWhenCalledWithJsrSaveCurrentProgramCounterInStack()
        {
            var reader = new StringReader("JSR 0x04");
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            cpu.SetProgramCounter(0x10);
            var instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.ReadMemoryValueAtAddress(cpu.StackPointer), Is.EqualTo(0x10));
        }

        [Test]
        public void ExecuteWhenCalledWithJsrSetsProgramCounterToOperandValue()
        {
            var reader = new StringReader("JSR 0x04");
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            cpu.SetProgramCounter(0x10);
            var instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.ProgramCounter, Is.EqualTo(0x04));
        }

        [Test]
        public void ExecuteWhenCalledWithIfNotEqualsAndResultIsFalseSetsIgnoreNextInstruction()
        {
            var reader = new StringReader("IFN A, 0");
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.IgnoreNextInstruction, Is.EqualTo(true));
        }

        [Test]
        public void ExecuteWhenCalledWithIfGreaterAndResultIsFalseSetsIgnoreNextInstruction()
        {
            var reader = new StringReader("IFG A, 0");
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.IgnoreNextInstruction, Is.EqualTo(true));
        }

        [Test]
        public void ExecuteWhenCalledWithIfBinaryAndResultIsFalseSetsIgnoreNextInstruction()
        {
            var reader = new StringReader("IFB A, 0x0");
            var lexer = new PeekLexer(reader, this.matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            var statments = parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new CentralProcessingUnit();
            var operandFactory = new InstructionOperandFactory();
            var builder = new InstructionBuilder(cpu, operandFactory);

            cpu.LoadProgram(program);
            var instruction = builder.Build(program[0]);
            instruction.Execute();

            Assert.That(cpu.IgnoreNextInstruction, Is.EqualTo(true));
        }

        [Test]
        public void InstructionBuilderWhenCalledWithNotchSampleGeneratesCorrectNumberOfInstructions()
        {
            const string Code =
                @"  ;Try some basic stuff
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
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);

            parser.Parse();
            var statments = parser.Statments;
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var cpu = new Mock<ICentralProcessingUnitStateOperations>();
            var operandFactory = new Mock<IInstructionOperandFactory>();
            var builder = new InstructionBuilder(cpu.Object, operandFactory.Object);

            var instructions =
                program.Select(builder.Build).Where(instruction => instruction != null).ToList();

            Assert.That(instructions.Count, Is.EqualTo(22));
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