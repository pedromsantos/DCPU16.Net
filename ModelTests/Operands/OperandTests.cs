namespace ModelTests.Operands
{
    using System;

    using Model;
    using Model.Operands;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class OperandTests
    {
        protected const ushort OpcodeWidth = 4;
        protected const ushort OperandWidth = 6;
        protected const ushort OperandLiteralOffset = 0x20;

        [Test]
        public void ReadThrowsInvalidOperationException()
        {
            var cpuStateManager = new Mock<ICpuStateOperations>();
            var operand = new PushOperand();

            Assert.Throws<InvalidOperationException>(() => operand.Read(cpuStateManager.Object));
        }

        [Test]
        public void WriteThrowsInvalidOperationException()
        {
            var cpuStateManager = new Mock<ICpuStateOperations>();
            var operand = new PopOperand();

            Assert.Throws<InvalidOperationException>(() => operand.Write(cpuStateManager.Object, 0));
        }
    }
}