﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.IO;
using UnityEngine;

namespace MonkArena {
    public static class RWConsole {
        public static StreamWriter Output;
        static FileStream stream;
        static object lastMessage;
        static string backspaceOfLastMessage;

        static int countOfLastMessage;

        public static void Initialize() {
            AllocConsole();
            IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            stream = new FileStream(stdHandle, FileAccess.Write);
            Output = new StreamWriter(stream, Encoding.ASCII) {
                AutoFlush = true,
            };

            Application.RegisterLogCallback(new Application.LogCallback(LogUnityError));
        }

        public static string ReadLine(bool intercept = false) {
            using(StreamReader reader = new StreamReader(stream)) {
                return reader.ReadLine();
            }
        }

        private static void LogUnityError(string condition, string stackTrace, LogType type) {
            Log($"{condition}\n\t{stackTrace}", type.ToString());
        }
        public static void Log(object message, string prefix = "INFO") {
            if (message == lastMessage) countOfLastMessage++;
            else countOfLastMessage = 1;

            string toConsole = $"{countOfLastMessage}x[{prefix}][{DateTime.UtcNow}] {message}";
            backspaceOfLastMessage = new string('\b', toConsole.Length);

            if (countOfLastMessage > 1) Output.WriteLine(backspaceOfLastMessage);
            Output.WriteLine(toConsole);
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
