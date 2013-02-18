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
    using System.Collections.Generic;

    using Lexer.Tokens;

    public class RegisterOperandBuilder : OperandBuilder
    {
        private static readonly IDictionary<string, Func<Operand>> RegisterCreationStrategyMapper =
           new Dictionary<string, Func<Operand>>
                {
                    { "PC", () => new ProgramCounterOperand() },
                    { "SP", () => new StackPointerOperand() },
                    { "O", () => new OverflowOperand() },
                    { "POP", () => new PopOperand() },
                    { "PEEK", () => new PeekOperand() },
                    { "PUSH", () => new PushOperand() },
                };

        protected override Operand CreateOperand(TokenBase token)
        {
            var registerType = token.Content.ToUpper();

            return RegisterCreationStrategyMapper.ContainsKey(registerType) ? RegisterCreationStrategyMapper[registerType]() : new RegisterOperand();
        }

        protected override void SetRegisterValue(TokenBase token)
        {
            this.Operand.RegisterValue = 
                RegisterOperand.ConvertTokenContentToRegisterIdentifier(token.Content);
        }
    }
}