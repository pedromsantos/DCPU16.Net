namespace ModelTests.Operands
{
    using Model.Operands;

    using NUnit.Framework;

    [TestFixture]
    public class PopOperandTests : OperandTests
    {
        [Test]
        public void ToStringReturnsOperandStringRepresentation()
        {
            var operand = new PopOperand();
            Assert.That(operand.ToString(), Is.EqualTo("POP"));
        }
    }
}