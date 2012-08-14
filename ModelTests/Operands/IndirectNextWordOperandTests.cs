namespace ModelTests.Operands
{
    using Model;
    using Model.Operands;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class IndirectNextWordOperandTests
    {
        private const ushort OpcodeWidth = 4;
        private const ushort OperandWidth = 6;
        private const ushort OperandLiteralOffset = 0x20;

        [Test]
        public void ProcessSetsNextWordAdressToNextInstruction()
        {
            var cpuStateManager = new Mock<ICpuStateOperations>();
            var operand = new IndirectNextWordOperand();

            cpuStateManager.Setup(m => m.IncrementProgramCounter()).Returns(0x1);
            cpuStateManager.Setup(m => m.ReadMemoryValueAtAddress(0x1)).Returns(0x10);
            
            operand.Process(cpuStateManager.Object);

            cpuStateManager.Setup(m => m.ReadMemoryValueAtAddress(0x10));

            operand.Read(cpuStateManager.Object);

            cpuStateManager.VerifyAll();
        }

        [Test]
        public void AssembleGeneratesCorrectValueIfOperandIsFirstOperand()
        {
            var operand = new IndirectNextWordOperand { NextWord = 0x20 };

            const int Index = 0;
            const ushort Shift = (ushort)(OpcodeWidth + (Index * OperandWidth));

            Assert.That(
                operand.AssembleOperand(Index), Is.EqualTo((ushort)((ushort)OperandType.OIndirectNextWord << Shift)));
        }

        [Test]
        public void AssembleGeneratesCorrectValueIfOperandIsSecondOperand()
        {
            var operand = new IndirectNextWordOperand { NextWord = 0x20 };

            const int Index = 1;
            const ushort Shift = (ushort)(OpcodeWidth + (Index * OperandWidth));

            Assert.That(
                operand.AssembleOperand(Index), Is.EqualTo((ushort)((ushort)OperandType.OIndirectNextWord << Shift)));
        }

        [Test]
        public void AssembleGeneratesCorrectValueIfNextWordLessThan32AndOperandIsFirstOperand()
        {
            var operand = new IndirectNextWordOperand { NextWord = 0x21 };

            const int Index = 0;
            const ushort Shift = (ushort)(OpcodeWidth + (Index * OperandWidth));

            Assert.That(
                operand.AssembleOperand(Index), Is.EqualTo((ushort)((ushort)OperandType.OIndirectNextWord << Shift)));
        }

        [Test]
        public void AssembleGeneratesCorrectValueIfNextWordLessThan32AndOperandIsSecondOperand()
        {
            var operand = new IndirectNextWordOperand { NextWord = 0x21 };

            const int Index = 1;
            const ushort Shift = (ushort)(OpcodeWidth + (Index * OperandWidth));

            Assert.That(
                operand.AssembleOperand(Index), Is.EqualTo((ushort)((ushort)OperandType.OIndirectNextWord << Shift)));
        }
    }
}