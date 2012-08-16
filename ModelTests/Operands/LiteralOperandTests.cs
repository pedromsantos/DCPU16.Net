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
    using System;

    using Model;
    using Model.Operands;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class LiteralOperandTests : OperandTests
    {
        [Test]
        public void ToStringReturnsOperandStringRepresentation()
        {
            var cpuStateManager = new Mock<ICpuStateOperations>();
            var operand = new LiteralOperand { OperandValue = 0x0 };
            operand.Process(cpuStateManager.Object);

            var expected = string.Format("0x{0:X4}", 0x0);

            Assert.That(operand.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void ProcessExtractsLiteralFromOperandValue()
        {
            var cpuStateManager = new Mock<ICpuStateOperations>();
            var operand = new LiteralOperand { OperandValue = 0x0 };
            
            operand.Process(cpuStateManager.Object);
            var literal = operand.Read(cpuStateManager.Object);
            
            Assert.That(literal, Is.EqualTo(0x0));
        }

        [Test]
        public void AssembleThrowsInvalidOperationExceptionorFirstOperand()
        {
            var operand = new LiteralOperand { OperandValue = 0x10 };
            Assert.Throws<InvalidOperationException>(() => operand.AssembleOperand(0));
        }

        [Test]
        public void AssembleThrowsInvalidOperationExceptionForSecondOperand()
        {
            var operand = new LiteralOperand { OperandValue = 0x10 };
            Assert.Throws<InvalidOperationException>(() => operand.AssembleOperand(1));
        }
    }
}