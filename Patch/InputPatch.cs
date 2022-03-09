using EditorShortCuts.Utils;
using HarmonyLib;
using System;
using UnityEngine;

namespace EditorShortCuts.Patch
{
    public static class InputPatch
    {
        [HarmonyPatch(typeof(Input), "GetKey", typeof(KeyCode))]
        public static class GetKeyPatch
        {
            public static void Postfix(KeyCode key, ref bool __result)
            {
                if (scnEditor.instance != null && !scnEditor.instance.playMode) {
                    var tuple = Main.Settings.keys.Find(t => t.Item1 == (int)key);
                    if (tuple != null && tuple.Item2 != -1 && (int)key != tuple.Item2)
                            __result = typeof(Input).Method<bool>("GetKeyInt", new object[] { (KeyCode)tuple.Item2 });
                }
            }
        }

        [HarmonyPatch(typeof(Input), "GetKeyUp", typeof(KeyCode))]
        public static class GetKeyUpPatch
        {
            public static void Postfix(KeyCode key, ref bool __result)
            {
                if (scnEditor.instance != null && !scnEditor.instance.playMode)
                {
                    var tuple = Main.Settings.keys.Find(t => t.Item1 == (int)key);
                    if (tuple != null && tuple.Item2 != -1 && (int)key != tuple.Item2)
                        __result = typeof(Input).Method<bool>("GetKeyUpInt", new object[] { (KeyCode)tuple.Item2 });
                }
            }
        }

        [HarmonyPatch(typeof(Input), "GetKeyDown", typeof(KeyCode))]
        public static class GetKeyDownPatch
        {
            public static void Postfix(KeyCode key, ref bool __result)
            {
                if (scnEditor.instance != null && !scnEditor.instance.playMode)
                {
                    var tuple = Main.Settings.keys.Find(t => t.Item1 == (int)key);
                    if (tuple != null && tuple.Item2 != -1 && (int)key != tuple.Item2)
                        __result = typeof(Input).Method<bool>("GetKeyDownInt", new object[] { (KeyCode)tuple.Item2 });
                }
            }
        }
    }
}
