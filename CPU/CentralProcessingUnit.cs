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
    using System.Collections.Generic;

    using Model;

    public class CentralProcessingUnit : ICentralProcessingUnitStateOperations
    {
        private readonly IInstructionBuilder instructionBuilder;
        
        private readonly Memory memory;

        private readonly Registers registers;

        public CentralProcessingUnit(IInstructionBuilder instructionBuilder)
        {
            this.instructionBuilder = instructionBuilder;
        }

        public CentralProcessingUnit()
        {
            this.memory = new Memory();
            this.registers = new Registers();
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

            instruction.Execute();

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
            return this.registers.StackPointer++;
        }

        public ushort DecrementStackPointer()
        {
            return this.registers.StackPointer--;
        }

        public ushort ReadValueAtProgramCounter()
        {
            return this.ReadMemoryValueAtAddress(this.registers.ProgramCounter);
        }

        public ushort SetProgramCounterTovalue(ushort value)
        {
            return this.registers.ProgramCounter = value;
        }

        public void Reset()
        {
            this.registers.Reset();
            this.memory.Reset();
        }
    }
}
