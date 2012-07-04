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

namespace DCPU16Assembler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Assembler;

    using Lexer;
    using Lexer.Tokens;

    using Parser;

    public class Program
    {
        private static readonly IList<TokenBase> Matchers = new List<TokenBase>
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

        public static void Main(string[] args)
        {
            string code;

            if (args.Count() != 2)
            {
                Usage();
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("{0} does not exist.", args[0]);
                Usage();
                return;
            }

            using (var myFile = new StreamReader(args[0]))
            {
                code = myFile.ReadToEnd();
            }

            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, Matchers);
            var directOperandFactory = new DirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory);
            parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(parser.Statments);
            
            var data = new List<byte>();
            foreach (var word in program)
            {
                data.Add((byte)(word >> 8));
                data.Add((byte)(word & 0xFF));
            }

            File.Create(args[1]);
            File.WriteAllBytes(args[1], data.ToArray());
        }

        private static void Usage()
        {
            Console.WriteLine("DCPU16 Assembler usage:");
            Console.WriteLine("DCPU16Assembler sourceFile outputFile");
        }
    }
}
