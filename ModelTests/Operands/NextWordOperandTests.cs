// --------------------------------------------------------------------------------------------------------------
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

namespace ModelTests.Operands
{
    using Model;
    using Model.Operands;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class NextWordOperandTests : OperandTests
    {
        [Test]
        public void ToStringReturnsOperandStringRepresentation()
        {
            var operand = new NextWordOperand();
            Assert.That(operand.ToString(), Is.EqualTo("NW"));
        }

        [Test]
        public void ReadIncrementsProgramCounterAndReturnMemoryValuePointedByProgramCounter()
        {
            var cpuStateManager = new Mock<ICpuStateOperations>();
            var operand = new NextWordOperand();

            cpuStateManager.Setup(m => m.IncrementProgramCounter()).Returns(0x1);
            cpuStateManager.Setup(m => m.ReadValueAtProgramCounter()).Returns(0x10);
            
            var nextWordValue = operand.Read(cpuStateManager.Object);

            operand.Read(cpuStateManager.Object);

            Assert.That(nextWordValue, Is.EqualTo(0x10));
        }

        [Test]
        public void NoOpIncrementsProgramCounter()
        {
            var cpuStateManager = new Mock<ICpuStateOperations>();
            var operand = new NextWordOperand();

            cpuStateManager.Setup(m => m.IncrementProgramCounter()).Returns(0x1);
            
            operand.NoOp(cpuStateManager.Object);
            
            cpuStateManager.VerifyAll();
        }

        [Test]
        public void AssembleGeneratesCorrectValueIfOperandIsFirstOperand()
        {
            var operand = new NextWordOperand { NextWord = 0x20 };

            const int Index = 0;
            const ushort Shift = (ushort)(OpcodeWidth + (Index * OperandWidth));

            Assert.That(
                operand.AssembleOperand(Index), Is.EqualTo((ushort)((ushort)OperandType.ONextWord << Shift)));
        }

        [Test]
        public void AssembleGeneratesCorrectValueIfOperandIsSecondOperand()
        {
            var operand = new NextWordOperand { NextWord = 0x20 };

            const int Index = 1;
            const ushort Shift = (ushort)(OpcodeWidth + (Index * OperandWidth));

            Assert.That(
                operand.AssembleOperand(Index), Is.EqualTo((ushort)((ushort)OperandType.ONextWord << Shift)));
        }

        [Test]
        public void AssembleGeneratesCorrectValueIfNextWordLessThan32AndOperandIsFirstOperand()
        {
            var operand = new NextWordOperand { NextWord = 0x10 };

            const int Index = 0;
            const ushort Shift = (ushort)(OpcodeWidth + (Index * OperandWidth));

            Assert.That(
                operand.AssembleOperand(Index), Is.EqualTo((ushort)((0x10 + OperandLiteralOffset) << Shift)));
        }

        [Test]
        public void AssembleGeneratesCorrectValueIfNextWordLessThan32AndOperandIsSecondOperand()
        {
            var operand = new IndirectNextWordOperand { NextWord = 0x10 };

            const int Index = 1;
            const ushort Shift = (ushort)(OpcodeWidth + (Index * OperandWidth));

            Assert.That(
                operand.AssembleOperand(Index), Is.EqualTo((ushort)((0x10 + OperandLiteralOffset) << Shift)));
        }
    }
}