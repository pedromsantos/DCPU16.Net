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

	public delegate void RegisterChangeHandler(int register, ushort value);

    public class Registers
    {
		private ushort stackPointer;
		private ushort programCounter;
		private ushort overflow;

        protected const int NumbrOfGeneralPursposeRegisters = 8;

        public Registers()
        {
            this.GeneralPurpose = new ushort[NumbrOfGeneralPursposeRegisters];
        }

		public event RegisterChangeHandler RegisterWillChange;
		public event RegisterChangeHandler RegisterDidChange;
		public event RegisterChangeHandler ProgramCounterWillChange;
		public event RegisterChangeHandler ProgramCounterDidChange;
		public event RegisterChangeHandler StackPointerWillChange;
		public event RegisterChangeHandler StackPointerDidChange;
		public event RegisterChangeHandler OverflowWillChange;
		public event RegisterChangeHandler OverflowDidChange;

        public ushort[] GeneralPurpose { get; set; }

        public ushort StackPointer {
			get 
			{
				return stackPointer;
			}

			set 
			{
				if (StackPointerWillChange != null)
				{
					StackPointerWillChange(0, value);
				}

				stackPointer = value;

				if (StackPointerDidChange != null)
				{
					StackPointerDidChange(0, value);
				}
			}
		}

        public ushort ProgramCounter 
		{
			get 
			{
				return programCounter;
			}

			set 
			{
				if (ProgramCounterWillChange != null)
				{
					ProgramCounterWillChange(0, value);
				}

				programCounter = value;

				if (ProgramCounterDidChange != null)
				{
					ProgramCounterDidChange(0, value);
				}
			}
		}

        public ushort Overflow 
		{
			get 
			{
				return overflow;
			}

			set 
			{
				if (OverflowWillChange != null)
				{
					OverflowWillChange(0, value);
				}

				overflow = value;

				if (OverflowDidChange != null)
				{
					OverflowDidChange(0, value);
				}
			}
		}

        public ushort ReadGeneralPursoseRegisterValue(ushort register)
        {
            return this.GeneralPurpose[register];
        }

        public void WriteGeneralPursoseRegisterValue(int register, ushort value)
        {
			if (RegisterWillChange != null)
			{
				RegisterWillChange(register, value);
			}

            this.GeneralPurpose[register] = value;

			if (RegisterDidChange != null)
			{
				RegisterDidChange(register, value);
			}
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
