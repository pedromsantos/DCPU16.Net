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

namespace Model.Parser
{
    using System;
    using System.Collections.Generic;

    public class Statment
    {
        private static readonly IDictionary<string, BasicOpcode> MenemonicToOpCodeMapper = new Dictionary<string, BasicOpcode>
            {
                { "SET", BasicOpcode.OpSet },
                { "ADD", BasicOpcode.OpAdd },
                { "SUB", BasicOpcode.OpSub },
                { "MUL", BasicOpcode.OpMul },
                { "DIV", BasicOpcode.OpDiv },
                { "MOD", BasicOpcode.OpMod },
                { "SHL", BasicOpcode.OpShl },
                { "SHR", BasicOpcode.OpShr },
                { "AND", BasicOpcode.OpAnd },
                { "BOR", BasicOpcode.OpBor },
                { "XOR", BasicOpcode.OpXor },
                { "IFE", BasicOpcode.OpIfe },
                { "IFN", BasicOpcode.OpIfn },
                { "IFG", BasicOpcode.OpIfg },
                { "IFB", BasicOpcode.OpIfb },
            };

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
            if (this.menemonic == "DAT")
            {
                this.Opcode = -1;
            }
            else if (this.menemonic == "JSR")
            {
                this.Opcode = 0x0;
            }
            else if (MenemonicToOpCodeMapper.ContainsKey(this.menemonic))
            {
                this.Opcode = (int)MenemonicToOpCodeMapper[this.menemonic];
            }
            else
            {
                throw new Exception("No operand for instruction: " + this.menemonic);
            }
        }
    }
}
