﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.IO;

namespace MonkArena {
    public static class Logger {
        static StreamWriter output;

        static Logger() {
            AllocConsole();
            IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            Encoding encoding = Encoding.GetEncoding(437);
            output = new StreamWriter(fileStream, encoding);
            output.AutoFlush = true;
        }

        public static void Log(object message, string prefix = "INFO") {
            output.WriteLine($"[{prefix}][{DateTime.UtcNow}] {message}");
        }
        public static void LogError(object message) {
            Log(message, "ERROR");
        }
        public static void LogInfo(object message) {
            Log(message, "INFO");
        }

        #region Interop
        [DllImport(
            "kernel32.dll",
            EntryPoint = "GetStdHandle",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport(
            "kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();
        private const int STD_OUTPUT_HANDLE = -11;
        #endregion
    }
}
