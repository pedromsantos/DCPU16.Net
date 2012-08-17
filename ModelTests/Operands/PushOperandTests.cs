namespace ModelTests.Operands
{
    using Model.Operands;

    using NUnit.Framework;

    [TestFixture]
    public class PushOperandTests : OperandTests
    {
        [Test]
        public void ToStringReturnsOperandStringRepresentation()
        {
            var operand = new PushOperand();
            Assert.That(operand.ToString(), Is.EqualTo("PUSH"));
        }
    }
}