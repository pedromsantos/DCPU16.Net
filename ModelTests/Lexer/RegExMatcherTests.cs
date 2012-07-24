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
    using Lexer;

    using Model.Lexer;

    using NUnit.Framework;

    [TestFixture]
    public class RegExMatcherTests
    {
        [Test]
        public void MatchWhenCalledMatchesTheProvideStringWithPatern()
        {
            var matcher = new RegExMatcher(string.Empty);

            Assert.That(matcher.Match(string.Empty), Is.EqualTo(string.Empty));
        }

        [Test]
        public void MatchWhenCalledReturnsStartIndexOfMatch()
        {
            var matcher = new RegExMatcher(string.Empty);

            Assert.That(matcher.Match(string.Empty), Is.EqualTo(string.Empty));
        }

        [Test]
        public void MatchWhenConstructedAcceptsThePatternUsedToMatch()
        {
            var matcher = new RegExMatcher("SET");

            Assert.That(matcher.Match("SET PUSH"), Is.EqualTo("SET"));
        }

        [Test]
        public void MatchWhenCalledAndMatchedTextIsNotPrefixofTextReturnsStringEmpty()
        {
            var matcher = new RegExMatcher("PUSH");

            Assert.That(matcher.Match("SET PUSH"), Is.EqualTo(string.Empty));
        }

        [Test]
        public void MatchWhenCalledAndThereIsNoMatchReturnsMinusOne()
        {
            var matcher = new RegExMatcher("POSH");

            Assert.That(matcher.Match("SET PUSH"), Is.EqualTo(string.Empty));
        }
    }
}