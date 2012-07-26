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

namespace Model.Parser.Operands
{
    using System;
    using System.Collections.Generic;

    public class RegisterOperand : Operand
    {
        private static readonly IDictionary<string, int> TokenToRegisterIdentifierMapper = new Dictionary<string, int>
            {
                { "A", (int)RegisterIdentifier.RegA },
                { "B", (int)RegisterIdentifier.RegB },
                { "C", (int)RegisterIdentifier.RegC },
                { "X", (int)RegisterIdentifier.RegX },
                { "Y", (int)RegisterIdentifier.RegY },
                { "Z", (int)RegisterIdentifier.RegZ },
                { "I", (int)RegisterIdentifier.RegI },
                { "J", (int)RegisterIdentifier.RegJ },
                { "PC", (int)SpecialRegisterIdentifier.SregPc },
                { "SP", (int)SpecialRegisterIdentifier.SregSp },
                { "O", (int)SpecialRegisterIdentifier.SregO },
                { "POP", (int)OperandType.OPop },
                { "PEEK", (int)OperandType.OPeek },
                { "PUSH", (int)OperandType.OPush }
            };

        private static readonly IDictionary<int, string> RegisterIdentifierToTokenStringMapper = new Dictionary<int, string>
            {
                { (int)RegisterIdentifier.RegA, "A" },
                { (int)RegisterIdentifier.RegB, "B" },
                { (int)RegisterIdentifier.RegC, "C" },
                { (int)RegisterIdentifier.RegX, "X" },
                { (int)RegisterIdentifier.RegY, "Y" },
                { (int)RegisterIdentifier.RegZ, "Z" },
                { (int)RegisterIdentifier.RegI, "I" },
                { (int)RegisterIdentifier.RegJ, "J" },
                { (int)SpecialRegisterIdentifier.SregPc, "PC" },
                { (int)SpecialRegisterIdentifier.SregSp, "SP" },
                { (int)SpecialRegisterIdentifier.SregO, "O" },
                { 11, "POP" },
                { 12, "PEEK" },
                { 13, "PUSH" }
            };

        public static int ConvertTokenContentToRegisterIdentifier(string tokenContent)
        {
            if (!TokenToRegisterIdentifierMapper.ContainsKey(tokenContent.ToUpper()))
            {
                throw new Exception("Invalid register name");
            }

            return TokenToRegisterIdentifierMapper[tokenContent.ToUpper()];
        }

        public static string ConvertRegisterIdentifierToTokenString(int registerIdentifier)
        {
            if (!RegisterIdentifierToTokenStringMapper.ContainsKey(registerIdentifier))
            {
                throw new Exception("Invalid register name");
            }

            return RegisterIdentifierToTokenStringMapper[registerIdentifier];
        }

        public override ushort Read(ICpuStateOperations cpuStateManager)
        {
            return cpuStateManager.ReadGeneralPursoseRegisterValue(this.OperandValue);
        }

        public override void Write(ICpuStateOperations cpuStateManager, ushort value)
        {
            cpuStateManager.WriteGeneralPursoseRegisterValue(this.OperandValue, value);
        }

        public override string ToString()
        {
            return ConvertRegisterIdentifierToTokenString(this.OperandValue);
        }

        protected override ushort Assemble(ushort shift)
        {
            return (ushort)(((ushort)OperandType.OReg + (ushort)this.RegisterValue) << shift);
        }
    }
}
