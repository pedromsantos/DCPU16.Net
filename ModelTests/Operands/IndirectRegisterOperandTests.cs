namespace ModelTests.Operands
{
    using Model;
    using Model.Operands;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class IndirectRegisterOperandTests : OperandTests
    {

        [Test]
        public void ToStringReturnsOperandStringRepresentation()
        {
            var cpuStateManager = new Mock<ICpuStateOperations>();
            var operand = new IndirectRegisterOperand { OperandValue = 6 };

            cpuStateManager.Setup(m => m.ReadGeneralPursoseRegisterValue(0x6)).Returns(0x10);
            
            operand.Process(cpuStateManager.Object);

            var expected = string.Format("[{0}]", "I");

            Assert.That(operand.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void ProcessSetsRegisterValueToAdressOfMemory()
        {
            var cpuStateManager = new Mock<ICpuStateOperations>();
            var operand = new IndirectRegisterOperand { OperandValue = 6 };

            cpuStateManager.Setup(m => m.ReadGeneralPursoseRegisterValue(0x6)).Returns(0x10);
            
            operand.Process(cpuStateManager.Object);

            cpuStateManager.Setup(m => m.ReadMemoryValueAtAddress(0x10));

            operand.Read(cpuStateManager.Object);

            cpuStateManager.VerifyAll();
        }

        [Test]
        public void AssembleGeneratesCorrectValueIfOperandIsFirstOperand()
        {
            var operand = new IndirectRegisterOperand { RegisterValue = 0 };

            const int Index = 0;
            const ushort Shift = (ushort)(OpcodeWidth + (Index * OperandWidth));

            Assert.That(
                operand.AssembleOperand(Index), Is.EqualTo(((ushort)OperandType.OIndirectReg + 0) << Shift));
        }

        [Test]
        public void AssembleGeneratesCorrectValueIfOperandIsSecondOperand()
        {
            var operand = new IndirectRegisterOperand { RegisterValue = 0 };

            const int Index = 1;
            const ushort Shift = (ushort)(OpcodeWidth + (Index * OperandWidth));

            Assert.That(
                operand.AssembleOperand(Index), Is.EqualTo(((ushort)OperandType.OIndirectReg + 0) << Shift));
        }
    }
}