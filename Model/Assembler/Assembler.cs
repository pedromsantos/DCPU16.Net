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

namespace Model.Assembler
{
    using System;
    using System.Collections.Generic;

    using Model;
    using Model.Parser;
    using Model.Parser.Operands;

    public class Assembler
    {
        protected const int OpcodeWidth = 4;
        protected const int OperandWidth = 6;
        protected const int OperandLiteralMax = 0x1F;

        private readonly IDictionary<string, int> labelAddresses;
        private readonly IDictionary<int, string> labelReferences;

        private readonly IList<ushort> program;

        public Assembler()
        {
            this.labelAddresses = new Dictionary<string, int>();
            this.labelReferences = new Dictionary<int, string>();

            this.program = new List<ushort>();
        }

        public IList<ushort> AssembleStatments(IEnumerable<Statment> statments)
        {
            foreach (var statment in statments)
            {
                this.AssembleStatment(statment);
            }

            this.ResolveLabelReferences();

            return this.program;
        }

        private void AssembleStatment(Statment statment)
        {
            var opcode = (ushort)statment.Opcode;

            if (!string.IsNullOrEmpty(statment.Label))
            {
                this.labelAddresses[statment.Label.Substring(1)] = this.program.Count;
            }

            if (statment.Dat.Count != 0)
            {
                foreach (var dat in statment.Dat)
                {
                    this.program.Add((ushort)dat);
                }

                return;
            }

            if (opcode == 0)
            {
                if (statment.OperandB != null)
                {
                    throw new Exception("Non-basic opcode must have single operand.");
                }

                opcode |= (ushort)NonBasicOpcode.OpJsr << OpcodeWidth;
                opcode |= statment.OperandA.AssembleOperand(1);
                this.program.Add(opcode);
                this.AssembleNextWordOperand(statment.OperandA);
            }
            else
            {
                opcode |= statment.OperandA.AssembleOperand(0);
                opcode |= statment.OperandB.AssembleOperand(1);

                this.program.Add(opcode);

                this.AssembleNextWordOperand(statment.OperandA);
                this.AssembleNextWordOperand(statment.OperandB);
            }
        }
        
        private void AssembleNextWordOperand(Operand operand)
        {
            if (operand is NextWordOperand || operand is IndirectNextWordOperand || operand is IndirectNextWordOffsetOperand)
            {
                if (!string.IsNullOrEmpty(operand.Label))
                {
                    this.labelReferences[this.program.Count] = operand.Label;
                    this.program.Add(0);
                }
                else if (operand.NextWord > OperandLiteralMax)
                {
                    this.program.Add((ushort)operand.NextWord);
                }
            }
        }

        private void ResolveLabelReferences()
        {
            foreach (var key in this.labelReferences.Keys)
            {
                var labelName = this.labelReferences[key];

                if (!this.labelAddresses.ContainsKey(labelName))
                {
                    throw new Exception(string.Format("Unknown label reference '{0}'", labelName));
                }

                this.program[key] = (ushort)this.labelAddresses[labelName];
            } 
        }
    }
}
