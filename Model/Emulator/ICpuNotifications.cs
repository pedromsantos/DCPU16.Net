namespace Model.Emulator
{
    public interface ICpuNotifications
    {
        event InstructionExecutionHandler InstructionWillExecute;

        event InstructionExecutionHandler InstructionDidExecute;

        event MemoryChangeHandler InstructionDidLoad;

        event MemoryChangeHandler MemoryWillChange;

        event MemoryChangeHandler MemoryDidChange;

        event RegisterChangeHandler RegisterWillChange;

        event RegisterChangeHandler RegisterDidChange;

        event RegisterChangeHandler ProgramCounterWillChange;

        event RegisterChangeHandler ProgramCounterDidChange;

        event RegisterChangeHandler StackPointerWillChange;

        event RegisterChangeHandler StackPointerDidChange;

        event RegisterChangeHandler OverflowWillChange;

        event RegisterChangeHandler OverflowDidChange;

        event MemoryChangeHandler VideoMemoryDidChange;

        event MemoryChangeHandler KeyboardMemoryDidChange;
    }
}