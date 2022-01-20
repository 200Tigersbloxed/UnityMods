using System;
using System.Diagnostics;
using MelonLoader;

namespace LabratUIKit
{
    public class LogHelper
    {
        private static readonly MelonLogger.Instance _logger = new MelonLogger.Instance("LabratUIKit");

        public static void Debug(object Message, bool ShouldStackFrame = true)
        {
            string msg = String.Empty;
            if (ShouldStackFrame)
            {
                StackFrame sf = new StackFrame(1);
                msg = sf.GetMethod().DeclaringType + "/" + sf.GetMethod() + " ";
            }
            msg = msg + Message;
            _logger.Msg("(DEBUG) " + msg);
        }

        public static void Log(object Message, bool ShouldStackFrame = false)
        {
            string msg = String.Empty;
            if (ShouldStackFrame)
            {
                StackFrame sf = new StackFrame(1);
                msg = sf.GetMethod().DeclaringType + "/" + sf.GetMethod() + " ";
            }
            msg = msg + Message;
            _logger.Msg(msg);
        }

        public static void Warn(object Message, bool ShouldStackFrame = false)
        {
            string msg = String.Empty;
            if (ShouldStackFrame)
            {
                StackFrame sf = new StackFrame(1);
                msg = sf.GetMethod().DeclaringType + "/" + sf.GetMethod() + " ";
            }
            msg = msg + Message;
            _logger.Warning(msg);
        }

        public static void Error(object Message, Exception e = null, bool ShouldStackFrame = true)
        {
            string msg = String.Empty;
            if (ShouldStackFrame)
            {
                StackFrame sf = new StackFrame(1);
                msg = sf.GetMethod().DeclaringType + "/" + sf.GetMethod() + " ";
            }
            msg = msg + Message;
            if (e != null)
                msg = msg + " | Exception: " + e;
            _logger.Error(msg);
        }
    }
}