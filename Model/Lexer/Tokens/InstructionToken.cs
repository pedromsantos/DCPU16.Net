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

namespace Model.Lexer.Tokens
{
    using System.Collections.Generic;

    public class InstructionToken : TokenBase
    {
        private static readonly IDictionary<string, BasicOpcode> MenemonicToOpCodeMapper = new Dictionary<string, BasicOpcode>
            {
                { "DAT", BasicOpcode.OpDat },
                { "JSR", BasicOpcode.OpJsr },
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

        public InstructionToken()
        {
            this.Matcher = new RegExMatcher(@"\b(((?i)dat)|((?i)set)|((?i)add)|((?i)sub)|((?i)mul)|((?i)div)|((?i)mod)|((?i)shl)|((?i)shr)|((?i)and)|((?i)bor)|((?i)xor)|((?i)ife)|((?i)ifn)|((?i)ifg)|((?i)ifb)|((?i)jsr))\b");
        }

        public BasicOpcode Opcode { get; set; }

        public override string Match(string input)
        {
            base.Match(input);

            if (!string.IsNullOrEmpty(this.Content))
            {
                this.SetOpCodeForMenemonic(this.Content);
            }

            return this.Content;
        }

        private void SetOpCodeForMenemonic(string content)
        {
            content = content.ToUpper();
            this.Opcode = MenemonicToOpCodeMapper[content];
        }
    }
}