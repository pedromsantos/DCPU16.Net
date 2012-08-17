namespace ModelTests.Operands
{
    using Model.Operands;

    using NUnit.Framework;

    [TestFixture]
    public class OverflowOperandTests : OperandTests
    {
        [Test]
        public void ToStringReturnsOperandStringRepresentation()
        {
            var operand = new OverflowOperand();
            Assert.That(operand.ToString(), Is.EqualTo("Ov"));
        }
    }
}