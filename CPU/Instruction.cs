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
    public abstract class Instruction
    {
        protected Instruction(ushort rawInstruction, CpuOperation operationA, CpuOperation operationB)
        {
            this.RawInstruction = rawInstruction;
            this.OperationA = operationA;
            this.OperationB = operationB;
        }

        protected CpuOperation OperationA { get; private set; }

        protected CpuOperation OperationB { get; private set; }

        protected ushort RawInstruction { get; private set; }

        public virtual ushort Execute()
        {
            if (this.OperationA != null)
            {
                this.OperationA.Process();
            }

            if (this.OperationB != null)
            {
                this.OperationB.Process();
            }

            return (ushort)this.Process();
        }

		public virtual void NoOp()
		{
			this.OperationA.NoOp();
			this.OperationB.NoOp();
		}

        /*
        public virtual void Undo()
        {
        }
        */

        protected abstract int Process();
    }
}
