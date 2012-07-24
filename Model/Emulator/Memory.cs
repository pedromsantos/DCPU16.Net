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

namespace Model.Emulator
{
    using System;
    using System.Collections.Generic;

    public delegate void MemoryChangeHandler(int address, ushort value);

    public class Memory
    {
        private const int MemorySize = 0x10000;

        private const int KeyboardMemoryStart = 0x9000;

        private const int KeyboardMemoryEnd = 0x900F;

        private const int VideoMemoryStart = 0x8000;

        private const int VideoMemoryEnd = 0x817F;

        private const int VideoCharacterSetMemoryStart = 0x8180;

        private const int VideoCharacterSetMemoryEnd = 0x827F;

        private const int VideoMiscelaneousDataMemory = 0x8280;

        private readonly ushort[] ram;

        public Memory()
            : this(MemorySize)
        {
        }

        public Memory(uint size)
        {
            this.ram = new ushort[size];
        }

        public event MemoryChangeHandler MemoryWillChange;

        public event MemoryChangeHandler MemoryDidChange;

        public event MemoryChangeHandler InstructionDidLoad;

        public event MemoryChangeHandler VideoMemoryDidChange;

        public event MemoryChangeHandler KeyboardMemoryDidChange;

        public ushort ReadValueAtAddress(ushort address)
        {
            var value = this.ram[address];

            this.ClearKeyboardBufferAfterRead(address);

            return value;
        }

        public void WriteValueAtAddress(int address, ushort value)
        {
            if (this.MemoryWillChange != null)
            {
                this.MemoryWillChange(address, value);
            }

            this.ram[address] = value;

            if (this.VideoMemoryDidChange != null 
                && (address >= VideoMemoryStart && address <= VideoMemoryEnd))
            {
                this.VideoMemoryDidChange(address, value);
            }

            if (this.KeyboardMemoryDidChange != null
                && (address >= KeyboardMemoryStart && address <= KeyboardMemoryEnd))
            {
                this.KeyboardMemoryDidChange(address, value);
            }

            if (this.MemoryDidChange != null)
            {
                this.MemoryDidChange(address, value);
            }
        }

        public void LoadProgram(IEnumerable<ushort> program)
        {
            this.Reset();

            var address = 0;

            foreach (var instruction in program)
            {
                this.WriteValueAtAddress(address, instruction);
                
                if (this.InstructionDidLoad != null)
                {
                    this.InstructionDidLoad(address, instruction);
                }

                address++;
            }
        }

        public void Reset()
        {
            Array.Clear(this.ram, 0, MemorySize);
        }

        private void ClearKeyboardBufferAfterRead(ushort address)
        {
            if (address >= KeyboardMemoryStart && address <= KeyboardMemoryEnd)
            {
                this.WriteValueAtAddress(address, 0);
            }
        }
    }
}
