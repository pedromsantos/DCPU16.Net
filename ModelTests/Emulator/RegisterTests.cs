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

namespace ModelTests.Emulator
{
    using Model.Emulator;

    using NUnit.Framework;

    [TestFixture]
    public class RegisterTests
    {
        [Test]
        public void WriteGeneralPursoseRegisterValueWhenCalledFiresRegisterWillChange()
        {
            var receivedEvents = new System.Collections.Generic.Dictionary<int, ushort>();

            var registers = new Registers();
            registers.RegisterWillChange += receivedEvents.Add;
            registers.WriteGeneralPursoseRegisterValue(0, 10);

            Assert.That(receivedEvents[0], Is.EqualTo(10));
        }

        [Test]
        public void WriteGeneralPursoseRegisterValueWhenCalledFiresRegisterDidChange()
        {
            var receivedEvents = new System.Collections.Generic.Dictionary<int, ushort>();

            var registers = new Registers();
            registers.RegisterDidChange += receivedEvents.Add;
            registers.WriteGeneralPursoseRegisterValue(0, 10);

            Assert.That(receivedEvents[0], Is.EqualTo(10));
        }

        [Test]
        public void OverflowWhenSetFiresOverflowWillChange()
        {
            var receivedEvents = new System.Collections.Generic.Dictionary<int, ushort>();

            var registers = new Registers();
            registers.OverflowWillChange += receivedEvents.Add;
            registers.Overflow = 10;

            Assert.That(receivedEvents[0], Is.EqualTo(10));
        }

        [Test]
        public void OverflowWhenSetFiresOverflowDidChange()
        {
            var receivedEvents = new System.Collections.Generic.Dictionary<int, ushort>();

            var registers = new Registers();
            registers.OverflowDidChange += receivedEvents.Add;
            registers.Overflow = 10;

            Assert.That(receivedEvents[0], Is.EqualTo(10));
        }

        [Test]
        public void ProgramCounterwhenSetFiresProgramCounterWillChange()
        {
            var receivedEvents = new System.Collections.Generic.Dictionary<int, ushort>();

            var registers = new Registers();
            registers.ProgramCounterWillChange += receivedEvents.Add;
            registers.ProgramCounter = 10;

            Assert.That(receivedEvents[0], Is.EqualTo(10));
        }

        [Test]
        public void ProgramCounterWhenSetFiresProgramCounterDidChange()
        {
            var receivedEvents = new System.Collections.Generic.Dictionary<int, ushort>();

            var registers = new Registers();
            registers.ProgramCounterDidChange += receivedEvents.Add;
            registers.ProgramCounter = 10;

            Assert.That(receivedEvents[0], Is.EqualTo(10));
        }

        [Test]
        public void StackPointerWhenSetFiresStackPointerWillChange()
        {
            var receivedEvents = new System.Collections.Generic.Dictionary<int, ushort>();

            var registers = new Registers();
            registers.StackPointerWillChange += receivedEvents.Add;
            registers.StackPointer = 10;

            Assert.That(receivedEvents[0], Is.EqualTo(10));
        }

        [Test]
        public void StackPointerWhenSetFiresStackPointerDidChange()
        {
            var receivedEvents = new System.Collections.Generic.Dictionary<int, ushort>();

            var registers = new Registers();
            registers.StackPointerDidChange += receivedEvents.Add;
            registers.StackPointer = 10;

            Assert.That(receivedEvents[0], Is.EqualTo(10));
        }
    }
}
