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

    using Model.Emulator.Instructions;

    public class Cpu : ICpu, ICpuStateOperations
    {
        private readonly IInstructionBuilder instructionBuilder;
        
        private readonly Registers registers;

        private bool programCounterSet;

        public Cpu(IInstructionBuilder instructionBuilder)
            : this()
        {
            this.instructionBuilder = instructionBuilder;
        }

        protected Cpu()
        {
            this.registers = new Registers();
        }

        public event Action<ushort, Instruction> InstructionWillExecute;

        public event Action<ushort, Instruction> InstructionDidExecute;

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

        public Memory Memory { private get; set; }

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

        public bool ExecuteNextInstruction()
        {
            var instruction = this.ReadNextInstruction();

            this.NotifyInstructionWillExecute(instruction.RawInstruction, instruction);
            instruction.Execute();
            this.NotifyInstructionDidExecute(instruction.RawInstruction, instruction);

            this.UpdateProgramCounter();

            return true;
        }

        public ushort ReadMemoryValueAtAddress(ushort address)
        {
            return this.Memory.ReadValueAtAddress(address);
        }

        public void WriteMemoryValueAtAddress(int address, ushort value)
        {
            this.Memory.WriteValueAtAddress(address, value);
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

        public Instruction ReadNextInstruction()
        {
            var rawInstruction = this.ReadValueAtProgramCounter();
            
            if (rawInstruction == 0x0)
            {
                return new NoOp();
            }

            if (!this.IgnoreNextInstruction)
            {
                return this.instructionBuilder.Build(rawInstruction, this);
            }

            this.IgnoreNextInstruction = false;
            return new NoOp();
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
        }

        private void NotifyInstructionDidExecute(ushort rawInstruction, Instruction instruction)
        {
            if (this.InstructionDidExecute != null)
            {
                this.InstructionDidExecute(rawInstruction, instruction);
            }
        }

        private void NotifyInstructionWillExecute(ushort rawInstruction, Instruction instruction)
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
