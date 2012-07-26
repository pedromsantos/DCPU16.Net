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

    public abstract class Operand
    {
        protected const ushort OpcodeWidth = 4;
        protected const ushort OperandWidth = 6;
        protected const ushort OperandLiteralMax = 0x1F;
        protected const ushort OperandLiteralOffset = 0x20;
        protected const ushort ShortMask = 0xFFFF;
        protected const ushort NumberOfRegisters = 8;
        protected const ushort NumberOfLiterals = 32;
        protected const ushort ShortShift = 16;

        public ushort OperandValue { get; set; }

        public int RegisterValue { get; set; }

        public string Label { get; set; }

        public int NextWord { get; set; }

        public string Token
        {
            get
            {
                return string.Empty;
            }
        }

        public virtual ushort Read(ICpuStateOperations cpuStateManager)
        {
            return 0;
        }

        public virtual void Write(ICpuStateOperations cpuStateManager, ushort value)
        {
            throw new InvalidOperationException();
        }

        public virtual void Process(ICpuStateOperations cpuStateManager)
        {
        }

        public virtual void NoOp(ICpuStateOperations cpuStateManager)
        {
        }

        public ushort AssembleOperand(ushort index)
        {
            var shift = (ushort)(OpcodeWidth + (ushort)(index * OperandWidth));
            return this.Assemble(shift);
        }

        protected abstract ushort Assemble(ushort shift);
    }
}