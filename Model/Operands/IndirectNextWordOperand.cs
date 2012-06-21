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
    public class IndirectNextWordOperand : Operand
    {
        private ushort nextWordAddress;

        public override ushort Read(ICentralProcessingUnitStateOperations cpuStateManager)
        {
            return cpuStateManager.ReadMemoryValueAtAddress(this.nextWordAddress);
        }

        public override void Write(ICentralProcessingUnitStateOperations cpuStateManager, ushort value)
        {
            cpuStateManager.WriteMemoryValueAtAddress(this.nextWordAddress, value);
        }

        public override void Process(ICentralProcessingUnitStateOperations cpuStateManager)
        {
            var programCounter = cpuStateManager.IncrementProgramCounter();
            this.nextWordAddress = cpuStateManager.ReadMemoryValueAtAddress(programCounter);
        }

        protected override ushort Assemble(ushort shift)
        {
            if ((this.NextWord <= OperandLiteralMax) && string.IsNullOrEmpty(this.Label))
            {
                return (ushort)((this.NextWord + OperandLiteralOffset) << shift);
            }

            return (ushort)((ushort)OperandType.OIndirectNextWord << shift);
        }
    }
}
