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
    using System;

    public class Registers
    {
        public const int NumbrOfGeneralPursposeRegisters = 8;

        public Registers()
        {
            this.GeneralPurpose = new ushort[NumbrOfGeneralPursposeRegisters];
        }

        public ushort[] GeneralPurpose { get; set; }

        public ushort StackPointer { get; set; }

        public ushort ProgramCounter { get; set; }

        public ushort Overflow { get; set; }

        public ushort ReadGeneralPursoseRegisterValue(ushort register)
        {
            return this.GeneralPurpose[register];
        }

        public void WriteGeneralPursoseRegisterValue(int register, ushort value)
        {
            this.GeneralPurpose[register] = value;
        }

        public void Reset()
        {
            Array.Clear(this.GeneralPurpose, 0, NumbrOfGeneralPursposeRegisters);
            this.StackPointer = 0x0;
            this.ProgramCounter = 0x0;
            this.Overflow = 0x0;
        }
    }
}
