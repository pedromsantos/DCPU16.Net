namespace Model.Emulator
{
    using System.Collections.Generic;

    public interface ICpu : ICpuStateOperations, ICpuNotifications
    {
        void LoadProgram(IEnumerable<ushort> program);

        bool ExecuteNextInstruction();

        void Reset();
    }
}