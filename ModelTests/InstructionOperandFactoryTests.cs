// --------------------------------------------------------------------------------------------------------------------
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

namespace ModelTests
{
    using System;

    using Model;
    using Model.Parser.Operands;

    using NUnit.Framework;

    [TestFixture]
    public class InstructionOperandFactoryTests
    {
        [Test]
        [TestCase((ushort)RegisterIdentifier.RegA, typeof(RegisterOperand))]
        [TestCase((ushort)RegisterIdentifier.RegB, typeof(RegisterOperand))]
        [TestCase((ushort)RegisterIdentifier.RegC, typeof(RegisterOperand))]
        [TestCase((ushort)RegisterIdentifier.RegI, typeof(RegisterOperand))]
        [TestCase((ushort)RegisterIdentifier.RegJ, typeof(RegisterOperand))]
        [TestCase((ushort)RegisterIdentifier.RegX, typeof(RegisterOperand))]
        [TestCase((ushort)RegisterIdentifier.RegY, typeof(RegisterOperand))]
        [TestCase((ushort)RegisterIdentifier.RegZ, typeof(RegisterOperand))]
        [TestCase((ushort)OperandType.OIndirectReg, typeof(IndirectRegisterOperand))]
        [TestCase((ushort)OperandType.OIndirectNextWordOffset, typeof(IndirectNextWordOffsetOperand))]
        [TestCase((ushort)OperandType.OPop, typeof(PopOperand))]
        [TestCase((ushort)OperandType.OPeek, typeof(PeekOperand))]
        [TestCase((ushort)OperandType.OPush, typeof(PushOperand))]
        [TestCase((ushort)OperandType.OSp, typeof(StackPointerOperand))]
        [TestCase((ushort)OperandType.OPc, typeof(ProgramCounterOperand))]
        [TestCase((ushort)OperandType.OO, typeof(OverflowOperand))]
        [TestCase((ushort)OperandType.OIndirectNextWord, typeof(IndirectNextWordOperand))]
        [TestCase((ushort)OperandType.ONextWord, typeof(NextWordOperand))]
        [TestCase((ushort)OperandType.OLiteral, typeof(LiteralOperand))]
        [TestCase((ushort)OperandType.ONull, typeof(NullOperand))]
        public void CreateWhenCalledWithOperandValueCreatesExpectedOperandType(ushort operandValue, Type expectdOperandType)
        {
            var builder = new InstructionOperandFactory();

            var operand = builder.Create(operandValue);

            Assert.That(operand, Is.InstanceOf(expectdOperandType));
        }

        [Test]
        [TestCase((ushort)RegisterIdentifier.RegA)]
        [TestCase((ushort)RegisterIdentifier.RegB)]
        [TestCase((ushort)RegisterIdentifier.RegC)]
        [TestCase((ushort)RegisterIdentifier.RegI)]
        [TestCase((ushort)RegisterIdentifier.RegJ)]
        [TestCase((ushort)RegisterIdentifier.RegX)]
        [TestCase((ushort)RegisterIdentifier.RegY)]
        [TestCase((ushort)RegisterIdentifier.RegZ)]
        [TestCase((ushort)OperandType.OIndirectReg)]
        [TestCase((ushort)OperandType.OIndirectNextWordOffset)]
        [TestCase((ushort)OperandType.OPop)]
        [TestCase((ushort)OperandType.OPeek)]
        [TestCase((ushort)OperandType.OPush)]
        [TestCase((ushort)OperandType.OSp)]
        [TestCase((ushort)OperandType.OPc)]
        [TestCase((ushort)OperandType.OO)]
        [TestCase((ushort)OperandType.OIndirectNextWord)]
        [TestCase((ushort)OperandType.ONextWord)]
        [TestCase((ushort)OperandType.OLiteral)]
        [TestCase((ushort)OperandType.ONull)]
        public void CreateWhenCalledWithOperandValueSavesOperandValueInOperand(ushort operandValue)
        {
            var builder = new InstructionOperandFactory();

            var operand = builder.Create(operandValue);

            Assert.That(operand.OperandValue, Is.EqualTo(operandValue));
        }
    }
}
