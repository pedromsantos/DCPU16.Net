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
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    public class Emulator : IEmulator
    {
        private readonly ICpu cpu;

        public Emulator(ICpu cpu)
        {
            this.cpu = cpu;
        }

        public event InstructionExecutionHandler InstructionWillExecute
        {
            add
            {
                this.cpu.InstructionWillExecute += value;
            }

            remove
            {
                this.cpu.InstructionWillExecute -= value;
            }
        }

        public event InstructionExecutionHandler InstructionDidExecute
        {
            add
            {
                this.cpu.InstructionDidExecute += value;
            }

            remove
            {
                this.cpu.InstructionDidExecute -= value;
            }
        }

        public event MemoryChangeHandler InstructionDidLoad
        {
            add
            {
                this.cpu.InstructionDidLoad += value;
            }

            remove
            {
                this.cpu.InstructionDidLoad -= value;
            }
        }

        public event MemoryChangeHandler MemoryWillChange
        {
            add
            {
                this.cpu.MemoryWillChange += value;
            }

            remove
            {
                this.cpu.MemoryWillChange -= value;
            }
        }

        public event MemoryChangeHandler MemoryDidChange
        {
            add
            {
                this.cpu.MemoryDidChange += value;
            }

            remove
            {
                this.cpu.MemoryDidChange -= value;
            }
        }

        public event RegisterChangeHandler RegisterWillChange
        {
            add
            {
                this.cpu.RegisterWillChange += value;
            }

            remove
            {
                this.cpu.RegisterWillChange -= value;
            }
        }

        public event RegisterChangeHandler RegisterDidChange
        {
            add
            {
                this.cpu.RegisterDidChange += value;
            }

            remove
            {
                this.cpu.RegisterDidChange -= value;
            }
        }

        public event RegisterChangeHandler ProgramCounterWillChange
        {
            add
            {
                this.cpu.ProgramCounterWillChange += value;
            }

            remove
            {
                this.cpu.ProgramCounterWillChange -= value;
            }
        }

        public event RegisterChangeHandler ProgramCounterDidChange
        {
            add
            {
                this.cpu.ProgramCounterDidChange += value;
            }

            remove
            {
                this.cpu.ProgramCounterDidChange -= value;
            }
        }

        public event RegisterChangeHandler StackPointerWillChange
        {
            add
            {
                this.cpu.StackPointerWillChange += value;
            }

            remove
            {
                this.cpu.StackPointerWillChange -= value;
            }
        }

        public event RegisterChangeHandler StackPointerDidChange
        {
            add
            {
                this.cpu.StackPointerDidChange += value;
            }

            remove
            {
                this.cpu.StackPointerDidChange -= value;
            }
        }

        public event RegisterChangeHandler OverflowWillChange
        {
            add
            {
                this.cpu.OverflowWillChange += value;
            }

            remove
            {
                this.cpu.OverflowWillChange -= value;
            }
        }

        public event RegisterChangeHandler OverflowDidChange
        {
            add
            {
                this.cpu.OverflowDidChange += value;
            }

            remove
            {
                this.cpu.OverflowDidChange -= value;
            }
        }

        public event MemoryChangeHandler VideoMemoryDidChange
        {
            add
            {
                this.cpu.VideoMemoryDidChange += value;
            }

            remove
            {
                this.cpu.VideoMemoryDidChange -= value;
            }
        }

        public event MemoryChangeHandler KeyboardMemoryDidChange
        {
            add
            {
                this.cpu.KeyboardMemoryDidChange += value;
            }

            remove
            {
                this.cpu.KeyboardMemoryDidChange -= value;
            }
        }

        public bool LoadProgram(string fileName)
        {
            if (File.Exists(fileName) != true)
            {
                return false;
            }

            var programData = File.ReadAllBytes(fileName);

            if ((programData.Length % 2) != 0)
            {
                return false;
            }

            this.LoadProgram(programData);

            return true;
        }

        public void LoadProgram(byte[] data)
        {
            IList<ushort> program = new List<ushort>();

            if ((data.Length % 2) != 0)
            {
                throw new InvalidDataException("Program data size must be a power of 2");
            }

            for (var i = 1; i < data.Length; i += 2)
            {
                var word = (ushort)((data[i - 1] << 8) + (data[i] & 0xFF));
                program.Add(word);
            }

            this.cpu.LoadProgram(program);
        }

        public void RunLoadedProgram()
        {
            bool executed;

            do
            {
                executed = this.cpu.ExecuteNextInstruction();
            }
            while (executed);
        }

        public void RunLoadedProgramWithDelay(int intervalBetweenInstructions)
        {
            bool executed;

            do
            {
                executed = this.cpu.ExecuteNextInstruction();
                Thread.Sleep(intervalBetweenInstructions);
            }
            while (executed);
        }
    }
}
