using EditorShortCuts.Utils;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace EditorShortCuts
{
    public static class Main
    {
        public static UnityModManager.ModEntry.ModLogger Logger;
        public static Harmony harmony;
        public static bool IsEnabled = false;
        public static Settings Settings;

        public static void Setup(UnityModManager.ModEntry modEntry)
        {
            Logger = modEntry.Logger;
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            Logger.Log("Loading Settings...");
            Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            Logger.Log("Load Completed!");
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            IsEnabled = value;
            if (value)
            {
                harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            else
            {
                harmony.UnpatchAll(modEntry.Info.Id);
            }
            return true;
        }

        public static GUIStyle redButton;
        public static GUIStyle disabledButton;
        public static int detect = 0;
        public static List<int> remove = new List<int>();
        public static int insert = 0;
        public static Dictionary<int, int> add = new Dictionary<int, int>();

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (!redButton?.normal?.background)
            {
                redButton = new GUIStyle(GUI.skin.button);
                Texture2D redButtonNormal = redButton.normal.background.Copy();
                Texture2D redButtonHover = redButton.hover.background.Copy();
                Texture2D redButtonActive = redButton.active.background.Copy();
                for (int w = 0; w < redButtonNormal.width; w++)
                    for (int h = 0; h < redButtonNormal.height; h++)
                    {
                        if (redButtonNormal.GetPixel(w, h).a != 0)
                            redButtonNormal.SetPixel(w, h, Color.Lerp(redButtonNormal.GetPixel(w, h), Color.red, 0.5f));
                        if (redButtonHover.GetPixel(w, h).a != 0)
                            redButtonHover.SetPixel(w, h, Color.Lerp(redButtonHover.GetPixel(w, h), Color.red, 0.5f));
                        if (redButtonActive.GetPixel(w, h).a != 0)
                            redButtonActive.SetPixel(w, h, Color.Lerp(redButtonActive.GetPixel(w, h), Color.red, 0.5f));
                    }
                redButtonNormal.Apply();
                redButtonHover.Apply();
                redButtonActive.Apply();
                redButton.normal.background = redButtonNormal;
                redButton.hover.background = redButtonHover;
                redButton.active.background = redButtonActive;
            }
            if (!disabledButton?.normal?.background)
            {
                disabledButton = new GUIStyle(GUI.skin.button);
                Texture2D disabledButtonNormal = disabledButton.normal.background.Copy();
                for (int w = 0; w < disabledButtonNormal.width; w++)
                    for (int h = 0; h < disabledButtonNormal.height; h++)
                        if (disabledButtonNormal.GetPixel(w, h).a != 0)
                            disabledButtonNormal.SetPixel(w, h, Color.Lerp(disabledButtonNormal.GetPixel(w, h), Color.black, 0.5f));
                disabledButtonNormal.Apply();
                disabledButton.normal.background = disabledButtonNormal;
                disabledButton.hover.background = disabledButtonNormal;
                disabledButton.active.background = disabledButtonNormal;
            }
            int i = 0;
            foreach (var tuple in Settings.keys)
            {
                i++;
                GUILayout.BeginHorizontal();
                GUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Width(300));
                if (detect == i * 2)
                {
                    GUILayout.Button(RDString.language == SystemLanguage.Korean ? "입력 대기중..." : "Waiting...");
                    Event e = Event.current;
                    if (e.isKey && e.type == EventType.KeyDown)
                    {
                        KeyCode code = e.keyCode;
                        if (!Settings.keys.Exists(t => t.Item1 == (int)code))
                        {
                            insert = i - 1;
                            remove.Add(tuple.Item1);
                            add.Add((int)code, tuple.Item2);
                        }
                        detect = 0;
                    }
                } else if (GUILayout.Button(tuple.Item1 == -1 ? (RDString.language == SystemLanguage.Korean ? "없음" :"None") : ((KeyCode)tuple.Item1).ToString()) && detect == 0)
                    detect = i * 2;
                GUILayout.FlexibleSpace();
                GUILayout.Label(" => ");
                GUILayout.FlexibleSpace();
                if (detect == i * 2 + 1)
                {
                    GUILayout.Button(RDString.language == SystemLanguage.Korean ? "입력 대기중..." : "Waiting...");
                    Event e = Event.current;
                    if (e.isKey && e.type == EventType.KeyDown)
                    {
                        KeyCode code = e.keyCode;
                        insert = i - 1;
                        remove.Add(tuple.Item1);
                        add.Add(tuple.Item1, (int)code);
                        detect = 0;
                    }
                }
                else if (GUILayout.Button(tuple.Item2 == -1 ? (RDString.language == SystemLanguage.Korean ? "없음" : "None") : ((KeyCode)tuple.Item2).ToString()) && detect == 0)
                    detect = i * 2 + 1;
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                if (GUILayout.Button("↑", i == 1 ? disabledButton : GUI.skin.button, GUILayout.Width(25), GUILayout.Height(26)) && i != 1)
                {
                    insert = i - 2;
                    remove.Add(tuple.Item1);
                    add.Add(tuple.Item1, tuple.Item2);
                }
                GUILayout.Space(2);
                if (GUILayout.Button("↓", i == Settings.keys.Count ? disabledButton : GUI.skin.button, GUILayout.Width(25), GUILayout.Height(26)) && i != Settings.keys.Count)
                {
                    insert = i;
                    remove.Add(tuple.Item1);
                    add.Add(tuple.Item1, tuple.Item2);
                }
                GUILayout.Space(5);
                if (GUILayout.Button(RDString.language == SystemLanguage.Korean ? "삭제" : "Delete", redButton, GUILayout.Width(80), GUILayout.Height(26)))
                    remove.Add(tuple.Item1);
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button(RDString.language == SystemLanguage.Korean ? "추가" : "Add", GUILayout.Width(80), GUILayout.Height(26)) && !Settings.keys.Exists(t => t.Item1 == -1))
                Settings.keys.Add(new Tuple<int, int>(-1, -1));
            remove.ForEach(code => Settings.keys.RemoveAll(t => t.Item1 == code));
            remove.Clear();
            add.ToList().ForEach(pair => Settings.keys.Insert(insert, new Tuple<int, int>(pair.Key, pair.Value)));
            add.Clear();
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Logger.Log("Saving Settings...");
            Settings.Save(modEntry);
            Logger.Log("Save Completed!");
        }
    }
}