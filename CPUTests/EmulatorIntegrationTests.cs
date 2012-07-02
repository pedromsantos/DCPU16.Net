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

namespace CPUTests
{
    using System.Collections.Generic;
    using System.IO;

    using Assembler;

    using CPU;

    using Lexer;
    using Lexer.Tokens;

    using NUnit.Framework;

    using Parser;

    [TestFixture]
    public class EmulatorIntegrationTests
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
        public void LoadProgramWhenCalledWhitValidProgramLoadsProgramInMemory()
        {
            const string Code =
                @"  ;Try some basic stuff
                                    SET A, 0x30              ; 7c01 0030
                                    SET [0x1000], 0x20       ; 7de1 1000 0020
                                    SUB A, [0x1000]          ; 7803 1000
                                    IFN A, 0x10              ; c00d
                                    SET PC, crash            ; 7dc1 001a [*]

                                    ; Do a loopy thing
                                    SET I, 10                ; a861
                                    SET A, 0x2000            ; 7c01 2000
                                    :loop         SET [0x2000+I], [A]      ; 2161 2000
                                    SUB I, 1                 ; 8463
                                    IFN I, 0                 ; 806d
                                    SET PC, loop             ; 7dc1 000d [*]

                                    ; Call a subroutine
                                    SET X, 0x4               ; 9031
                                    JSR testsub              ; 7c10 0018 [*]
                                    SET PC, crash            ; 7dc1 001a [*]

                                    :testsub      SHL X, 4   ; 9037
                                    SET PC, POP              ; 61c1

                                    ; Hang forever. X should now be 0x40 if everything went right.
									:crash        SET PC, crash            ; 7dc1 001a [*]";

            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser(lexer);
            parser.Parse();
            var statments = parser.Statments;
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

            var data = new List<byte>();
            foreach (var word in program)
            {
                data.Add((byte)(word >> 8));
                data.Add((byte)(word & 0xFF));
            }

            var emulator = new Emulator();
            var receivedEvents = new Dictionary<int, ushort>();
            emulator.MemoryDidChange += receivedEvents.Add; 
            emulator.LoadProgram(data.ToArray());

            var expectedInstruction = new[]
                {
                    0x7c01, 0x0030, 0x7de1, 0x1000, 0x0020, 0x7803, 0x1000, 0xc00d, 
                    0x7dc1, 0x001a, 0xa861, 0x7c01, 0x2000, 0x2161, 0x2000, 0x8463,
                    0x806d, 0x7dc1, 0x000d, 0x9031, 0x7c10, 0x0018, 0x7dc1, 0x001a, 
                    0x9037, 0x61c1, 0x7dc1, 0x001a
                };

            for (var i = 0; i < 28; i++)
            {
                Assert.That(receivedEvents[i], Is.EqualTo(expectedInstruction[i]));
            }
        }
    }
}
