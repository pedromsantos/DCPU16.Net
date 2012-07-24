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

namespace Model.LexerTests
{
    using Lexer.Tokens;

    using NUnit.Framework;

    [TestFixture]
    public class InstructionTokenTests
    {
        [Test]
        public void MatchWhenCalledWhithSetInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("SET"), Is.EqualTo("SET"));
        }

        [Test]
        public void MatchWhenCalledWhithDatInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("DAT"), Is.EqualTo("DAT"));
        }

        [Test]
        public void MatchWhenCalledWhithAddInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("ADD"), Is.EqualTo("ADD"));
        }

        [Test]
        public void MatchWhenCalledWhithSubInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("SUB"), Is.EqualTo("SUB"));
        }

        [Test]
        public void MatchWhenCalledWhithMulInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("MUL"), Is.EqualTo("MUL"));
        }

        [Test]
        public void MatchWhenCalledWhithDivInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("DIV"), Is.EqualTo("DIV"));
        }

        [Test]
        public void MatchWhenCalledWhithModInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("MOD"), Is.EqualTo("MOD"));
        }

        [Test]
        public void MatchWhenCalledWhithShlInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("SHL"), Is.EqualTo("SHL"));
        }

        [Test]
        public void MatchWhenCalledWhithShrInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("SHR"), Is.EqualTo("SHR"));
        }

        [Test]
        public void MatchWhenCalledWhithAndInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("AND"), Is.EqualTo("AND"));
        }

        [Test]
        public void MatchWhenCalledWhithBorInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("BOR"), Is.EqualTo("BOR"));
        }

        [Test]
        public void MatchWhenCalledWhithXorInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("XOR"), Is.EqualTo("XOR"));
        }

        [Test]
        public void MatchWhenCalledWhithIfeInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("IFE"), Is.EqualTo("IFE"));
        }

        [Test]
        public void MatchWhenCalledWhithIfnInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("IFN"), Is.EqualTo("IFN"));
        }

        [Test]
        public void MatchWhenCalledWhithIfgInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("IFG"), Is.EqualTo("IFG"));
        }

        [Test]
        public void MatchWhenCalledWhithIfbInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("IFB"), Is.EqualTo("IFB"));
        }

        [Test]
        public void MatchWhenCalledWhithJsrInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("JSR"), Is.EqualTo("JSR"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseSetInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("set"), Is.EqualTo("set"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseDatInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("dat"), Is.EqualTo("dat"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseAddInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("add"), Is.EqualTo("add"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseSubInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("sub"), Is.EqualTo("sub"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseMulInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("mul"), Is.EqualTo("mul"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseDivInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("div"), Is.EqualTo("div"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseModInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("mod"), Is.EqualTo("mod"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseShlInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("shl"), Is.EqualTo("shl"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseShrInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("shr"), Is.EqualTo("shr"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseAndInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("and"), Is.EqualTo("and"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseBorInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("bor"), Is.EqualTo("bor"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseXorInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("xor"), Is.EqualTo("xor"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseIfeInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("ife"), Is.EqualTo("ife"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseIfnInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("ifn"), Is.EqualTo("ifn"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseIfgInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("ifg"), Is.EqualTo("ifg"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseIfbInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("ifb"), Is.EqualTo("ifb"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseJsrInstructionReturnsTokenPosition()
        {
            var tokenMatcher = new InstructionToken();

            Assert.That(tokenMatcher.Match("jsr"), Is.EqualTo("jsr"));
        }
    }
}
