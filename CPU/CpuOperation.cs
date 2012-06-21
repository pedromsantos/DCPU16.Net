
namespace CPU
{
    using Model;

    public class CpuOperation
    {
        private readonly Operand operand;

        private readonly ICentralProcessingUnitStateOperations cpuStateManager;

        public CpuOperation(Operand operand, ICentralProcessingUnitStateOperations cpuStateManager)
        {
            this.operand = operand;
            this.cpuStateManager = cpuStateManager;
        }

        public ushort Read
        {
            get
            {
                return this.operand.Read(this.cpuStateManager);
            }
        }

        public int Write
        {
            set
            {
                if (value > ushort.MaxValue)
                {
                    this.cpuStateManager.Overflow = 0x0001;
                }
                else if (value < 0)
                {
                    this.cpuStateManager.Overflow = ushort.MaxValue;
                }

                this.operand.Write(this.cpuStateManager, (ushort)value);
            }
        }

        public void Process()
        {
            this.operand.Process(this.cpuStateManager);
        }
    }
}
