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
    using Operands;
    using Parser;

    public class Assembler
    {
        private const int OpcodeWidth = 4;
        private const int OperandLiteralMax = 0x1F;

        private readonly IDictionary<string, int> labelAddresses;
        private readonly IDictionary<int, string> labelReferences;

        private readonly IList<ushort> program;

        private Statment currentStatment;
        private ushort currentOpcode;

        public Assembler()
        {
            this.program = new List<ushort>();
            this.labelAddresses = new Dictionary<string, int>();
            this.labelReferences = new Dictionary<int, string>();
        }

        public IList<ushort> AssembleStatments(IEnumerable<Statment> statments)
        {
            foreach (var statment in statments)
            {
                this.currentStatment = statment;
                this.AssembleStatment();
            }

            this.ResolveLabelReferences();

            return this.program;
        }

        private void AssembleStatment()
        {
            this.AssembleLabel();

            if (this.currentStatment.Dat.Count != 0)
            {
                foreach (var dat in this.currentStatment.Dat)
                {
                    this.program.Add((ushort)dat);
                }

                return;
            }

            this.AssembleOperands();
        }

        private void AssembleLabel()
        {
            if (!string.IsNullOrEmpty(this.currentStatment.Label))
            {
                this.labelAddresses[this.currentStatment.Label.Substring(1)] = this.program.Count;
            }
        }

        private void AssembleOperands()
        {
            this.currentOpcode = (ushort)this.currentStatment.Opcode;

            if (this.currentOpcode == 0)
            {
                this.AssembleSpecialOperand();
            }
            else
            {
                this.AssembleBasicOperand();
            }
        }

        private void AssembleBasicOperand()
        {
            this.currentOpcode |= this.currentStatment.OperandA.AssembleOperand(0);
            this.currentOpcode |= this.currentStatment.OperandB.AssembleOperand(1);

            this.program.Add(this.currentOpcode);

            this.AssembleNextWordOperand(this.currentStatment.OperandA);
            this.AssembleNextWordOperand(this.currentStatment.OperandB);
        }

        private void AssembleSpecialOperand()
        {
            if (this.currentStatment.OperandB != null)
            {
                throw new Exception("Non-basic opcode must have single operand.");
            }

            this.currentOpcode |= (ushort)NonBasicOpcode.OpJsr << OpcodeWidth;
            this.currentOpcode |= this.currentStatment.OperandA.AssembleOperand(1);
            this.program.Add(this.currentOpcode);
            this.AssembleNextWordOperand(this.currentStatment.OperandA);
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
