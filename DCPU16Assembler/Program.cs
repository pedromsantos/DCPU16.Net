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

    using Assembler;

    using CPU;

    using Lexer;
    using Lexer.Tokens;

    using Microsoft.GotDotNet;

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

        private static readonly IDictionary<int, ushort> LoadedInstructions = new Dictionary<int, ushort>();
        private static readonly IDictionary<int, ushort> MemoryChanged = new Dictionary<int, ushort>();
        private static readonly IList<KeyValuePair<ushort, Instruction>> ExecutedInstructions = new List<KeyValuePair<ushort, Instruction>>();

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Usage();
                return;
            }

            if (!File.Exists(args[1]))
            {
                Console.WriteLine("{0} does not exist.", args[1]);
                Usage();
                return;
            }

            if (args[0] == "-c")
            {
                AssembleFile(args[1], args[2]);
            }
            else if (args[0] == "-r")
            {
                ConsoleEx.TextColor(ConsoleForeground.White, ConsoleBackground.Blue);
                ConsoleEx.DrawRectangle(BorderStyle.None, 0, 0, 80, 25, true);

                DisplayRegisters();
                DisplayStack();
                DisplayExecutedInstructions();
                DisplayMemory();

                RunProgram(args[1]);
            }
        }

        private static void RunProgram(string inputFileName)
        {
            var programData = File.ReadAllBytes(inputFileName);

            if ((programData.Length % 2) != 0)
            {
                return;
            }

            var emulator = new Emulator();
            emulator.InstructionDidLoad += LoadedInstructions.Add;
            emulator.LoadProgram(programData);

            emulator.RegisterDidChange += RegisterDidChange;
            emulator.ProgramCounterDidChange += ProgramCounterDidChange;
            emulator.StackPointerDidChange += StackPointerDidChange;
            emulator.OverflowDidChange += OverflowDidChange;
            emulator.InstructionWillExecute += (rawInstruction, instruction) => ExecutedInstructions.Add(new KeyValuePair<ushort, Instruction>(rawInstruction, instruction));
            emulator.InstructionDidExecute += InstructionDidExecute;
            emulator.MemoryWillChange += MemoryChanged.Add;
            emulator.MemoryDidChange += MemoryDidChange;
            emulator.RunLoadedProgramWithDelay(100);
        }

        private static void AssembleFile(string inputFileName, string outputFileName)
        {
            string code;
            using (var myFile = new StreamReader(inputFileName))
            {
                code = myFile.ReadToEnd();
            }

            var reader = new StringReader(code);
            var lexer = new PeekLexer(reader, Matchers);
            var directOperandFactory = new DirectOperandFactory();
            var indirectOperandFactory = new IndirectOperandFactory();
            var parser = new Parser(lexer, directOperandFactory, indirectOperandFactory);

            parser.Parse();
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(parser.Statments);
            
            var data = new List<byte>();
            foreach (var word in program)
            {
                data.Add((byte)(word >> 8));
                data.Add((byte)(word & 0xFF));
            }

            File.WriteAllBytes(outputFileName, data.ToArray());
        }

        private static void Usage()
        {
            Console.WriteLine("DCPU16 Assembler usage:");
            Console.WriteLine("DCPU16Assembler -c sourceFile outputFile");
            Console.WriteLine("DCPU16Assembler -r programFile");
            Console.WriteLine();
            Console.WriteLine("optional --verbose");
        }

        private static void MemoryDidChange(int address, ushort value)
        {
            if (MemoryChanged.Count > 32)
            {
                MemoryChanged.Clear();
                MemoryChanged.Add(address, value);
                DisplayMemory();
            }

            var column = 0;
            var line = 0;
            foreach (var t in MemoryChanged)
            {
                ConsoleEx.WriteAt(16 + (column * 16), 17 + line, string.Format("[0x{0:X4}]=0x{1:X4}", t.Key, t.Value));
                column++;

                if (column <= 3)
                {
                    continue;
                }
                column = 0;
                line++;
            }
        }

        private static void RegisterDidChange(int register, ushort value)
        {
            ConsoleEx.WriteAt(6, 3 + register, string.Format("0x{0:X4}", value));
        }

        private static void ProgramCounterDidChange(int register, ushort value)
        {
            ConsoleEx.WriteAt(6, 12, string.Format("0x{0:X4}", value));
        }

        private static void StackPointerDidChange(int register, ushort value)
        {
            ConsoleEx.WriteAt(6, 13, string.Format("0x{0:X4}", value));
        }

        private static void OverflowDidChange(int register, ushort value)
        {
            ConsoleEx.WriteAt(6, 11, string.Format("0x{0:X4}", value));
        }

        private static void InstructionDidExecute(ushort rawInstruction, Instruction instruction)
        {
            if (ExecutedInstructions.Count > 11)
            {
                ExecutedInstructions.Clear();
                ExecutedInstructions.Add(new KeyValuePair<ushort, Instruction>(rawInstruction, instruction));
                DisplayExecutedInstructions();
            }

            int line = ExecutedInstructions.Count + 1;

            ConsoleEx.WriteAt(17, line, string.Format("0x{0:X4}", rawInstruction));
            ConsoleEx.WriteAt(24, line, instruction.ToString());
        }

        private static void DisplayRegisters()
        {
            ConsoleEx.TextColor(ConsoleForeground.White, ConsoleBackground.Blue);
            ConsoleEx.DrawRectangle(BorderStyle.LineSingle, 0, 0, 14, 14, true);
            ConsoleEx.CursorVisible = false;

            ConsoleEx.WriteAt(2, 0, " REGISTERS ");

            ConsoleEx.WriteAt(3, 2, string.Format("A: 0x{0:X4}", 0));
            ConsoleEx.WriteAt(3, 3, string.Format("B: 0x{0:X4}", 0));
            ConsoleEx.WriteAt(3, 4, string.Format("C: 0x{0:X4}", 0));
            ConsoleEx.WriteAt(3, 5, string.Format("X: 0x{0:X4}", 0));
            ConsoleEx.WriteAt(3, 6, string.Format("Y: 0x{0:X4}", 0));
            ConsoleEx.WriteAt(3, 7, string.Format("Z: 0x{0:X4}", 0));
            ConsoleEx.WriteAt(3, 8, string.Format("I: 0x{0:X4}", 0));
            ConsoleEx.WriteAt(3, 9, string.Format("J: 0x{0:X4}", 0));
            ConsoleEx.WriteAt(3, 10, string.Format("O: 0x{0:X4}", 0));
            ConsoleEx.WriteAt(2, 11, string.Format("PC: 0x{0:X4}", 0));
            ConsoleEx.WriteAt(2, 12, string.Format("SP: 0x{0:X4}", 0));
        }

        private static void DisplayExecutedInstructions()
        {
            ConsoleEx.TextColor(ConsoleForeground.White, ConsoleBackground.Blue);
            ConsoleEx.DrawRectangle(BorderStyle.LineSingle, 15, 0, 64, 14, true);
            ConsoleEx.CursorVisible = false;

            ConsoleEx.WriteAt(44, 0, " EXECUTED ");

            for (var i = 2; i < 14; i++)
            {
                ConsoleEx.WriteAt(17, i, string.Format("{0,62}", ' '));
            }
        }

        private static void DisplayMemory()
        {
            ConsoleEx.TextColor(ConsoleForeground.White, ConsoleBackground.Blue);
            ConsoleEx.DrawRectangle(BorderStyle.LineSingle, 15, 15, 64, 9, true);
            ConsoleEx.CursorVisible = false;

            ConsoleEx.WriteAt(46, 15, " MEMORY ");

            for (var i = 17; i < 20; i++)
            {
                ConsoleEx.WriteAt(16, i, string.Format("{0,63}", ' '));
            }
        }

        private static void DisplayStack()
        {
            ConsoleEx.TextColor(ConsoleForeground.White, ConsoleBackground.Blue);
            ConsoleEx.DrawRectangle(BorderStyle.LineSingle, 0, 15, 14, 9, true);
            ConsoleEx.CursorVisible = false;

            ConsoleEx.WriteAt(4, 15, " STACK ");

            for (var i = 0; i < 6; i++)
            {
                ConsoleEx.WriteAt(4, i + 17, string.Format("0x{0:X4}", 0));
            }
        }
    }
}
