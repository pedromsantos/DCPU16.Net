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

    using Model.Lexer.Tokens;

    public class PeekLexer : ILexer
    {
        public PeekLexer(
            TextReader reader,
            IEnumerable<TokenBase> tokenMatchers)
        {
            this.IgnoreTokenStrategy = new IgnoreNoneTokenStrategy();
            this.ConsumeTokenStrategy = new PeekTokenStrategy();
            this.Reader = reader;
            this.TokenMatchers = tokenMatchers;
            this.NextLine();
        }
        
        public PeekLexer(
            TextReader reader, 
            IEnumerable<TokenBase> tokenMatchers, 
            IIgnoreTokenStrategy ignoreTokenStrategy, 
            IConsumeTokenStrategy consumeTokenStrategy)
        {
            this.IgnoreTokenStrategy = ignoreTokenStrategy;
            this.ConsumeTokenStrategy = consumeTokenStrategy;
            this.Reader = reader;
            this.TokenMatchers = tokenMatchers;
            this.NextLine();
        }

        public IConsumeTokenStrategy ConsumeTokenStrategy { get; set; }

        public IIgnoreTokenStrategy IgnoreTokenStrategy { get; set; }

        public string LineRemaining { get; set; }

        public int LineNumber { get; set; }

        public int Position { get; set; }

        public TextReader Reader { get; set; }

        public IEnumerable<TokenBase> TokenMatchers { get; private set; }

        public TokenBase ConsumeTokenUsingStrategy(IConsumeTokenStrategy tokenConsumingStrategy)
        {
            var oldStrategy = this.ConsumeTokenStrategy;
            this.ConsumeTokenStrategy = tokenConsumingStrategy;
            var token = this.NextToken();
            this.ConsumeTokenStrategy = oldStrategy;

            return token;
        }

        public TokenBase NextToken()
        {
            if (this.LineRemaining == null)
            {
                return null;
            }

            foreach (var matcher in this.TokenMatchers)
            {
                var matched = matcher.Match(this.LineRemaining);
                
                if (matched != string.Empty)
                {
                    var token = matcher;
                    token.Content = matched;

                    if (this.ConsumeTokenStrategy.IsTokenToBeConsumed(matcher)
                        || this.IgnoreTokenStrategy.IsTokenToBeIgnored(token))
                    {
                        this.Position += matched.Length;
                        this.LineRemaining = this.LineRemaining.Substring(matched.Length);
                    }

                    if (this.LineRemaining.Length == 0)
                    {
                        this.NextLine();
                    }

                    if (this.IgnoreTokenStrategy.IsTokenToBeIgnored(token))
                    {
                        continue;
                    }
                        
                    return token;
                }
            }

            throw new Exception(
                string.Format(
                    "Unable to match against any tokens at line {0} position {1} \"{2}\"",
                    this.LineNumber,
                    this.Position,
                    this.LineRemaining));
        }

        private void NextLine()
        {
            do
            {
                this.LineRemaining = this.Reader.ReadLine();
                ++this.LineNumber;
                this.Position = 0;
            }
            while (this.LineRemaining != null && this.LineRemaining.Length == 0);
        }
    }
}