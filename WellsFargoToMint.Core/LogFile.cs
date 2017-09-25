using System;

namespace MPT.WellsFargoToMint.Core
{
    public class LogFile : ILogFile
    {
        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Debug(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Exception(string exceptionMessage)
        {
            throw new NotImplementedException();
        }

        public void Message(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Open(string pathToLogFile = "")
        {
            throw new NotImplementedException();
        }

        public void Trace(string message)
        {
            throw new NotImplementedException();
        }
    }
}
