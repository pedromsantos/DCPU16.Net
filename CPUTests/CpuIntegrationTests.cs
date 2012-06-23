
namespace CPUTests
{
	using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Assembler;

    using CPU;
    using CPU.Instructions;

    using Lexer;
    using Lexer.Tokens;

    using Model;

    using Moq;

    using NUnit.Framework;

    using Parser;

	[TestFixture]
	public class CpuIntegrationTests
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
        public void CanStepThrougthModifiedNotchSample()
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
									:crash        SET A, 0            ; 7dc1 001a [*]";

            var reader = new StringReader(Code);
            var lexer = new PeekLexer(reader, this.matchers);
            var parser = new Parser(lexer);
            parser.Parse();
            var statments = parser.Statments;
            var assembler = new Assembler();
            var program = assembler.AssembleStatments(statments);

			var operandFactory = new InstructionOperandFactory();
            var cpu = new CentralProcessingUnit(operandFactory);
            cpu.LoadProgram(program);

			var executed = true;

			while(executed)
			{
				executed = cpu.ExecuteNextInstruction();
			}

			Assert.That(cpu.ReadGeneralPursoseRegisterValue((ushort)RegisterIdentifier.RegX), Is.EqualTo(0x40));
        }

	}
}

