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

    public class Cpu : ICpu
    {
        private readonly IInstructionBuilder instructionBuilder;
        
        private readonly Memory memory;

        private readonly Registers registers;

        private bool programCounterSet;

        public Cpu(IInstructionBuilder instructionBuilder)
            : this()
        {
            this.instructionBuilder = instructionBuilder;
        }

        protected Cpu()
        {
            this.memory = new Memory();
            this.registers = new Registers();
        }

        public event Action<ushort, Instruction> InstructionWillExecute;

        public event Action<ushort, Instruction> InstructionDidExecute;

        public event Action<int, ushort> InstructionDidLoad
        {
            add
            {
                this.memory.InstructionDidLoad += value;
            }

            remove
            {
                this.memory.InstructionDidLoad -= value;
            }
        }

        public event Action<int, ushort> MemoryWillChange
        {
            add
            {
                this.memory.MemoryWillChange += value;
            }

            remove
            {
                this.memory.MemoryWillChange -= value;
            }
        }

        public event Action<int, ushort> MemoryDidChange
        {
            add
            {
                this.memory.MemoryDidChange += value;
            }

            remove
            {
                this.memory.MemoryDidChange -= value;
            }
        }

        public event Action<int, ushort> RegisterWillChange
        {
            add
            {
                this.registers.RegisterWillChange += value;
            }

            remove
            {
                this.registers.RegisterWillChange -= value;
            }
        }

        public event Action<int, ushort> RegisterDidChange
        {
            add
            {
                this.registers.RegisterDidChange += value;
            }

            remove
            {
                this.registers.RegisterDidChange -= value;
            }
        }

        public event Action<int, ushort> ProgramCounterWillChange
        {
            add
            {
                this.registers.ProgramCounterWillChange += value;
            }

            remove
            {
                this.registers.ProgramCounterWillChange -= value;
            }
        }

        public event Action<int, ushort> ProgramCounterDidChange
        {
            add
            {
                this.registers.ProgramCounterDidChange += value;
            }

            remove
            {
                this.registers.ProgramCounterDidChange -= value;
            }
        }

        public event Action<int, ushort> StackPointerWillChange
        {
            add
            {
                this.registers.StackPointerWillChange += value;
            }

            remove
            {
                this.registers.StackPointerWillChange -= value;
            }
        }

        public event Action<int, ushort> StackPointerDidChange
        {
            add
            {
                this.registers.StackPointerDidChange += value;
            }

            remove
            {
                this.registers.StackPointerDidChange -= value;
            }
        }

        public event Action<int, ushort> OverflowWillChange
        {
            add
            {
                this.registers.OverflowWillChange += value;
            }

            remove
            {
                this.registers.OverflowWillChange -= value;
            }
        }

        public event Action<int, ushort> OverflowDidChange
        {
            add
            {
                this.registers.OverflowDidChange += value;
            }

            remove
            {
                this.registers.OverflowDidChange -= value;
            }
        }

        public event Action<int, ushort> VideoMemoryDidChange
        {
            add
            {
                this.memory.VideoMemoryDidChange += value;
            }

            remove
            {
                this.memory.VideoMemoryDidChange -= value;
            }
        }

        public event Action<int, ushort> KeyboardMemoryDidChange
        {
            add
            {
                this.memory.KeyboardMemoryDidChange += value;
            }

            remove
            {
                this.memory.KeyboardMemoryDidChange -= value;
            }
        }

        public ushort ProgramCounter
        {
            get
            {
                return this.registers.ProgramCounter;
            }
        }

        public ushort StackPointer
        {
            get
            {
                return this.registers.StackPointer;
            }
        }

        public ushort Overflow
        {
            get
            {
                return this.registers.Overflow;
            }

            set
            {
                this.registers.Overflow = value;
            }
        }

        public bool IgnoreNextInstruction { get; set; }

        public void LoadProgram(IEnumerable<ushort> program)
        {
            this.Reset();
            this.memory.LoadProgram(program);    
        }

        public bool ExecuteNextInstruction()
        {
            var rawInstruction = this.ReadValueAtProgramCounter();

            if (rawInstruction == 0x0)
            {
                return false;
            }

            var instruction = this.instructionBuilder.Build(rawInstruction, this);

            if (!this.IgnoreNextInstruction)
            {
                this.NotifyInstrictionWillExecute(rawInstruction, instruction);
                instruction.Execute();
                this.NotifyInstructionDidExacute(rawInstruction, instruction);
            }
            else
            {
                instruction.NoOp();
                this.IgnoreNextInstruction = false;
            }

            this.UpdateProgramCounter();

            return true;
        }

        public ushort ReadMemoryValueAtAddress(ushort address)
        {
            return this.memory.ReadValueAtAddress(address);
        }

        public void WriteMemoryValueAtAddress(int address, ushort value)
        {
            this.memory.WriteValueAtAddress(address, value);
        }

        public ushort ReadGeneralPursoseRegisterValue(ushort register)
        {
            return this.registers.ReadGeneralPursoseRegisterValue(register);
        }

        public void WriteGeneralPursoseRegisterValue(int register, ushort value)
        {
            this.registers.WriteGeneralPursoseRegisterValue(register, value);
        }

        public ushort IncrementProgramCounter()
        {
            return ++this.registers.ProgramCounter;
        }

        public ushort IncrementStackPointer()
        {
            return ++this.registers.StackPointer;
        }

        public ushort DecrementStackPointer()
        {
            return --this.registers.StackPointer;
        }

        public ushort ReadValueAtProgramCounter()
        {
            return this.ReadMemoryValueAtAddress(this.registers.ProgramCounter);
        }

        public ushort SetProgramCounter(ushort value)
        {
            this.programCounterSet = true;
            return this.registers.ProgramCounter = value;
        }

        public ushort SetStackPointer(ushort value)
        {
            return this.registers.StackPointer = value;
        }

        public void Reset()
        {
            this.registers.Reset();
            this.memory.Reset();
        }

        private void NotifyInstructionDidExacute(ushort rawInstruction, Instruction instruction)
        {
            if (this.InstructionDidExecute != null)
            {
                this.InstructionDidExecute(rawInstruction, instruction);
            }
        }

        private void NotifyInstrictionWillExecute(ushort rawInstruction, Instruction instruction)
        {
            if (this.InstructionWillExecute != null)
            {
                this.InstructionWillExecute(rawInstruction, instruction);
            }
        }

        private void UpdateProgramCounter()
        {
            if (!this.programCounterSet)
            {
                this.IncrementProgramCounter();
            }
            else
            {
                this.programCounterSet = false;
            }
        }
    }
}
