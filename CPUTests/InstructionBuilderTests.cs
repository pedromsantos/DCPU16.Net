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

    using CPU;
    using CPU.Instructions;

    using Model;
    using Model.Operands;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class InstructionBuilderTests
    {
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
            var cpu = new Mock<ICentralProcessingUnitStateOperations>();
            var operandFactory = new Mock<IInstructionOperandFactory>();

            operandFactory.Setup(m => m.Create(It.IsAny<ushort>())).Returns(new NullOperand());

            var builder = new InstructionBuilder(cpu.Object, operandFactory.Object);

            var instruction = builder.Build(rawInstruction);

            Assert.That(instruction, Is.InstanceOf(expectedInstruction));
        }
    }
}
