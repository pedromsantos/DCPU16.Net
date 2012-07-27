namespace Model.Emulator
{
    public interface IEmulator : ICpuNotifications
    {
        bool LoadProgram(string fileName);

        void LoadProgram(byte[] data);

        void RunLoadedProgram();

        void RunLoadedProgramWithDelay(int intervalBetweenInstructions);
    }
}