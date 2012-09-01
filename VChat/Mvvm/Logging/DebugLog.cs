using System;
using System.Diagnostics;
using Caliburn.Micro;

namespace VChat.Mvvm.Logging
{
    public class DebugLog : ILog
    {
        private readonly Type _type;

        public DebugLog(Type type)
        {
            _type = type;
        }

        #region ILog

        public void Info(string format, params object[] args)
        {
            Debug.WriteLine(string.Format("[INFO] {0} {1}", _type.FullName, string.Format(format, args)));
        }

        public void Warn(string format, params object[] args)
        {
            Debug.WriteLine(string.Format("[WARN] {0} {1}", _type.FullName, string.Format(format, args)));
        }

        public void Error(Exception exception)
        {
            Debug.WriteLine(string.Format("[ERROR] {0} {1}", _type.FullName, exception));
        }

        #endregion
    }
}