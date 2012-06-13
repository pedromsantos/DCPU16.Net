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

namespace Model.Operands
{
    using System;

    using Lexer.Tokens;

    public class RegisterOperandBuilder : OperandBuilder
    {
        protected override Operand CreateOperand(TokenBase token)
        {
            if (token.Content == "PC") return new ProgramCounterOperand();
            if (token.Content == "SP") return new StackPointerOperand();
            if (token.Content == "O") return new OverflowOperand();
            if (token.Content == "POP") return new PopOperand();
            if (token.Content == "PEEK") return new PeekOperand();
            if (token.Content == "PUSH") return new PushOperand();

            return new RegisterOperand();
        }

        protected override void SetRegisterValue(TokenBase token)
        {
            this.Operand.RegisterValue = 
                ConvertTokenContentToRegisterIdentifier(token.Content);
        }

        private static int ConvertTokenContentToRegisterIdentifier(string tokenContent)
        {
            if (tokenContent == "A") return (int)RegisterIdentifier.RegA;
            if (tokenContent == "B") return (int)RegisterIdentifier.RegB;
            if (tokenContent == "C") return (int)RegisterIdentifier.RegC;
            if (tokenContent == "X") return (int)RegisterIdentifier.RegX;
            if (tokenContent == "Y") return (int)RegisterIdentifier.RegY;
            if (tokenContent == "Z") return (int)RegisterIdentifier.RegZ;
            if (tokenContent == "I") return (int)RegisterIdentifier.RegI;
            if (tokenContent == "J") return (int)RegisterIdentifier.RegJ;
            if (tokenContent == "PC") return (int)SpecialRegisterIdentifier.SregPc;
            if (tokenContent == "SP") return (int)SpecialRegisterIdentifier.SregSp;
            if (tokenContent == "O") return (int)SpecialRegisterIdentifier.SregO;
            if (tokenContent == "POP") return (int)OperandType.OPop;
            if (tokenContent == "PEEK") return (int)OperandType.OPeek;
            if (tokenContent == "PUSH") return (int)OperandType.OPush;

            throw new Exception("Invalid register name");
        }
    }
}