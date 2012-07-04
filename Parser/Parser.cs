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

namespace Parser
{
    using System;
    using System.Collections.Generic;

    using Lexer;
    using Lexer.Tokens;

    using Model;
    using Model.Operands;

    public class Parser : IParser
    {
        private readonly IConsumeTokenStrategy consumeStrategy = new ConsumeTokenStrategy();

        private readonly IOperandFactory directOperandFactory;
        private readonly IOperandFactory indirectOperandFactory;

        private readonly ILexer lexer;

        public Parser(ILexer lexer, IOperandFactory directOperandFactory, IOperandFactory indirectOperandFactory)
        {
            this.lexer = lexer;
            this.indirectOperandFactory = indirectOperandFactory;
            this.directOperandFactory = directOperandFactory;
            lexer.ConsumeTokenStrategy = new PeekTokenStrategy();
            lexer.IgnoreTokenStrategy = new IgnoreWhiteSpaceTokenStrategy();

            this.Statments = new List<Statment>();
        }

        public IList<Statment> Statments { get; private set; }

        public IEnumerable<Statment> Parse()
        {
            while (this.ParseStatment())
                ;

            return this.Statments;
        }

        public bool ParseStatment()
        {
            this.DiscardEmptyLines();

            if (this.lexer.NextToken() == null)
            {
                return false;
            }

            var statment = new Statment();

            this.ParseLabel(statment);
            this.ParseMenemonic(statment);
            
            if (statment.Menemonic != "DAT")
            {
                this.ParseOperands(statment);
            }
            else
            {
                this.ParseData(statment);
            }

            this.Statments.Add(statment);
            this.ParseComments();

            return true;
        }

        private void DiscardEmptyLines()
        {
            TokenBase token;
            do
            {
                token = this.lexer.NextToken();

                if (token is CommentToken || token is WhiteSpaceToken)
                {
                    token = this.lexer.ConsumeTokenUsingStrategy(this.consumeStrategy);
                }
                else
                {
                    return;
                }
            }
            while (token is CommentToken || token is WhiteSpaceToken);
        }

        private void ParseLabel(Statment statment)
        {
            var token = this.lexer.NextToken();

            if (token is LabelToken)
            {
                this.lexer.ConsumeTokenUsingStrategy(this.consumeStrategy);
                statment.Label = token.Content;
            }
        }

        private void ParseMenemonic(Statment statment)
        {
            var token = this.lexer.ConsumeTokenUsingStrategy(this.consumeStrategy);

            if (!(token is InstructionToken))
            {
                throw new Exception(
                    string.Format(
                        "Expected INSTRUCTION at line {0}:{1} found '{2}'",
                        this.lexer.LineNumber,
                        this.lexer.Position, 
                    token.Content));
            }

            statment.Menemonic = token.Content.ToUpper();
        }

        private void ParseComments()
        {
            var token = this.lexer.NextToken();

            if (token is CommentToken)
            {
                this.lexer.ConsumeTokenUsingStrategy(this.consumeStrategy);
            }
        }

        private void ParseOperands(Statment statment)
        {
            statment.OperandA = this.ParseOperand();

            var token = this.lexer.ConsumeTokenUsingStrategy(this.consumeStrategy);

            if (token is CommaToken)
            {
                statment.OperandB = this.ParseOperand();
            }
        }

        private Operand ParseOperand()
        {
            var token = this.lexer.ConsumeTokenUsingStrategy(this.consumeStrategy);

            if (token is OpenBracketToken)
            {
                return this.ParseIndirectOperand();
            }

            return this.directOperandFactory.CreateOperand(token);
        }

        private Operand ParseIndirectOperand()
        {
            var token = this.lexer.ConsumeTokenUsingStrategy(this.consumeStrategy);

            Operand operand;

            if (this.lexer.NextToken() is PluslToken)
            {
                this.lexer.ConsumeTokenUsingStrategy(this.consumeStrategy);
                var rigthToken = this.lexer.ConsumeTokenUsingStrategy(this.consumeStrategy);
                operand = new IndirectNextWordOffsetOperandBuilder(token).Build(rigthToken);
            }
            else
            {
                operand = this.indirectOperandFactory.CreateOperand(token);
            }

            this.AssertIndirectOperandIsTerminatedWithACloseBracketToken();

            return operand;
        }

        private void AssertIndirectOperandIsTerminatedWithACloseBracketToken()
        {
            var token = this.lexer.ConsumeTokenUsingStrategy(this.consumeStrategy);

            if (!(token is CloseBracketToken))
            {
                throw new Exception(
                    string.Format(
                        "Expected CLOSEBRACKET at line {0}:{1} found '{2}'",
                        this.lexer.LineNumber,
                        this.lexer.Position,
                        token.Content));
            }
        }
        
        private void ParseData(Statment statment)
        {
            do
            {
                if (this.lexer.NextToken() is CommaToken)
                {
                    this.lexer.ConsumeTokenUsingStrategy(this.consumeStrategy);
                }

                var token = this.lexer.ConsumeTokenUsingStrategy(this.consumeStrategy);

                if (token is HexToken)
                {
                    statment.Dat.Add(Convert.ToUInt16(token.Content, 16));
                }
                else if (token is DecimalToken)
                {
                    statment.Dat.Add(Convert.ToUInt16(token.Content, 10));
                }
                else if (token is StringToken)
                {
                    foreach (var t in token.Content)
                    {
                        if (t == ' ')
                        {
                            continue;
                        }

                        statment.Dat.Add(t);
                    }
                }
            }
            while (this.lexer.NextToken() is CommaToken);
        }
    }
}
