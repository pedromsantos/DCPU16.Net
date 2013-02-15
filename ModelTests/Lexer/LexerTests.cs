// --------------------------------------------------------------------------------------------------------------
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

namespace ModelTests.Lexer
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Model.Lexer;
    using Model.Lexer.Tokens;

    using NUnit.Framework;

    [TestFixture]
    public class LexerTests
    {
        private IList<TokenBase> matchers;

        [SetUp]
        public void Setup()
        {
            this.matchers = new List<TokenBase>
                {
                    new WhiteSpaceToken(),
                    new CommentToken(),
                    new LabelToken(),
                    new HexToken(),
                    new DecimalToken(),
                    new PluslToken(),
                    new CommaToken(),
                    new OpenBracketToken(),
                    new CloseBracketToken(),
                    new InstructionToken(),
                    new RegisterToken(),
                    new StringToken(),
                    new LabelReferenceToken()
                };
        }

        [Test]
        public void ConsumeTokenWhenCalledForEmptyInputReturnsNull()
        {
            var reader = new StringReader(string.Empty);
            var lexer = new CodeLexer(reader, null);

            Assert.That(lexer.NextToken(), Is.Null); 
        }

        [Test]
        [TestCase(" ", typeof(WhiteSpaceToken))]
        [TestCase(";comment", typeof(CommentToken))]
        [TestCase(":label", typeof(LabelToken))]
        [TestCase("[", typeof(OpenBracketToken))]
        [TestCase("]", typeof(CloseBracketToken))]
        [TestCase(",", typeof(CommaToken))]
        [TestCase("+", typeof(PluslToken))]
        [TestCase("1", typeof(DecimalToken))]
        [TestCase("0x0", typeof(HexToken))]
        [TestCase("SET", typeof(InstructionToken))]
        [TestCase("I", typeof(RegisterToken))]
        [TestCase("\"string\"", typeof(StringToken))]
        [TestCase("labelreference", typeof(LabelReferenceToken))]
        public void ConsumeTokenWhenCalledWithCommentInputReturnsCommentToken(string input, Type expectedToken)
        {
            var reader = new StringReader(input);
            var lexer = new CodeLexer(reader, this.matchers);

            Assert.That(lexer.NextToken(), Is.InstanceOf(expectedToken));
        }
        
        [Test]
        public void PeekNextTokenWhenCalledDoesNotConsumeToken()
        {
            var reader = new StringReader("SET ");
            var lexer = new CodeLexer(reader, new List<TokenBase> { new WhiteSpaceToken(), new InstructionToken() });

            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(InstructionToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(InstructionToken)));
        }

        [Test]
        public void ConsumeTokenWhenCalledAndNoTokenMatchesInputThrows()
        {
            var reader = new StringReader("A");
            var lexer = new CodeLexer(reader, new List<TokenBase> { new WhiteSpaceToken(), new InstructionToken() });

            Assert.Throws<Exception>(() => lexer.NextToken());
        }

        [Test]
        public void ConsumeTokenWhenCalledConsumesToken()
        {
            var reader = new StringReader("SET ");
            var lexer = new CodeLexer(
                reader,
                new List<TokenBase> { new WhiteSpaceToken(), new InstructionToken() },
                new ConsumeTokenStrategy(new IgnoreNoneTokenStrategy()));

            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(InstructionToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(WhiteSpaceToken)));
        }

        [Test]
        public void ConsumeTokenWhenCalledIgnoresWhiteSpace()
        {
            var reader = new StringReader(" SET");
            var lexer = new CodeLexer(
                reader,
                new List<TokenBase> { new WhiteSpaceToken(), new InstructionToken() },
                new ConsumeTokenStrategy(new IgnoreWhiteSpaceTokenStrategy()));

            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(InstructionToken)));
        }

        [Test]
        public void CanLexSource()
        {
            const string Code = @"DAT 0x10, 0x20, 0x30, 0x40, 0x50
                                 SET I, 0";
            var reader = new StringReader(Code);
            var lexer = new CodeLexer(reader, this.matchers, new ConsumeTokenStrategy(new IgnoreWhiteSpaceTokenStrategy()));

            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(InstructionToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(HexToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(CommaToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(HexToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(CommaToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(HexToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(CommaToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(HexToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(CommaToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(HexToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(InstructionToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(RegisterToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(CommaToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(DecimalToken)));
        }

        [Test]
        public void CanLexSource1()
        {
            const string Code = @"SET [0x1000], 0x20";
            var reader = new StringReader(Code);
            var lexer = new CodeLexer(reader, this.matchers, new ConsumeTokenStrategy(new IgnoreWhiteSpaceTokenStrategy()));

            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(InstructionToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(OpenBracketToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(HexToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(CloseBracketToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(CommaToken)));
            Assert.That(lexer.NextToken(), Is.InstanceOf(typeof(HexToken)));
        }
    }
}
