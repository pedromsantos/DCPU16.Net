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
    using System.Collections.Generic;
    using System.Diagnostics;

    using Model;

    public delegate void InstructionExecutionHandler(ushort instruction);

    public class CentralProcessingUnit : ICentralProcessingUnitStateOperations
    {
        private readonly IInstructionBuilder instructionBuilder;
        
        private readonly Memory memory;

        private readonly Registers registers;

        private bool programCounterSet;

        public CentralProcessingUnit(InstructionOperandFactory operandFactory)
            : this()
        {
            this.instructionBuilder = new InstructionBuilder(this, operandFactory);
        }

        public CentralProcessingUnit()
        {
            this.memory = new Memory();
            this.registers = new Registers();
        }

        public event InstructionExecutionHandler InstructionWillExecute;
        
        public event InstructionExecutionHandler InstructionDidExecute;

        public event MemoryChangeHandler MemoryWillChange
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

        public event MemoryChangeHandler MemoryDidChange
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

        public event RegisterChangeHandler RegisterWillChange
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

        public event RegisterChangeHandler RegisterDidChange
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

        public event RegisterChangeHandler ProgramCounterWillChange
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

        public event RegisterChangeHandler ProgramCounterDidChange
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

        public event RegisterChangeHandler StackPointerWillChange
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

        public event RegisterChangeHandler StackPointerDidChange
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

        public event RegisterChangeHandler OverflowWillChange
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

        public event RegisterChangeHandler OverflowDidChange
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

        public event MemoryChangeHandler MemoryWillChange
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

        public event MemoryChangeHandler MemoryDidChange
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

        public event RegisterChangeHandler RegisterWillChange
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

        public event RegisterChangeHandler RegisterDidChange
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

        public event RegisterChangeHandler ProgramCounterWillChange
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

        public event RegisterChangeHandler ProgramCounterDidChange
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

        public event RegisterChangeHandler StackPointerWillChange
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

        public event RegisterChangeHandler StackPointerDidChange
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

        public event RegisterChangeHandler OverflowWillChange
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

        public event RegisterChangeHandler OverflowDidChange
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

        public event MemoryChangeHandler VideoMemoryDidChange
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

        public event MemoryChangeHandler KeyboardMemoryDidChange
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

        public bool IgnoreNextInstruction { get; set; }

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

        public void LoadProgram(IEnumerable<ushort> program)
        {
            this.Reset();
            this.memory.LoadProgram(program);    
        }

        public bool ExecuteNextInstruction()
        {
            if (this.memory.ReadValueAtAddress(this.registers.ProgramCounter) == 0x0)
            {
                return false;
            }

            var rawInstruction = this.memory.ReadValueAtAddress(this.registers.ProgramCounter);

            var instruction = this.instructionBuilder.Build(rawInstruction);

            if (!this.IgnoreNextInstruction)
            {
                if (this.InstructionWillExecute != null)
                {
                    this.InstructionWillExecute(rawInstruction);
                }

                instruction.Execute();
                this.DebugShowRegisters();

                if (this.InstructionDidExecute != null)
                {
                    this.InstructionDidExecute(rawInstruction);
                }
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

        [Conditional("DEBUG")]
        private void DebugShowRegisters()
        {
            Debug.WriteLine("----------------");
            Debug.WriteLine("Register Info");
            Debug.WriteLine(
                string.Format(
                    "\tA = {0:X4},\tB = {1:X4},\tC = {2:X4}",
                    this.ReadGeneralPursoseRegisterValue((ushort)RegisterIdentifier.RegA),
                    this.ReadGeneralPursoseRegisterValue((ushort)RegisterIdentifier.RegB),
                this.ReadGeneralPursoseRegisterValue((ushort)RegisterIdentifier.RegC)));

            Debug.WriteLine(
                string.Format(
                    "\tX = {0:X4},\tY = {1:X4},\tZ = {2:X4}",
                    this.ReadGeneralPursoseRegisterValue((ushort)RegisterIdentifier.RegX),
                    this.ReadGeneralPursoseRegisterValue((ushort)RegisterIdentifier.RegY),
                this.ReadGeneralPursoseRegisterValue((ushort)RegisterIdentifier.RegZ)));

            Debug.WriteLine(
                string.Format(
                    "\tI = {0:X4},\tJ = {1:X4}",
                    this.ReadGeneralPursoseRegisterValue((ushort)RegisterIdentifier.RegI),
                    this.ReadGeneralPursoseRegisterValue((ushort)RegisterIdentifier.RegJ)));

            Debug.WriteLine(
                string.Format(
                    "\tPC = {0:X4},\tSP = {1:X4},\tO = {2:X4}",
                    this.ProgramCounter,
                    this.StackPointer,
                this.Overflow));
        }
    }
}
