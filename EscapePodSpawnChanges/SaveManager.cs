using HarmonyLib;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SMLHelper.V2.Utility;


namespace LifePodRemastered
{
    public class SaveManager
    {
        [System.Serializable]
        public class Saves
        {
            public string name = "JBEAST";
            public string lastname = "JBEASTLASTname";
        }

        public static Saves myData = new Saves();
        
        public static void WriteSettingsToCurrentSlot()
        {
            string StringOutPut = JsonUtility.ToJson(myData);
            File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/modconfig.json", StringOutPut);
        }

        public static void ReadSettingsToCurrectSlot()
        {
            string file = File.ReadAllText(SaveUtils.GetCurrentSaveDataDir() + "/modconfig.json");
            myData = JsonUtility.FromJson<Saves>(file);
            Debug.Log(myData.name);
            Debug.Log(myData.lastname);

        }
        public static FMODAsset GetFmodAsset(string audioPath)
        {
            FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();
            asset.path = audioPath;
            return asset;
        }
    }
}
