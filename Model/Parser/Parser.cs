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

    using Model;
    using Model.Lexer;
    using Model.Lexer.Tokens;
    using Model.Operands;

    public class Parser : IParser
    {
        private readonly IConsumeTokenStrategy consumeStrategy = new ConsumeTokenStrategy(new IgnoreWhiteSpaceTokenStrategy());

        private readonly IOperandFactory directOperandFactory;
        private readonly IOperandFactory indirectOperandFactory;

        private readonly ILexer lexer;

        private readonly IList<Statment> statments;

        private Statment currentStatment;

        public Parser(ILexer lexer, IOperandFactory directOperandFactory, IOperandFactory indirectOperandFactory)
        {
            this.lexer = lexer;
            this.indirectOperandFactory = indirectOperandFactory;
            this.directOperandFactory = directOperandFactory;

            lexer.ConsumeTokenStrategy = new PeekTokenStrategy(new IgnoreWhiteSpaceTokenStrategy());

            this.statments = new List<Statment>();
        }

        public ICollection<Statment> Parse()
        {
            while (this.ParseStatment())
            {
            }

            return this.statments;
        }

        private bool ParseStatment()
        {
            this.DiscardEmptyLines();

            if (this.PeekNextToken() == null)
            {
                return false;
            }

            this.currentStatment = new Statment();

            this.ParseLabel();
            this.ParseMenemonic();
            this.ParseInstruction();
            this.ParseComments();

            this.statments.Add(this.currentStatment);

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

            this.currentStatment.Opcode = ((InstructionToken)token).Opcode;
        }

        private void ParseInstruction()
        {
            if (this.currentStatment.Opcode == BasicOpcode.OpDat)
            {
                this.ParseData();
            }
            else
            {
                this.ParseOperands();
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
            var token = this.ConsumeNextToken().Clone();

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

        private void AddDatToCurrentStatment(TokenBase token)
        {
            if (token is INumericToken)
            {
                this.currentStatment.Dat.Add(((INumericToken)token).NumericValue);
            }
            else if (token is StringToken)
            {
                foreach (var t in token.Content.Where(t => t != ' '))
                {
                    this.currentStatment.Dat.Add(t);
                }
            }
        }

        private void ParseComments()
        {
            var token = this.PeekNextToken();

            if (token is CommentToken)
            {
                this.ConsumeNextToken();
            }
        }

        private TokenBase PeekNextToken()
        {
            return this.lexer.NextToken();
        }

        private TokenBase ConsumeNextToken()
        {
            return this.lexer.NextTokenUsingStrategy(this.consumeStrategy);
        }
    }
}
