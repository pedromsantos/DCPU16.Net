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

        private ushort rawInstruction;

        public InstructionBuilder(IInstructionOperandFactory operandFactory)
        {
            this.operandFactory = operandFactory;
            this.instructionCache = new Dictionary<ushort, Instruction>();
            this.instructionMapper = new Dictionary<BasicOpcode, Func<Instruction>>
                {
                    { BasicOpcode.OpSet, () => new SetInstruction(this.rawInstruction, this.operationA, this.operationB) },
                    { BasicOpcode.OpAdd, () => new AddInstruction(this.rawInstruction, this.operationA, this.operationB) },
                    { BasicOpcode.OpSub, () => new SubInstruction(this.rawInstruction, this.operationA, this.operationB) },
                    { BasicOpcode.OpMul, () => new MulInstruction(this.rawInstruction, this.operationA, this.operationB) },
                    { BasicOpcode.OpDiv, () => new DivInstruction(this.rawInstruction, this.operationA, this.operationB) },
                    { BasicOpcode.OpMod, () => new ModInstruction(this.rawInstruction, this.operationA, this.operationB) },
                    { BasicOpcode.OpShl, () => new ShlInstruction(this.rawInstruction, this.operationA, this.operationB) },
                    { BasicOpcode.OpShr, () => new ShrInstruction(this.rawInstruction, this.operationA, this.operationB) },
                    { BasicOpcode.OpAnd, () => new AndInstruction(this.rawInstruction, this.operationA, this.operationB) },
                    { BasicOpcode.OpBor, () => new BorInstruction(this.rawInstruction, this.operationA, this.operationB) },
                    { BasicOpcode.OpXor, () => new XorInstruction(this.rawInstruction, this.operationA, this.operationB) },
                    { BasicOpcode.OpIfe, () => new IfeInstruction(this.rawInstruction, this.operationA, this.operationB) },
                    { BasicOpcode.OpIfn, () => new IfnInstruction(this.rawInstruction, this.operationA, this.operationB) },
                    { BasicOpcode.OpIfg, () => new IfgInstruction(this.rawInstruction, this.operationA, this.operationB) },
                    { BasicOpcode.OpIfb, () => new IfbInstruction(this.rawInstruction, this.operationA, this.operationB) },
                };
        }

        public Instruction Build(ushort instructionValue, ICpuStateOperations cpuState)
        {
            if (this.instructionCache.Keys.Contains(instructionValue))
            {
                return this.instructionCache[instructionValue];
            }

            this.rawInstruction = instructionValue;

            this.opcode = (ushort)(rawInstruction & OpMask);

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