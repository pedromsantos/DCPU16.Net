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

namespace Model.Lexer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Tokens;

    public class CodeLexer : ILexer, IDisposable
    {
        private readonly TextReader reader;
        private readonly IEnumerable<TokenBase> tokenMatchers;

        public CodeLexer(
            TextReader reader,
            IEnumerable<TokenBase> tokenMatchers)
        {
            this.ConsumeTokenStrategy = new PeekTokenStrategy(new IgnoreNoneTokenStrategy());
            this.reader = reader;
            this.tokenMatchers = tokenMatchers;
            this.NextLine();
        }
        
        public CodeLexer(
            TextReader reader, 
            IEnumerable<TokenBase> tokenMatchers, 
            IConsumeTokenStrategy consumeTokenStrategy)
        {
            this.ConsumeTokenStrategy = consumeTokenStrategy;
            this.reader = reader;
            this.tokenMatchers = tokenMatchers;
            this.NextLine();
        }

        public string LineRemaining { get; private set; }

        public IConsumeTokenStrategy ConsumeTokenStrategy { private get; set; }

        public int LineNumber { get; private set; }

        public int Position { get; private set; }

        public void Dispose()
        {
            this.reader.Dispose();
        }

        public TokenBase NextToken()
        {
            if (this.LineRemaining == null)
            {
                return null;
            }

            var matcher = this.MatchToken();

            if (this.ConsumeTokenStrategy.IsTokenToBeIgnored(matcher))
            {
                matcher = this.NextToken();
            }

            return matcher;
        }

        public TokenBase NextTokenUsingStrategy(IConsumeTokenStrategy tokenConsumingStrategy)
        {
            var oldStrategy = this.ConsumeTokenStrategy;
            this.ConsumeTokenStrategy = tokenConsumingStrategy;
            var token = this.NextToken();
            this.ConsumeTokenStrategy = oldStrategy;

            return token;
        }

        private TokenBase MatchToken()
        {
            var matcher = this.tokenMatchers.FirstOrDefault(m => m.Match(this.LineRemaining) != string.Empty);

            if (matcher != null)
            {
                this.ConsumeToken(matcher);

                if (this.LineRemaining.Length == 0)
                {
                    this.NextLine();
                }   
 
                return matcher;
            }

            throw new Exception(
                string.Format(
                    "Unable to match against any tokens at line {0} position {1} \"{2}\"",
                    this.LineNumber,
                    this.Position,
                    this.LineRemaining));
        }

        private void ConsumeToken(TokenBase matcher)
        {
            this.LineRemaining = this.ConsumeTokenStrategy.ConsumeToken(this.LineRemaining, matcher);
            this.Position += this.LineRemaining.StartsWith(matcher.Content) ? 0 : matcher.Content.Length;
        }

        private void NextLine()
        {
            do
            {
                this.LineRemaining = this.reader.ReadLine();
                ++this.LineNumber;
                this.Position = 0;
            }
            while (this.LineRemaining != null && this.LineRemaining.Length == 0);
        }
    }
}