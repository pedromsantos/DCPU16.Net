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

namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Model.Operands;

    public class InstructionOperandFactory : IInstructionOperandFactory
    {
        private static readonly IDictionary<IList<int>, Func<Operand>> operandTypeMapper =
            new Dictionary<IList<int>, Func<Operand>>
                {
                    { new List<int>
                        {
                            (int)RegisterIdentifier.RegA,
                            (int)RegisterIdentifier.RegB,
                            (int)RegisterIdentifier.RegC,
                            (int)RegisterIdentifier.RegI,
                            (int)RegisterIdentifier.RegJ,
                            (int)RegisterIdentifier.RegX,
                            (int)RegisterIdentifier.RegY,
                            (int)RegisterIdentifier.RegZ,
                        }, () => { return new RegisterOperand(); }
                    },
                    { new List<int> { (int)OperandType.OIndirectReg }, () => { return new IndirectRegisterOperand(); } },
                    { new List<int> { (int)OperandType.OIndirectNextWordOffset }, () => { return new IndirectNextWordOffsetOperand(); } },
                    { new List<int> { (int)OperandType.OPop }, () => { return new PopOperand(); } },
                    { new List<int> { (int)OperandType.OPeek }, () => { return new PeekOperand(); } },
                    { new List<int> { (int)OperandType.OPush }, () => { return new PushOperand(); } },
                    { new List<int> { (int)OperandType.OSp }, () => { return new StackPointerOperand(); } },
                    { new List<int> { (int)OperandType.OPc }, () => { return new ProgramCounterOperand(); } },
                    { new List<int> { (int)OperandType.OO }, () => { return new OverflowOperand(); } },
                    { new List<int> { (int)OperandType.OIndirectNextWord }, () => { return new IndirectNextWordOperand(); } },
                    { new List<int> { (int)OperandType.ONextWord }, () => { return new NextWordOperand(); } },
                    { new List<int> { (int)OperandType.OLiteral }, () => { return new LiteralOperand(); } },
                    { new List<int> { (int)OperandType.ONull }, () => { return new NullOperand(); } },
                };

        protected Operand Operand { get; set; }

        public Operand Create(ushort operandValue)
        {
            var key = operandTypeMapper.Keys.Last(k => k.Any(e => e <= operandValue));
            this.Operand = operandTypeMapper[key]();
            this.Operand.OperandValue = operandValue;
            return this.Operand;
        }
    }
}
