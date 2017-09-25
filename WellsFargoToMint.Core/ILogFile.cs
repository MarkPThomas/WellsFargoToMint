
namespace MPT.WellsFargoToMint.Core
{
    public interface ILogFile
    {
        void Open(string pathToLogFile = "");
        void Message(string message, params object[] args);
        void Debug(string message, params object[] args);
        void Trace(string message);
        void Exception(string exceptionMessage);
        void Close();
    }
}
