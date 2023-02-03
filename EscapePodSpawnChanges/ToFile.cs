using HarmonyLib;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LifePodRemastered
{
    public class ToFile : MonoBehaviour
    {

        public class Saves
        {
            public string Slot;
            public bool HasIntroDone;
        }

        [System.Serializable]
        public class Data
        {
            public Saves[] SlotData;
        }

        public Data myData = new Data();

        public void CreateJson()
        {
            if (!File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/modconfig.json"))
            {
                File.Create(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/modconfig.json");
            }
            else
            {
                return;
            }
        }
        public void WriteSettingsToJson()
        {
/*            myData.name = "JBEAST";
            myData.lastname = "JBEASTLASTname";*/
            string StringOutPut = JsonUtility.ToJson(myData);

            File.WriteAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/modconfig.json", StringOutPut);
        }
        public void GetSlotInfo()
        {
            //myData = JsonUtility.FromJson<Dat
        }
    }
}
