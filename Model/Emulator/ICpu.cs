namespace Model.Emulator
{
    using System.Collections.Generic;

    public interface ICpu : ICpuStateOperations, ICpuNotifications
    {
        void LoadData(IEnumerable<ushort> data);

        bool ExecuteNextInstruction();

        void Reset();
    }
}