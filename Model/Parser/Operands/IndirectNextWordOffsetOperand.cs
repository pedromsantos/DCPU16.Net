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

namespace Model.Parser.Operands
{
    public class IndirectNextWordOffsetOperand : Operand
    {
        private ushort nextWordAddress;

        private ushort registerValue;

        public override ushort Read(ICpuStateOperations cpuStateManager)
        {
            var value = cpuStateManager.ReadGeneralPursoseRegisterValue((ushort)(this.OperandValue % NumberOfRegisters));
            return
                (ushort)(cpuStateManager.ReadMemoryValueAtAddress((ushort)(this.nextWordAddress + value)) & ShortMask);
        }

        public override void Write(ICpuStateOperations cpuStateManager, ushort value)
        {
            cpuStateManager.WriteMemoryValueAtAddress(((this.nextWordAddress + this.registerValue) & ShortMask), value);
        }

        public override void Process(ICpuStateOperations cpuStateManager)
        {
            var programCounter = cpuStateManager.IncrementProgramCounter();
            this.nextWordAddress = cpuStateManager.ReadMemoryValueAtAddress(programCounter);
            this.registerValue =
                cpuStateManager.ReadGeneralPursoseRegisterValue((ushort)(this.OperandValue % NumberOfRegisters));
        }

        public override string ToString()
        {
            return string.Format(
                "[{0}+{1}={2}]",
                string.Format("0x{0:X4}", this.nextWordAddress),
                RegisterOperand.ConvertRegisterIdentifierToTokenString(this.OperandValue % NumberOfRegisters),
                string.Format("0x{0:X4}", this.registerValue));
        }

        protected override ushort Assemble(ushort shift)
        {
            return (ushort)(((ushort)OperandType.OIndirectNextWordOffset + this.RegisterValue) << shift);
        }
    }
}