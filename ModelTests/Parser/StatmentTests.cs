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

namespace ModelTests.Parser
{
    using System;

    using Model;
    using Model.Parser;

    using NUnit.Framework;

    [TestFixture]
    public class StatmentTests
    {
        [Test]
        [TestCase("SET", BasicOpcode.OpSet)]
        [TestCase("ADD", BasicOpcode.OpAdd)]
        [TestCase("SUB", BasicOpcode.OpSub)]
        [TestCase("MUL", BasicOpcode.OpMul)]
        [TestCase("DIV", BasicOpcode.OpDiv)]
        [TestCase("MOD", BasicOpcode.OpMod)]
        [TestCase("SHL", BasicOpcode.OpShl)]
        [TestCase("SHR", BasicOpcode.OpShr)]
        [TestCase("AND", BasicOpcode.OpAnd)]
        [TestCase("BOR", BasicOpcode.OpBor)]
        [TestCase("XOR", BasicOpcode.OpXor)]
        [TestCase("IFE", BasicOpcode.OpIfe)]
        [TestCase("IFN", BasicOpcode.OpIfn)]
        [TestCase("IFG", BasicOpcode.OpIfg)]
        [TestCase("IFB", BasicOpcode.OpIfb)]
        public void SetMenemonicWhenCalledWithMenemonicSetsOpcodeToCorrectValue(string menemonic, BasicOpcode expectedOpcodeValue)
        {
            var statment = new Statment { Menemonic = menemonic };

            Assert.That(statment.Opcode, Is.EqualTo((int)expectedOpcodeValue));
        }

        [Test]
        public void SetMenemonicWhenCalledWithInvalidMenemonicThrows()
        {
            Assert.Throws<Exception>(() => new Statment { Menemonic = "JSM" });
        }
    }
}