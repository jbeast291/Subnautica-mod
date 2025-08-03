using FMOD;
using Nautilus.Assets;
using Nautilus.Handlers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LifePodRemastered.presetSystem;

public class BasePreset
{
    public string fileName;
    public BasePreset(string fileName)
    {
        this.fileName = fileName;
    }
    public int ConfigVersion;
    public Vector3 location;
    public Vector2 storageSize;
    public bool heavyPod;
    public bool startRepaired;
    public bool customIntro;
    public List<LatestItemFormat> LifePodStorageInfo;
    public List<TechType> LifePodStorageTechTypes;
    public bool errorsDuringParsing = false;

    public bool Load()
    {
        string filePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "Presets",
            fileName + ".json");

        string raw = File.ReadAllText(filePath);
        PresetVersionFormat deserializedVersionInfoOnly;
        try
        {
            deserializedVersionInfoOnly = JsonConvert.DeserializeObject<PresetVersionFormat>(raw);
        }
        catch (JsonReaderException ex)
        {
            BepInExEntry.Logger.LogError($"Unable to read \"{fileName}.json\"! JsonReaderException, please check that your preset.json file is formated correctly. There should be a comma after entries (except the last). If you need help, ask in the subnautica modding discord and show them your preset file.");
            return false;
        }

        if (deserializedVersionInfoOnly == null)
        {
            BepInExEntry.Logger.LogError($"Unable to read \"{fileName}.json\"! No version field found.");
            return false;
        }
        else if(deserializedVersionInfoOnly.ConfigVersion <= 1)
        {
            BepInExEntry.Logger.LogInfo($"Loading \"{fileName}.json\" preset with V1 loader...");
            PresetV1Loader.PresetDataV1Format deserializedV1 = JsonConvert.DeserializeObject<PresetV1Loader.PresetDataV1Format>(raw);
            return PresetV1Loader.Load(this,deserializedV1);
        } 
        else
        {
            BepInExEntry.Logger.LogError($"Unable to load \"{fileName}.json\"! No loader available for specified version.");
            return false;
        }
    }
    public string getItemListString()
    {
        string itemList = "\n";
        foreach (LatestItemFormat item in LifePodStorageInfo)
        {
            if (item.ErrorsDuringParsingTechType)
            {
                itemList += "<color=#ff0000ff>";
            }
            itemList += $"{item.Quantity} {item.Name}";
            if (item.IsModded)
            {
                itemList += $" ({item.ModGUID})";
            }
            if (item.ErrorsDuringParsingTechType)
            {
                itemList += "</color>";
            }
            itemList += "\n";
        }

        return itemList;
    }

    public class LatestItemFormat : PresetV1Loader.PresetDataV1Format.ItemFormat;
    public class PresetVersionFormat { public int ConfigVersion { get; set; } }
}



