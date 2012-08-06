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

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using Microsoft.GotDotNet;

    using Model;
    using Model.Assembler;
    using Model.Emulator;
    using Model.Lexer;
    using Model.Lexer.Tokens;
    using Model.Parser;

    public class Program
    {
        private const int OverflowLine = 10;
        private const int StackPointerLine = 12;
        private const int ProgramCounterLine = 11;
        private const int GeneralRegisterStarterLine = 2;
        private const int RegisterValueColumn = 6;
        private const int GeneralRegisterTitleColumn = 3;
        private const int OverflowTitleColumn = 3;
        private const int StackPointerTitleColumn = 2;
        private const int ProgramCounterTitleColumn = 2;

        private const string RegisterFormatString = "{0}: 0x{1:X4}";
        private const string HexFormatString = "0x{0:X4}";

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

        /*
        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Usage();
                return;
            }

            if (!File.Exists(args.Last()))
            {
                Console.WriteLine("{0} does not exist.", args[1]);
                Usage();
                return;
            }

            if (args.First() == "-c")
            {
                AssembleFile(args[1], args.Last());
            }
            else if (args.First() == "-r")
            {
                ConsoleEx.TextColor(ConsoleForeground.White, ConsoleBackground.Blue);
                ConsoleEx.DrawRectangle(BorderStyle.None, 0, 0, 80, 25, true);
                ConsoleEx.CursorVisible = false;

                DisplayRegisters();
                DisplayStack();
                DisplayMemory();

                switch (args[1])
                {
                    case "-i":
                        DisplayExecutedInstructions();
                        RunProgram(args.Last(), args[1]);
                        break;
                    default:
                        DisplayOutput();
                        RunProgram(args.Last());
                        return;
                }
            }
        }
        */

        public static void Main(string[] args)
        {
            ConsoleEx.TextColor(ConsoleForeground.White, ConsoleBackground.Blue);
            ConsoleEx.DrawRectangle(BorderStyle.None, 0, 0, 80, 25, true);
            ConsoleEx.CursorVisible = false;

            DisplayRegisters();
            DisplayStack();
            DisplayMemory();
            DisplayOutput();
            RunProgram("text_write_test.bin");
        }

        private static void RunProgram(string inputFileName, string displayOption = "")
        {
            var container = new WindsorContainer();

            container.Register(
                Component.For<IInstructionOperandFactory>().ImplementedBy<InstructionOperandFactory>(),
                Component.For<IInstructionBuilder>().ImplementedBy<InstructionBuilder>(),
                Component.For<ICpu>().ImplementedBy<Cpu>(),
                Component.For<IEmulator>().ImplementedBy<Emulator>());

            var programData = File.ReadAllBytes(inputFileName);

            if ((programData.Length % 2) != 0)
            {
                return;
            }
            
            var emulator = container.Resolve<IEmulator>();
            emulator.InstructionDidLoad += LoadedInstructions.Add;
            emulator.LoadProgram(programData);

            emulator.RegisterDidChange += RegisterDidChange;
            emulator.ProgramCounterDidChange += ProgramCounterDidChange;
            emulator.StackPointerDidChange += StackPointerDidChange;
            emulator.OverflowDidChange += OverflowDidChange;
            emulator.InstructionWillExecute += (rawInstruction, instruction) => ExecutedInstructions.Add(new KeyValuePair<ushort, Instruction>(rawInstruction, instruction));

            switch (displayOption)
            {
                case "-i":
                    emulator.InstructionDidExecute += InstructionDidExecute;
                    break;
                default:
                    emulator.VideoMemoryDidChange += VideoMemoryDidChange;
                    break;
            }

            emulator.MemoryWillChange += MemoryChanged.Add;
            emulator.MemoryDidChange += MemoryDidChange;
            emulator.RunLoadedProgramWithDelay(25);
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
            Console.WriteLine("DCPU16Assembler -r [-i] 'display instructions instead of program output' programFile");
            Console.WriteLine();
            Console.WriteLine("optional --verbose");
        }

        private static void VideoMemoryDidChange(int address, ushort value)
        {
            const int Vidwidth = 32;
            const int Vidmem = 0x8000;

            var prevBackColor = Console.BackgroundColor;
            var prevForeColor = Console.ForegroundColor;

            /*
            var color = (value >> 8) & 0x0f;
            Console.BackgroundColor = Enum.IsDefined(typeof(ConsoleColor), color)
                                          ? (ConsoleColor)color
                                          : ConsoleColor.Blue;
            color = value >> 12;
            Console.ForegroundColor = Enum.IsDefined(typeof(ConsoleColor), color)
                                          ? (ConsoleColor)color
                                          : ConsoleColor.White;
            */

            var celladdress = address - Vidmem;
            var column = (celladdress % Vidwidth) + 21;
            var line = (celladdress / Vidwidth) + 1;
            var character = ((char)(value & 0x7f)).ToString();
            ConsoleEx.WriteAt(column, line, character);

            Console.BackgroundColor = prevBackColor;
            Console.ForegroundColor = prevForeColor;
        }

        private static void MemoryDidChange(int address, ushort value)
        {
            const int MaxMemoryAddressesToDisplay = 32;
            if (MemoryChanged.Count > MaxMemoryAddressesToDisplay)
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
            ConsoleEx.WriteAt(RegisterValueColumn, GeneralRegisterStarterLine + register, string.Format("0x{0:X4}", value));
        }

        private static void ProgramCounterDidChange(int register, ushort value)
        {
            ConsoleEx.WriteAt(RegisterValueColumn, ProgramCounterLine, string.Format(HexFormatString, value));
        }

        private static void StackPointerDidChange(int register, ushort value)
        {
            ConsoleEx.WriteAt(RegisterValueColumn, StackPointerLine, string.Format(HexFormatString, value));
        }

        private static void OverflowDidChange(int register, ushort value)
        {
            ConsoleEx.WriteAt(RegisterValueColumn, OverflowLine, string.Format(HexFormatString, value));
        }

        private static void InstructionDidExecute(ushort rawInstruction, Instruction instruction)
        {
            const int MaxInstructionsToDisplay = 11;
            if (ExecutedInstructions.Count > MaxInstructionsToDisplay)
            {
                ExecutedInstructions.Clear();
                ExecutedInstructions.Add(new KeyValuePair<ushort, Instruction>(rawInstruction, instruction));
                DisplayExecutedInstructions();
            }

            int line = ExecutedInstructions.Count + 1;

            ConsoleEx.WriteAt(17, line, string.Format(HexFormatString, rawInstruction));
            ConsoleEx.WriteAt(24, line, instruction.ToString());
        }

        private static void DisplayRegisters()
        {
            ConsoleEx.DrawRectangle(BorderStyle.LineSingle, 0, 0, 14, 14, true);
            ConsoleEx.WriteAt(2, 0, " REGISTERS ");

            ConsoleEx.WriteAt(
                GeneralRegisterTitleColumn, GeneralRegisterStarterLine, string.Format(RegisterFormatString, "A", 0));
            ConsoleEx.WriteAt(GeneralRegisterTitleColumn, GeneralRegisterStarterLine + 1, string.Format(RegisterFormatString, "B", 0));
            ConsoleEx.WriteAt(GeneralRegisterTitleColumn, GeneralRegisterStarterLine + 2, string.Format(RegisterFormatString, "C", 0));
            ConsoleEx.WriteAt(GeneralRegisterTitleColumn, GeneralRegisterStarterLine + 3, string.Format(RegisterFormatString, "X", 0));
            ConsoleEx.WriteAt(GeneralRegisterTitleColumn, GeneralRegisterStarterLine + 4, string.Format(RegisterFormatString, "Y", 0));
            ConsoleEx.WriteAt(GeneralRegisterTitleColumn, GeneralRegisterStarterLine + 5, string.Format(RegisterFormatString, "Z", 0));
            ConsoleEx.WriteAt(GeneralRegisterTitleColumn, GeneralRegisterStarterLine + 6, string.Format(RegisterFormatString, "I", 0));
            ConsoleEx.WriteAt(GeneralRegisterTitleColumn, GeneralRegisterStarterLine + 7, string.Format(RegisterFormatString, "J", 0));
            ConsoleEx.WriteAt(OverflowTitleColumn, OverflowLine, string.Format(RegisterFormatString, "O", 0));
            ConsoleEx.WriteAt(ProgramCounterTitleColumn, ProgramCounterLine, string.Format(RegisterFormatString, "PC", 0));
            ConsoleEx.WriteAt(StackPointerTitleColumn, StackPointerLine, string.Format(RegisterFormatString, "SP", 0));
        }

        private static void DisplayExecutedInstructions()
        {
            ConsoleEx.DrawRectangle(BorderStyle.LineSingle, 15, 0, 64, 14, true);
            ConsoleEx.WriteAt(44, 0, " EXECUTED ");

            for (var i = 2; i < 14; i++)
            {
                ConsoleEx.WriteAt(17, i, string.Format("{0,62}", ' '));
            }
        }

        private static void DisplayMemory()
        {
            ConsoleEx.DrawRectangle(BorderStyle.LineSingle, 15, 15, 64, 9, true);
            ConsoleEx.WriteAt(46, 15, " MEMORY ");

            for (var i = 17; i < 20; i++)
            {
                ConsoleEx.WriteAt(16, i, string.Format("{0,63}", ' '));
            }
        }

        private static void DisplayStack()
        {
            ConsoleEx.DrawRectangle(BorderStyle.LineSingle, 0, 15, 14, 9, true);
            ConsoleEx.WriteAt(4, 15, " STACK ");

            for (var i = 0; i < 6; i++)
            {
                ConsoleEx.WriteAt(4, i + 17, string.Format(HexFormatString, 0));
            }
        }

        private static void DisplayOutput()
        {
            ConsoleEx.DrawRectangle(BorderStyle.LineSingle, 20, 0, 20 + 33, 0 + 13, true);
            ConsoleEx.WriteAt(44, 0, " OUTPUT ");
        }
    }
}
