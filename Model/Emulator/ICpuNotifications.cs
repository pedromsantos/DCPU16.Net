namespace Model.Emulator
{
    using System;

    public interface ICpuNotifications
    {
        event Action<ushort, Instruction> InstructionWillExecute;

        event Action<ushort, Instruction> InstructionDidExecute;

        event Action<int, ushort> RegisterWillChange;

        event Action<int, ushort> RegisterDidChange;

        event Action<int, ushort> ProgramCounterWillChange;

        event Action<int, ushort> ProgramCounterDidChange;

        event Action<int, ushort> StackPointerWillChange;

        event Action<int, ushort> StackPointerDidChange;

        event Action<int, ushort> OverflowWillChange;

        event Action<int, ushort> OverflowDidChange;
    }
}