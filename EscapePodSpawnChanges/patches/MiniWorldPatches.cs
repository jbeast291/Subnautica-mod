using HarmonyLib;
using LifePodRemastered.Monos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;
using static MiniWorld;

namespace LifePodRemastered.patches;

[HarmonyPatch(typeof(MiniWorld))]
internal class MiniWorldPatches
{

    static Vector3 unityPointToWorldPointOffset = new Vector3(2048, 3040, 2048);

    [HarmonyPatch(typeof(MiniWorld), "UpdatePosition")]
    [HarmonyPrefix]
    public static bool UpdatePositionPreFix(MiniWorld __instance)
    {
        if(LargeWorldStreamer.main == null)
        {
            //Debug.Log("isRunning");
            __instance.hologramHolder.rotation = Quaternion.identity;
            Vector3 b = (LPRGlobals.SelectedSpawn + unityPointToWorldPointOffset) / 4f;
            foreach (KeyValuePair<Int3, MiniWorld.Chunk> keyValuePair in __instance.loadedChunks)
            {
                MiniWorld.Chunk value = keyValuePair.Value;
                Vector3 a = (keyValuePair.Key * 32).ToVector3() - b;
                value.gameObject.transform.localPosition = a * __instance.chunkScale;
            }
            return false;
        }

        return true;

    }

    [HarmonyPatch(typeof(MiniWorld), nameof(MiniWorld.RebuildHologram))]
    class Patch_RebuildHologram
    {
        static bool Prefix(MiniWorld __instance, ref IEnumerator __result)
        {
            if(LargeWorldStreamer.main == null)
            {
                __result = MyCustomCoroutine(__instance);
                return false; // skip original
            }
            return true;
        }

        static IEnumerator MyCustomCoroutine(MiniWorld instance)
        {
            while (!(instance == null))
            {
                if (!instance.gameObject.activeInHierarchy)
                {
                    instance.ClearAllChunks();
                }
                else if (instance.gameObject.activeInHierarchy)
                {
                    Int3 block = Int3.Floor(LPRGlobals.SelectedSpawn + unityPointToWorldPointOffset);
                    
                    int minY = 1300;//lowest realistically needed to be loaded is 1300 
                    Int3 u = new Int3(block.x - instance.mapWorldRadius, minY, block.z - instance.mapWorldRadius);
                    int belowSelectedPoint = (int)(LPRGlobals.SelectedSpawn.y + unityPointToWorldPointOffset.y + 100);
                    int maxY = Math.Min(belowSelectedPoint, 3200);
                    Int3 u2 = new Int3(block.x + instance.mapWorldRadius, maxY, block.z + instance.mapWorldRadius);
                    block >>= 2;
                    Int3 mins = (u >> 2) / 32;
                    Int3 maxs = (u2 >> 2) / 32;
                    bool chunkAdded = false;
                    Int3.RangeEnumerator iter = Int3.Range(mins, maxs);
                    while (iter.MoveNext())
                    {
                        Int3 chunkId = iter.Current;
                        instance.requestChunks.Add(chunkId);
                        if (!instance.GetChunkExists(chunkId))
                        {
                            string chunkPath = instance.GetChunkFilename(chunkId);
                            if (!AddressablesUtility.Exists<Mesh>(chunkPath))
                            {
                                continue;
                            }
                            AsyncOperationHandle<Mesh> request = AddressablesUtility.LoadAsync<Mesh>(chunkPath);
                            yield return request;
                            if (instance == null)
                            {
                                AddressablesUtility.QueueRelease<Mesh>(ref request);
                                yield break;
                            }
                            if (request.Status == AsyncOperationStatus.Failed || instance.GetChunkExists(chunkId))
                            {
                                continue;
                            }
                            instance.GetOrMakeChunk(chunkId, request, chunkPath);
                            chunkAdded = true;
                            chunkPath = null;
                            request = default(AsyncOperationHandle<Mesh>);
                        }
                        chunkId = default(Int3);
                    }
                    iter = default(Int3.RangeEnumerator);
                    instance.ClearUnusedChunks(instance.requestChunks);
                    instance.requestChunks.Clear();
                    if (chunkAdded)
                    {
                        instance.UpdatePosition();
                    }
                }
                yield return new WaitForSeconds(MiniWorldController.main.mapRebuildRate);
            }
            yield break;
        }
    }

    [HarmonyPatch(typeof(MiniWorld), "GetOrMakeChunk")]
    [HarmonyPrefix]
    public static bool GetOrMakeChunkPreFix(Int3 chunkId, AsyncOperationHandle<Mesh> handle, string chunkPath, MiniWorld __instance)
    {
        GameObject gameObject;
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        if (__instance.chunkPool.Count > 0)
        {
            gameObject = __instance.chunkPool.Pop();
            gameObject.name = chunkPath;
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.enabled = true;
            meshFilter = gameObject.GetComponent<MeshFilter>();
        }
        else
        {
            gameObject = new GameObject(chunkPath);
            gameObject.transform.SetParent(__instance.hologramObject.transform, false);
            gameObject.transform.localScale = new Vector3(__instance.chunkScale, __instance.chunkScale, __instance.chunkScale);
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

            //Render layer
            int uiLayer = LayerMask.NameToLayer("Minimap");
            gameObject.layer = uiLayer;
        }

        // Set local position immediately, fixes weird popin
        Vector3 b = (LPRGlobals.SelectedSpawn + unityPointToWorldPointOffset) / 4f;
        Vector3 chunkPos = (chunkId * 32).ToVector3() - b;
        gameObject.transform.localPosition = chunkPos * __instance.chunkScale;

        __instance.loadedChunks.Add(chunkId, new MiniWorld.Chunk
        {
            handle = handle,
            gameObject = gameObject
        });
        meshFilter.sharedMesh = handle.Result;
        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        meshRenderer.sharedMaterial = __instance.materialInstance;
        meshRenderer.receiveShadows = false;
        return false;
    }
}

