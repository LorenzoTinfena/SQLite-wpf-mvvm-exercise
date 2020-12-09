using System;

namespace SQLiteWpfMvvmExercise.ViewModels
{
    public static class Log
    {
        public enum LogType
        {
            Standard,
            Warning,
            Error
        }

        public static event Action<object, Log.LogType> LogHandler;

        #region public functions
        public static void Standard(object e) => Log.LogHandler(e, Log.LogType.Standard);
        public static void Warning(object e) => Log.LogHandler(e, Log.LogType.Warning);
        public static void Error(object e) => Log.LogHandler(e, Log.LogType.Error);
        #endregion
    }
}
