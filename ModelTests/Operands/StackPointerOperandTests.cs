namespace ModelTests.Operands
{
    using Model.Operands;

    using NUnit.Framework;

    [TestFixture]
    public class StackPointerOperandTests : OperandTests
    {
        [Test]
        public void ToStringReturnsOperandStringRepresentation()
        {
            var operand = new StackPointerOperand();
            Assert.That(operand.ToString(), Is.EqualTo("SP"));
        }
    }
}