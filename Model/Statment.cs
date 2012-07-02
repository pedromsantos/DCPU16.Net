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

namespace Model
{
    using System;
    using System.Collections.Generic;

    public class Statment
    {
        private string menemonic;

        public Statment()
        {
            this.Dat = new List<int>();
        }

        public string Label { get; set; }

        public int Opcode { get; set; }

        public Operand OperandA { get; set; }

        public Operand OperandB { get; set; }

        public IList<int> Dat { get; set; }

        public string Menemonic
        {
            get
            {
                return this.menemonic;
            }

            set
            {
                this.menemonic = value;
                this.SetOpCodeForMenemonic();
            }
        }

        public void SetOpCodeForMenemonic()
        {
            if (this.menemonic == "SET") this.Opcode = (int)BasicOpcode.OpSet;
            else if (this.menemonic == "ADD") this.Opcode = (int)BasicOpcode.OpAdd;
            else if (this.menemonic == "SUB") this.Opcode = (int)BasicOpcode.OpSub;
            else if (this.menemonic == "MUL") this.Opcode = (int)BasicOpcode.OpMul;
            else if (this.menemonic == "DIV") this.Opcode = (int)BasicOpcode.OpDiv;
            else if (this.menemonic == "MOD") this.Opcode = (int)BasicOpcode.OpMod;
            else if (this.menemonic == "SHL") this.Opcode = (int)BasicOpcode.OpShl;
            else if (this.menemonic == "SHR") this.Opcode = (int)BasicOpcode.OpShr;
            else if (this.menemonic == "AND") this.Opcode = (int)BasicOpcode.OpAnd;
            else if (this.menemonic == "BOR") this.Opcode = (int)BasicOpcode.OpBor;
            else if (this.menemonic == "XOR") this.Opcode = (int)BasicOpcode.OpXor;
            else if (this.menemonic == "IFE") this.Opcode = (int)BasicOpcode.OpIfe;
            else if (this.menemonic == "IFN") this.Opcode = (int)BasicOpcode.OpIfn;
            else if (this.menemonic == "IFG") this.Opcode = (int)BasicOpcode.OpIfg;
            else if (this.menemonic == "IFB") this.Opcode = (int)BasicOpcode.OpIfb;
            else if (this.menemonic == "DAT") this.Opcode = -1;

            // non-basic opcodes
            else if (this.menemonic == "JSR")
            {
                this.Opcode = 0x0;
            }
            else
            {
                throw new Exception("No operand for instruction: " + this.menemonic);
            }
        }
    }
}
