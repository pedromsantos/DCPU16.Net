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

    using Model;
    using Model.Emulator.Instructions;

    public class InstructionBuilder : IInstructionBuilder
    {
        private const ushort OpMask = 0xF;
        private const ushort OperandAMask = 0x3F;
        private const ushort OperandAShift = 4;
        private const ushort OperandBMask = 0x3F;
        private const ushort OperandBShift = 10;
        
        private readonly IDictionary<BasicOpcode, Func<Instruction>> instructionMapper;

        private readonly IDictionary<ushort, Instruction> instructionCache;

        private readonly IInstructionOperandFactory operandFactory;

        private ushort opcode;

        private CpuOperation operationA;

        private CpuOperation operationB;

        public InstructionBuilder(IInstructionOperandFactory operandFactory)
        {
            this.operandFactory = operandFactory;
            this.instructionCache = new Dictionary<ushort, Instruction>();
            this.instructionMapper = new Dictionary<BasicOpcode, Func<Instruction>>
                {
                    { BasicOpcode.OpSet, () => { return new SetInstruction(this.opcode, this.operationA, this.operationB); } },
                    { BasicOpcode.OpAdd, () => { return new AddInstruction(this.opcode, this.operationA, this.operationB); } },
                    { BasicOpcode.OpSub, () => { return new SubInstruction(this.opcode, this.operationA, this.operationB); } },
                    { BasicOpcode.OpMul, () => { return new MulInstruction(this.opcode, this.operationA, this.operationB); } },
                    { BasicOpcode.OpDiv, () => { return new DivInstruction(this.opcode, this.operationA, this.operationB); } },
                    { BasicOpcode.OpMod, () => { return new ModInstruction(this.opcode, this.operationA, this.operationB); } },
                    { BasicOpcode.OpShl, () => { return new ShlInstruction(this.opcode, this.operationA, this.operationB); } },
                    { BasicOpcode.OpShr, () => { return new ShrInstruction(this.opcode, this.operationA, this.operationB); } },
                    { BasicOpcode.OpAnd, () => { return new AndInstruction(this.opcode, this.operationA, this.operationB); } },
                    { BasicOpcode.OpBor, () => { return new BorInstruction(this.opcode, this.operationA, this.operationB); } },
                    { BasicOpcode.OpXor, () => { return new XorInstruction(this.opcode, this.operationA, this.operationB); } },
                    { BasicOpcode.OpIfe, () => { return new IfeInstruction(this.opcode, this.operationA, this.operationB); } },
                    { BasicOpcode.OpIfn, () => { return new IfnInstruction(this.opcode, this.operationA, this.operationB); } },
                    { BasicOpcode.OpIfg, () => { return new IfgInstruction(this.opcode, this.operationA, this.operationB); } },
                    { BasicOpcode.OpIfb, () => { return new IfbInstruction(this.opcode, this.operationA, this.operationB); } },
                };
        }

        public Instruction Build(ushort instructionValue, ICpuStateOperations cpuState)
        {
            if (this.instructionCache.Keys.Contains(instructionValue))
            {
                return this.instructionCache[instructionValue];
            }

            this.opcode = (ushort)(instructionValue & OpMask);

            if (this.opcode == 0)
            {
                var op = (ushort)((instructionValue >> OperandAShift) & OperandAMask);

                if (op == 0x01)
                {
                    var operandValue = (ushort)((instructionValue >> OperandBShift) & OperandBMask);

                    this.operationA = new CpuOperation(this.operandFactory.Create(operandValue), cpuState);
                    this.operationB = null;

                    var jsrInstruction = new JsrInstruction(0, this.operationA, this.operationA);
                    this.instructionCache[instructionValue] = jsrInstruction;

                    return jsrInstruction;
                }

                return null;
            }

            var firstOperandValue = (ushort)((instructionValue >> OperandAShift) & OperandAMask);
            var secondOperandValue = (ushort)((instructionValue >> OperandBShift) & OperandBMask);

            this.operationA = new CpuOperation(this.operandFactory.Create(firstOperandValue), cpuState);
            this.operationB = new CpuOperation(this.operandFactory.Create(secondOperandValue), cpuState);

            var instruction = this.instructionMapper[(BasicOpcode)this.opcode]();
            this.instructionCache[instructionValue] = instruction;

            return instruction;
        }
    }
}