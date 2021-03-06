﻿// --------------------------------------------------------------------------------------------------------------------
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

        public event Action<int, ushort> MemoryWillChange;

        public event Action<int, ushort> MemoryDidChange;

        public event Action<int, ushort> ValueDidLoad;

        public event Action<int, ushort> VideoMemoryDidChange;

        public event Action<int, ushort> KeyboardMemoryDidChange;

        public ushort ReadValueAtAddress(ushort address)
        {
            var value = this.ram[address];

            this.ClearKeyboardBufferAfterRead(address);

            return value;
        }

        public void WriteValueAtAddress(int address, ushort value)
        {
            this.NotifyMemoryWillChange(address, value);

            this.ram[address] = value;

            this.NotifyMemoryDidChange(address, value);
        }

        public void LoadData(IEnumerable<ushort> data)
        {
            this.Reset();

            var address = 0;

            foreach (var value in data)
            {
                this.WriteValueAtAddress(address, value);
                
                if (this.ValueDidLoad != null)
                {
                    this.ValueDidLoad(address, value);
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

        private void NotifyMemoryDidChange(int address, ushort value)
        {
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

        private void NotifyMemoryWillChange(int address, ushort value)
        {
            if (this.MemoryWillChange != null)
            {
                this.MemoryWillChange(address, value);
            }
        }
    }
}
