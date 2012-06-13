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

    using Lexer.Tokens;

    using Model;
    using Model.Operands;

    using NUnit.Framework;

    [TestFixture]
    public class OperandBuilderTests
    {
        [Test]
        public void BuildWhenCalledOnLabelReferenceOperandBuilderSetsNextWordOnOperandToZero()
        {
            var builder = new LabelReferenceOperandBuilder();

            var operand = builder.Build(new LabelToken());

            Assert.That(operand.NextWord == 0);
        }

        [Test]
        public void BuildWhenCalledOnLabelReferenceOperandBuilderSetsLabelToTokenContent()
        {
            var builder = new LabelReferenceOperandBuilder();

            var token = new LabelToken { Content = "label" };

            var operand = builder.Build(token);

            Assert.That(operand.Label == "label");
        }

        [Test]
        public void BuildWhenCalledOnLabelReferenceOperandBuilderCreatesNextWordOperand()
        {
            var builder = new LabelReferenceOperandBuilder();

            var token = new LabelToken { Content = "label" };

            var operand = builder.Build(token);

            Assert.That(operand, Is.TypeOf(typeof(NextWordOperand)));
        }

        [Test]
        public void BuildWhenCalledOnRegisterOperandBuilderCreatesRegisterOperand()
        {
            var builder = new RegisterOperandBuilder();

            var token = new LabelToken { Content = "A" };

            var operand = builder.Build(token);

            Assert.That(operand, Is.TypeOf(typeof(RegisterOperand)));
        }

        [Test]
        public void BuildWhenCalledOnRegisterOperandAndOperandIsPcBuilderCreatesRegisterOperand()
        {
            var builder = new RegisterOperandBuilder();

            var token = new LabelToken { Content = "PC" };

            var operand = builder.Build(token);

            Assert.That(operand, Is.TypeOf(typeof(ProgramCounterOperand)));
        }

        [Test]
        public void BuildWhenCalledOnRegisterOperandAndOperandIsSpBuilderCreatesRegisterOperand()
        {
            var builder = new RegisterOperandBuilder();

            var token = new LabelToken { Content = "SP" };

            var operand = builder.Build(token);

            Assert.That(operand, Is.TypeOf(typeof(StackPointerOperand)));
        }

        [Test]
        public void BuildWhenCalledOnRegisterOperandAndOperandIsOBuilderCreatesRegisterOperand()
        {
            var builder = new RegisterOperandBuilder();

            var token = new LabelToken { Content = "O" };

            var operand = builder.Build(token);

            Assert.That(operand, Is.TypeOf(typeof(OverflowOperand)));
        }

        [Test]
        public void BuildWhenCalledOnRegisterOperandAndOperandIsPopBuilderCreatesRegisterOperand()
        {
            var builder = new RegisterOperandBuilder();

            var token = new LabelToken { Content = "POP" };

            var operand = builder.Build(token);

            Assert.That(operand, Is.TypeOf(typeof(PopOperand)));
        }

        [Test]
        public void BuildWhenCalledOnRegisterOperandAndOperandIsPeekBuilderCreatesRegisterOperand()
        {
            var builder = new RegisterOperandBuilder();

            var token = new LabelToken { Content = "PEEK" };

            var operand = builder.Build(token);

            Assert.That(operand, Is.TypeOf(typeof(PeekOperand)));
        }

        [Test]
        public void BuildWhenCalledOnRegisterOperandAndOperandIsPushBuilderCreatesRegisterOperand()
        {
            var builder = new RegisterOperandBuilder();

            var token = new LabelToken { Content = "PUSH" };

            var operand = builder.Build(token);

            Assert.That(operand, Is.TypeOf(typeof(PushOperand)));
        }

        [Test]
        public void BuildWhenCalledOnRegisterOperandBuilderSetsRegisterValueToCorrespondingRegisterIdentifierForRegisterA()
        {
            var builder = new RegisterOperandBuilder();

            var token = new LabelToken { Content = "A" };

            var operand = builder.Build(token);

            Assert.That(operand.RegisterValue == (int)RegisterIdentifier.RegA);
        }

        [Test]
        public void BuildWhenCalledOnRegisterOperandBuilderSetsRegisterValueToCorrespondingRegisterIdentifierForRegisterB()
        {
            var builder = new RegisterOperandBuilder();

            var token = new LabelToken { Content = "B" };

            var operand = builder.Build(token);

            Assert.That(operand.RegisterValue == (int)RegisterIdentifier.RegB);
        }

        [Test]
        public void BuildWhenCalledOnRegisterOperandBuilderSetsRegisterValueToCorrespondingRegisterIdentifierForRegisterC()
        {
            var builder = new RegisterOperandBuilder();

            var token = new LabelToken { Content = "C" };

            var operand = builder.Build(token);

            Assert.That(operand.RegisterValue == (int)RegisterIdentifier.RegC);
        }

        [Test]
        public void BuildWhenCalledOnRegisterOperandBuilderSetsRegisterValueToCorrespondingRegisterIdentifierForRegisterX()
        {
            var builder = new RegisterOperandBuilder();

            var token = new LabelToken { Content = "X" };

            var operand = builder.Build(token);

            Assert.That(operand.RegisterValue == (int)RegisterIdentifier.RegX);
        }

        [Test]
        public void BuildWhenCalledOnRegisterOperandBuilderSetsRegisterValueToCorrespondingRegisterIdentifierForRegisterY()
        {
            var builder = new RegisterOperandBuilder();

            var token = new LabelToken { Content = "Y" };

            var operand = builder.Build(token);

            Assert.That(operand.RegisterValue == (int)RegisterIdentifier.RegY);
        }

        [Test]
        public void BuildWhenCalledOnRegisterOperandBuilderSetsRegisterValueToCorrespondingRegisterIdentifierForRegisterZ()
        {
            var builder = new RegisterOperandBuilder();

            var token = new LabelToken { Content = "Z" };

            var operand = builder.Build(token);

            Assert.That(operand.RegisterValue == (int)RegisterIdentifier.RegZ);
        }

        [Test]
        public void BuildWhenCalledOnRegisterOperandBuilderSetsRegisterValueToCorrespondingRegisterIdentifierForRegisterI()
        {
            var builder = new RegisterOperandBuilder();

            var token = new LabelToken { Content = "I" };

            var operand = builder.Build(token);

            Assert.That(operand.RegisterValue == (int)RegisterIdentifier.RegI);
        }

        [Test]
        public void BuildWhenCalledOnRegisterOperandBuilderSetsRegisterValueToCorrespondingRegisterIdentifierForRegisterJ()
        {
            var builder = new RegisterOperandBuilder();

            var token = new LabelToken { Content = "J" };

            var operand = builder.Build(token);

            Assert.That(operand.RegisterValue == (int)RegisterIdentifier.RegJ);
        }

        [Test]
        public void BuildWhenCalledOnRegisterOperandBuilderThrowsForInvalidRegiterName()
        {
            var builder = new RegisterOperandBuilder();

            var token = new LabelToken { Content = "AX" };

            Assert.Throws<Exception>(() => builder.Build(token));
        }

        [Test]
        public void BuildWhenCalledOnIndirectRegisterOperandBuilderSetsRegisterValueToCorrespondingRegisterIdentifierForRegister()
        {
            var builder = new IndirectRegisterOperandBuilder();

            var token = new LabelToken { Content = "J" };

            var operand = builder.Build(token);

            Assert.That(operand.RegisterValue == (int)RegisterIdentifier.RegJ);
        }

        [Test]
        public void BuildWhenCalledOnNextWordOperandBuilderSetsNextWordValueToTokenContentForHexValue()
        {
            var builder = new NextWordOperandBuilder();

            var token = new HexToken { Content = "0xF" };

            var operand = builder.Build(token);

            Assert.That(operand.NextWord == 15);
        }

        [Test]
        public void BuildWhenCalledOnNextWordOperandBuilderSetsNextWordValueToTokenContentForDecimalValue()
        {
            var builder = new NextWordOperandBuilder();

            var token = new DecimalToken { Content = "10" };

            var operand = builder.Build(token);

            Assert.That(operand.NextWord == 10);
        }

        [Test]
        public void BuildWhenCalledOnIndirectNextWordOperandBuilderSetsNextWordValueToTokenContentForDecimalValue()
        {
            var builder = new IndirectNextWordOperandBuilder();

            var token = new DecimalToken { Content = "10" };

            var operand = builder.Build(token);

            Assert.That(operand.NextWord == 10);
        }

        [Test]
        public void BuildWhenCalledOnIndirectNextWordOperandBuilderSetsNextWordValueToTokenContentForHexValue()
        {
            var builder = new IndirectNextWordOperandBuilder();

            var token = new HexToken() { Content = "0xF" };

            var operand = builder.Build(token);

            Assert.That(operand.NextWord == 15);
        }

        [Test]
        public void BuildWhenCalledOnIndirectNextWordOffsetOperandBuilderSetsNextWordValueToTokenContentForHexValue()
        {
            var leftToken = new HexToken() { Content = "0xF" };

            var builder = new IndirectNextWordOffsetOperandBuilder(leftToken);

            var rigthToken = new RegisterToken() { Content = "A" };

            var operand = builder.Build(rigthToken);

            Assert.That(operand.NextWord == 15);
        }

        [Test]
        public void BuildWhenCalledOnIndirectNextWordOffsetOperandBuilderSetsRegisterToRightTokenContent()
        {
            var leftToken = new HexToken() { Content = "0xF" };

            var builder = new IndirectNextWordOffsetOperandBuilder(leftToken);

            var rigthToken = new RegisterToken() { Content = "A" };

            var operand = builder.Build(rigthToken);

            Assert.That(operand.RegisterValue == (int)RegisterIdentifier.RegA);
        }
    }
}
