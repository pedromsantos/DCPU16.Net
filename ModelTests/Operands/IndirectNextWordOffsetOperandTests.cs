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
    public class IndirectNextWordOffsetOperandTests : OperandTests
    {
        [Test]
        public void ToStringReturnsOperandStringRepresentation()
        {
            var cpuStateManager = new Mock<ICpuStateOperations>();
            var operand = new IndirectNextWordOffsetOperand();

            cpuStateManager.Setup(m => m.IncrementProgramCounter()).Returns(0x1);
            cpuStateManager.Setup(m => m.ReadMemoryValueAtAddress(0x1)).Returns(0x10);
            cpuStateManager.Setup(m => m.ReadGeneralPursoseRegisterValue(0x0)).Returns(0x0);

            operand.Process(cpuStateManager.Object);

            var expected = string.Format(
                "[{0}+{1}={2}]", string.Format("0x{0:X4}", 0x10), "A", string.Format("0x{0:X4}", 0x0));

            Assert.That(operand.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void ProcessSetsNextWordAdressToNextInstruction()
        {
            var cpuStateManager = new Mock<ICpuStateOperations>();
            var operand = new IndirectNextWordOffsetOperand();

            cpuStateManager.Setup(m => m.IncrementProgramCounter()).Returns(0x1);
            cpuStateManager.Setup(m => m.ReadMemoryValueAtAddress(0x1)).Returns(0x10);
            cpuStateManager.Setup(m => m.ReadGeneralPursoseRegisterValue(0x0)).Returns(0x0);
            
            operand.Process(cpuStateManager.Object);

            cpuStateManager.Setup(m => m.ReadMemoryValueAtAddress(0x10));

            operand.Read(cpuStateManager.Object);

            cpuStateManager.VerifyAll();
        }

        [Test]
        public void ProcessSetsRegisterValueToValueInRegisterIdentifiedByOperandValue()
        {
            var cpuStateManager = new Mock<ICpuStateOperations>();
            var operand = new IndirectNextWordOffsetOperand();

            cpuStateManager.Setup(m => m.IncrementProgramCounter()).Returns(0x1);
            cpuStateManager.Setup(m => m.ReadMemoryValueAtAddress(0x1)).Returns(0x0);
            cpuStateManager.Setup(m => m.ReadGeneralPursoseRegisterValue(0x6)).Returns(0x10);

            operand.OperandValue = 0x6;
            operand.Process(cpuStateManager.Object);

            cpuStateManager.Setup(m => m.ReadMemoryValueAtAddress(0x10));

            operand.Read(cpuStateManager.Object);

            cpuStateManager.VerifyAll();
        }

        [Test]
        public void AssembleGeneratesCorrectValueIfOperandIsFirstOperand()
        {
            var operand = new IndirectNextWordOffsetOperand { RegisterValue = 0 };

            const int Index = 0;
            const ushort Shift = (ushort)(OpcodeWidth + (Index * OperandWidth));

            Assert.That(
                operand.AssembleOperand(Index), Is.EqualTo((ushort)(((ushort)OperandType.OIndirectNextWordOffset + 0) << Shift)));
        }

        [Test]
        public void AssembleGeneratesCorrectValueIfOperandIsSecondOperand()
        {
            var operand = new IndirectNextWordOffsetOperand { RegisterValue = 0 };

            const int Index = 1;
            const ushort Shift = (ushort)(OpcodeWidth + (Index * OperandWidth));

            Assert.That(
                operand.AssembleOperand(Index), Is.EqualTo((ushort)(((ushort)OperandType.OIndirectNextWordOffset + 0) << Shift)));
        }
    }
}
