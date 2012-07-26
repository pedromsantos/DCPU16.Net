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

namespace Model.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Lexer.Tokens;

    using Model;
    using Model.Lexer;
    using Model.Parser.Operands;

    public class Parser : IParser
    {
        private readonly IConsumeTokenStrategy consumeStrategy = new ConsumeTokenStrategy();

        private readonly IOperandFactory directOperandFactory;
        private readonly IOperandFactory indirectOperandFactory;

        private readonly ILexer lexer;

        private Statment currentStatment;

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
            {
            }

            return this.Statments;
        }

        public bool ParseStatment()
        {
            this.DiscardEmptyLines();

            if (this.PeekNextToken() == null)
            {
                return false;
            }

            this.currentStatment = new Statment();

            this.ParseLabel();
            this.ParseMenemonic();
            
            if (this.currentStatment.Menemonic == "DAT")
            {
                this.ParseData();
            }
            else
            {
                this.ParseOperands();
            }

            this.Statments.Add(this.currentStatment);
            this.ParseComments();

            return true;
        }

        private void DiscardEmptyLines()
        {
            TokenBase token;
            do
            {
                token = this.PeekNextToken();

                if (token is CommentToken || token is WhiteSpaceToken)
                {
                    token = this.ConsumeNextToken();
                }
                else
                {
                    return;
                }
            }
            while (token is CommentToken || token is WhiteSpaceToken);
        }

        private void ParseLabel()
        {
            var token = this.PeekNextToken();

            if (token is LabelToken)
            {
                this.ConsumeNextToken();
                this.currentStatment.Label = token.Content;
            }
        }

        private void ParseMenemonic()
        {
            var token = this.ConsumeNextToken();

            if (!(token is InstructionToken))
            {
                throw new Exception(
                    string.Format(
                        "Expected INSTRUCTION at line {0}:{1} found '{2}'",
                        this.lexer.LineNumber,
                        this.lexer.Position, 
                    token.Content));
            }

            this.currentStatment.Menemonic = token.Content.ToUpper();
        }

        private void ParseComments()
        {
            var token = this.PeekNextToken();

            if (token is CommentToken)
            {
                this.ConsumeNextToken();
            }
        }

        private void ParseOperands()
        {
            this.currentStatment.OperandA = this.ParseOperand();

            var token = this.ConsumeNextToken();

            if (token is CommaToken)
            {
                this.currentStatment.OperandB = this.ParseOperand();
            }
        }

        private Operand ParseOperand()
        {
            var token = this.ConsumeNextToken();

            if (token is OpenBracketToken)
            {
                return this.ParseIndirectOperand();
            }

            return this.directOperandFactory.CreateOperand(token);
        }

        private Operand ParseIndirectOperand()
        {
            var token = this.ConsumeNextToken();

            Operand operand;

            if (this.PeekNextToken() is PluslToken)
            {
                this.ConsumeNextToken();
                var rigthToken = this.ConsumeNextToken();
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
            var token = this.ConsumeNextToken();

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

        private void ParseData()
        {
            do
            {
                if (this.PeekNextToken() is CommaToken)
                {
                    this.ConsumeNextToken();
                }

                var token = this.ConsumeNextToken();

                this.AddDatToCurrentStatment(token);
            }
            while (this.PeekNextToken() is CommaToken);
        }

        private void AddDatToCurrentStatment(TokenBase token)
        {
            if (token is HexToken)
            {
                this.currentStatment.Dat.Add(Convert.ToUInt16(token.Content, 16));
            }
            else if (token is DecimalToken)
            {
                this.currentStatment.Dat.Add(Convert.ToUInt16(token.Content, 10));
            }
            else if (token is StringToken)
            {
                foreach (var t in token.Content.Where(t => t != ' '))
                {
                    this.currentStatment.Dat.Add(t);
                }
            }
        }

        private TokenBase PeekNextToken()
        {
            return this.lexer.NextToken();
        }

        private TokenBase ConsumeNextToken()
        {
            return this.lexer.ConsumeTokenUsingStrategy(this.consumeStrategy);
        }
    }
}
