using BepInEx;
using Nautilus.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LifePodRemastered.presetSystem.BasePreset;

namespace LifePodRemastered.presetSystem;

public class PresetV1Loader
{
    public static bool Load(BasePreset preset, PresetDataV1Format deserialized)
    {
        preset.ConfigVersion = deserialized.ConfigVersion;
        if (Util.isStringVector3(deserialized.location))
        {
            preset.location = Util.StringToVector3(deserialized.location);
        }
        else
        {
            BepInExEntry.Logger.LogError($"Failed to parse Location from \"{preset.fileName}.json\", not Vector3! Check if you followed the proper formatting.");
            preset.errorsDuringParsing = true;
        }
        if(OptionsMono.storageSizes.TryGetValue(deserialized.storageSize, out Vector2 storageSize))
        {
            preset.storageSize = storageSize;
        }
        else
        {
            BepInExEntry.Logger.LogError($"Failed to parse StorageSize from \"{preset.fileName}.json\"! It must be set one of these values: {string.Join(", ", OptionsMono.storageSizes.Keys)}");
            preset.storageSize = new Vector2(4, 8);
        }

        preset.heavyPod = deserialized.heavyPod;
        preset.startRepaired = deserialized.startRepaired;
        preset.customIntro = deserialized.customIntro;

        preset.LifePodStorageInfo = deserialized.lifePodStorage
            .Select(item => new LatestItemFormat
            {
                Name = item.Name,
                Quantity = item.Quantity,
                IsModded = item.IsModded,
                ModGUID = item.ModGUID,
                ErrorsDuringParsingTechType = false
            }).ToList();

        BepInExEntry.Logger.LogInfo($"Parsing \"{preset.fileName}.json\" items to techtypes...");
        preset.LifePodStorageTechTypes = new List<TechType>();
        for (int i = 0; i < deserialized.lifePodStorage.Count; i++)
        {
            PresetDataV1Format.ItemFormat item = deserialized.lifePodStorage[i];
            TechType itemTechType;
            if (!item.IsModded)
            {
                itemTechType = GetVinillaTechType(preset, item);
            } 
            else
            {
                itemTechType = GetModdedTechType(preset, item);
            }

            if (itemTechType == TechType.None)
            {
                preset.LifePodStorageInfo[i].ErrorsDuringParsingTechType = true;
                continue;
            }
            for (int j = 0; j < item.Quantity; j++)
            {
                preset.LifePodStorageTechTypes.Add(itemTechType);
            }
        }
        BepInExEntry.Logger.LogInfo($"Finished Loading \"{preset.fileName}.json\" preset");
        return true;
    }
    private static TechType GetVinillaTechType(BasePreset preset, PresetDataV1Format.ItemFormat item)
    {
        if (UWE.Utils.TryParseEnum<TechType>(item.Name, out TechType result))
        {
            return result;
        }
        BepInExEntry.Logger.LogError($"Failed to parse {item.Name} from \"{preset.fileName}.json\" into a valid item! Check spelling/accuracy!");
        preset.errorsDuringParsing = true;
        return TechType.None;
    }

    private static TechType GetModdedTechType(BasePreset preset, PresetDataV1Format.ItemFormat item)
    {
        if (EnumHandler.TryGetValue<TechType>(item.Name, out TechType result))
        {
            return result;
        }
        preset.errorsDuringParsing = true;
        if (item.ModGUID.IsNullOrWhiteSpace())
        {
            BepInExEntry.Logger.LogError($"Failed to parse {item.Name} from \"{preset.fileName}.json\" into a valid item! ModGUID is missing. Although it is not required, having it will give more info into the cause of the error.");

        }
        else if (!BepInExEntry.isGUIDLoaded(item.ModGUID))
        {
            BepInExEntry.Logger.LogError($"Failed to parse {item.Name} from \"{preset.fileName}.json\" into a valid item! {item.ModGUID} is not loaded, ensure you install this mod.");

        } 
        else
        {
            BepInExEntry.Logger.LogError($"Failed to parse {item.Name} from \"{preset.fileName}.json\" into a valid item! {item.ModGUID} is loaded however but the way it handles items may not be compatible. Double check that the item id is correct! If all else fails, create a bug report for life pod remastered");
        }
        return TechType.None;
    }


    public class PresetDataV1Format
    {
        public int ConfigVersion;
        public string location;
        public string storageSize;
        public bool heavyPod;
        public bool startRepaired;
        public bool customIntro;
        public List<ItemFormat> lifePodStorage;
        public class ItemFormat
        {
            public string Name;
            public int Quantity;
            public bool IsModded;
            public string ModGUID;
            public bool ErrorsDuringParsingTechType;
        }

    }

}

