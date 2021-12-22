using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Verse;

namespace VFEM_MoatPatch
{
    [StaticConstructorOnStartup]
    public static class Mod
    {
        public static string Author => "GonDragon";
        public static string Name => Assembly.GetName().Name;
        public static string Id => Author + "." + Name;

        public static string Version => Assembly.GetName().Version.ToString();

        private static Assembly Assembly
        {
            get
            {
                return Assembly.GetAssembly(typeof(Mod));
            }
        }

        public static readonly Harmony Harmony;

        static Mod()
        {
            Harmony = new Harmony(Id);
            Harmony.PatchAll();
        }

        public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));

        public static void Warning(string message) => Verse.Log.Warning(PrefixMessage(message));

        public static void Error(string message) => Verse.Log.Error(PrefixMessage(message));

        public static void ErrorOnce(string message, string key) => Verse.Log.ErrorOnce(PrefixMessage(message), key.GetHashCode());

        public static void Message(string message) => Messages.Message(message, MessageTypeDefOf.TaskCompletion, false);

        [Conditional("DEBUG")]
        public static void Debug(string message) => Warning(message);

        private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";
    }
}
