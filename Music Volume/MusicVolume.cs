using MelonLoader;
using UnityEngine;
using System.Diagnostics;
using System;
using System.IO;
using System.Reflection;

namespace MusicVolume {

    public class MusicVolume : MelonMod {

        private static QMSingleButton VolumeMinusButton;
        private static QMSingleButton VolumePlusButton;
        static string name;
        static int x;
        static int y;
        static int change;
        Vector2 plusPos;
        Vector2 minusPos;

        public override void OnApplicationStart() {
            if (!File.Exists(Environment.CurrentDirectory + @"\Mods\MusicVolume\SoundVolumeView.exe")) extract();

            MelonPrefs.RegisterCategory("MusicVolume", "MusicVolume");
            MelonPrefs.RegisterString("MusicVolume", "ProgramName", "Google Chrome", "Program name to change the volume of");
            MelonPrefs.RegisterString("MusicVolume", "Change%", "5", "What % to change the volume by when you click");
            MelonPrefs.RegisterString("MusicVolume", "ButtonX", "0", "X position of buttons");
            MelonPrefs.RegisterString("MusicVolume", "ButtonY", "1", "Y position of buttons");
            MelonPrefs.RegisterString("MusicVolume", "PlusButtonPosition", "topRight", "Plus button position");
            MelonPrefs.RegisterString("MusicVolume", "MinusButtonPosition", "bottomRight", "Minus button position");

            name = MelonPrefs.GetString("MusicVolume", "ProgramName");
            change = MelonPrefs.GetInt("MusicVolume", "Change%");
            x = MelonPrefs.GetInt("MusicVolume", "ButtonX");
            y = MelonPrefs.GetInt("MusicVolume", "ButtonY");
            plusPos = getVector(MelonPrefs.GetString("MusicVolume", "PlusButtonPosition"));
            minusPos = getVector(MelonPrefs.GetString("MusicVolume", "MinusButtonPosition"));
        }

        public static Vector2 getVector(String str) {
            switch (str.ToLower()) {
                case ("topright"):
                    return new Vector2(105f, 105f);
                case ("bottomright"):
                    return new Vector2(105f, -105f);
                case ("topleft"):
                    return new Vector2(-105f, 105f);
                case ("bottomleft"):
                    return new Vector2(-105f, -105f);
            }
            return new Vector2(0, 0);
        }

        public override void VRChat_OnUiManagerInit() {
            InitializeButtons();
        }

        private void InitializeButtons() {
            VolumeMinusButton = new QMSingleButton("ShortcutMenu", x, y, "-", delegate () {
                changeVolume((change*-1).ToString());
            }, "Decrease " + name + " volume.", null, null);
            VolumeMinusButton.getGameObject().GetComponent<RectTransform>().sizeDelta /= new Vector2(2.0f, 2.0f);
            VolumeMinusButton.getGameObject().GetComponent<RectTransform>().anchoredPosition += minusPos;

            VolumePlusButton = new QMSingleButton("ShortcutMenu", x, y, "+", delegate () {
                changeVolume(change.ToString());
            }, "Increase " + name + " volume.", null, null);
            VolumePlusButton.getGameObject().GetComponent<RectTransform>().sizeDelta /= new Vector2(2.0f, 2.0f);
            VolumePlusButton.getGameObject().GetComponent<RectTransform>().anchoredPosition += plusPos;
        }

        public static void changeVolume(string change) {
            Process p = new Process();
            p.StartInfo.FileName = @"Mods\MusicVolume\SoundVolumeView.exe";
            p.StartInfo.Arguments = "/ChangeVolume \"" + name + "\" " + change;
            p.Start();
        }

        public static void extract() {
            MelonLogger.Log("Extracting resources to mods folder");
            Directory.CreateDirectory(Environment.CurrentDirectory + @"\Mods\MusicVolume");
            extractFile("SoundVolumeView.exe");
            extractFile("SoundVolumeView.chm");
            extractFile("readme.txt");
        }

        public static void extractFile(string file) {
            using (Stream s = Assembly.GetCallingAssembly().GetManifestResourceStream("Music_Volume.SoundVolumeView." + file))
            using (BinaryReader r = new BinaryReader(s))
            using (FileStream fs = new FileStream(Environment.CurrentDirectory + @"\Mods\MusicVolume\" + file, FileMode.OpenOrCreate))
            using (BinaryWriter w = new BinaryWriter(fs))
                w.Write(r.ReadBytes((int)s.Length));
        }

    }

}