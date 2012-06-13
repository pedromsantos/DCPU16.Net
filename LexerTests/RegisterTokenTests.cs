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

namespace LexerTests
{
    using Lexer.Tokens;

    using NUnit.Framework;

    [TestFixture]
    public class RegisterTokenTests
    {
        [Test]
        public void MatchWhenCalledWhithARegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("A"), Is.EqualTo("A"));
        }

        [Test]
        public void MatchWhenCalledWhithBRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("B"), Is.EqualTo("B"));
        }

        [Test]
        public void MatchWhenCalledWhithCRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("C"), Is.EqualTo("C"));
        }

        [Test]
        public void MatchWhenCalledWhithXRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("X"), Is.EqualTo("X"));
        }

        [Test]
        public void MatchWhenCalledWhithYRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("Y"), Is.EqualTo("Y"));
        }

        [Test]
        public void MatchWhenCalledWhithZRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("Z"), Is.EqualTo("Z"));
        }

        [Test]
        public void MatchWhenCalledWhithIRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("I"), Is.EqualTo("I"));
        }

        [Test]
        public void MatchWhenCalledWhithJRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("J"), Is.EqualTo("J"));
        }

        [Test]
        public void MatchWhenCalledWhithPcRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("PC"), Is.EqualTo("PC"));
        }

        [Test]
        public void MatchWhenCalledWhithSpRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("SP"), Is.EqualTo("SP"));
        }

        [Test]
        public void MatchWhenCalledWhithORegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("O"), Is.EqualTo("O"));
        }

        [Test]
        public void MatchWhenCalledWhithPushReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("PUSH"), Is.EqualTo("PUSH"));
        }

        [Test]
        public void MatchWhenCalledWhithPeekReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("PEEK"), Is.EqualTo("PEEK"));
        }

        [Test]
        public void MatchWhenCalledWhithPopReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("POP"), Is.EqualTo("POP"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseARegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("a"), Is.EqualTo("a"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseBRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("b"), Is.EqualTo("b"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseCRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("c"), Is.EqualTo("c"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseXRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("x"), Is.EqualTo("x"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseYRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("y"), Is.EqualTo("y"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseZRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("z"), Is.EqualTo("z"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseIRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("i"), Is.EqualTo("i"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseJRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("j"), Is.EqualTo("j"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCasePcRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("pc"), Is.EqualTo("pc"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseSpRegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("sp"), Is.EqualTo("sp"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCaseORegisterReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("o"), Is.EqualTo("o"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCasePushReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("push"), Is.EqualTo("push"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCasePeekReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("peek"), Is.EqualTo("peek"));
        }

        [Test]
        public void MatchWhenCalledWhithLowerCasePopReturnsTokenPosition()
        {
            var tokenMatcher = new RegisterToken();

            Assert.That(tokenMatcher.Match("pop"), Is.EqualTo("pop"));
        }
    }
}
