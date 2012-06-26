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

namespace CPU
{
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using Model;

    public class Emulator
    {
        private readonly CentralProcessingUnit cpu;

        private readonly InstructionOperandFactory operandFactory;

        public Emulator()
        {
            this.operandFactory = new InstructionOperandFactory();
            this.cpu = new CentralProcessingUnit(this.operandFactory);
        }

        public bool LoadProgram(string fileName)
        {
            if (File.Exists(fileName) != true)
            {
                return false;
            }

            var programData = File.ReadAllBytes(fileName);

            if ((programData.Length % 2) != 0)
            {
                return false;
            }

            this.LoadProgram(programData);

            return true;
        }

        public void LoadProgram(byte[] programData)
        {
            IList<ushort> program;

            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream(programData))
            {
                program = (List<ushort>)bf.Deserialize(ms);
            }

            this.cpu.LoadProgram(program);
        }
    }
}
