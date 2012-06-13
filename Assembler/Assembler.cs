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

namespace Assembler
{
    using System;
    using System.Collections.Generic;

    using Model;
    using Model.Operands;

    public class Assembler
    {
        protected const int OperandWidth = 6;
        protected const int OperandLiteralMax = 0x1F;

        private readonly IDictionary<string, int> labelDefinitions;
        private readonly IDictionary<int, string> labelReferences;

        private readonly IList<ushort> program;

        public Assembler()
        {
            this.labelDefinitions = new Dictionary<string, int>();
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
                this.labelDefinitions[statment.Label.Substring(1)] = this.program.Count;
            }

            if (opcode == 0)
            {
                if (statment.OperandB != null)
                {
                    throw new Exception("Non-basic opcode must have single operand.");
                }

                opcode = 0;
                opcode |= (int)NonBasicOpcode.OpJsr << OperandWidth;
                opcode |= statment.OperandA.AssembleOperand(0);
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
            if (operand is IndirectNextWordOffsetOperand)
            {
                this.program.Add((ushort)operand.NextWord);
            }

            if (operand is NextWordOperand || operand is IndirectNextWordOperand)
            {
                if (!string.IsNullOrEmpty(operand.Label))
                {
                    this.labelReferences[this.program.Count] = operand.Label;
                }
                else if (operand.NextWord > OperandLiteralMax)
                {
                    this.program.Add((ushort)operand.NextWord);
                }
            }
        }

        private void ResolveLabelReferences()
        {
            foreach (var labelReference in this.labelReferences)
            {
                var label = this.labelReferences[labelReference.Key];

                if (this.labelDefinitions.ContainsKey(label))
                {
                    var instructionIndex = labelReference.Key - 1;
                    var labelDefinitionIndex = this.labelDefinitions[label];
                    this.program[instructionIndex] = (ushort)labelDefinitionIndex;
                }
            } 
        }
    }
}
