
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

        public ushort Write
        {
            set
            {
                this.operand.Write(this.cpuStateManager, value);
            }
        }

        public void Process()
        {
            this.operand.Process(this.cpuStateManager);
        }
    }
}
