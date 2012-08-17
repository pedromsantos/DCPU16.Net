namespace ModelTests.Operands
{
    using Model.Operands;

    using NUnit.Framework;

    [TestFixture]
    public class PeekOperandTests : OperandTests
    {
        [Test]
        public void ToStringReturnsOperandStringRepresentation()
        {
            var operand = new PeekOperand();
            Assert.That(operand.ToString(), Is.EqualTo("PEEK"));
        }
    }
}