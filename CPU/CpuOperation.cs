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

namespace CPU
{
    using Model;

    public class CpuOperation
    {
        private readonly Operand operand;

        private readonly ICentralProcessingUnitStateOperations cpuStateManager;

        public CpuOperation(Operand operand, ICentralProcessingUnitStateOperations cpuStateManager)
        {
            this.operand = operand;
            this.cpuStateManager = cpuStateManager;
        }

        public ushort Read
        {
            get
            {
                return this.operand.Read(this.cpuStateManager);
            }
        }

        public int Write
        {
            set
            {
                if (value > ushort.MaxValue)
                {
                    this.cpuStateManager.Overflow = 0x0001;
                }
                else if (value < 0)
                {
                    this.cpuStateManager.Overflow = ushort.MaxValue;
                }

                this.operand.Write(this.cpuStateManager, (ushort)value);
            }
        }

        public void JumpSubRoutine(ushort value)
        {
            var stackPointerValue = this.cpuStateManager.DecrementStackPointer();
            this.cpuStateManager.WriteMemoryValueAtAddress(stackPointerValue, this.cpuStateManager.ProgramCounter);
            this.cpuStateManager.SetProgramCounterTovalue(value);
        }

        public void SetOverflowRegister(ushort value)
        {
            this.cpuStateManager.Overflow = value;
        }

        public void SetIgnoreNextInstruction()
        {
            this.cpuStateManager.IgnoreNextInstruction = true;
        }

        public void Process()
        {
            this.operand.Process(this.cpuStateManager);
        }
    }
}
