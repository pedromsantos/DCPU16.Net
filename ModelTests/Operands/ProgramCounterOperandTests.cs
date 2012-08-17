namespace ModelTests.Operands
{
    using Model.Operands;

    using NUnit.Framework;

    [TestFixture]
    public class ProgramCounterOperandTests : OperandTests
    {
        [Test]
        public void ToStringReturnsOperandStringRepresentation()
        {
            var operand = new ProgramCounterOperand();
            Assert.That(operand.ToString(), Is.EqualTo("PC"));
        }
    }
}