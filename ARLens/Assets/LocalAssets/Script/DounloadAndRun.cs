using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class DounloadAndRun : MonoBehaviour
{


    private string url = "file:///C:/Users/sne/RemoteContent/Assets/AssetBundles/content/new_cube1";
    IEnumerator Start()
    {
        using (UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url))
        {
            yield return webRequest.SendWebRequest();
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);
            // var loadAsset = bundle.LoadAllAssetsAsync<TextAsset>();
            var loadMapAsset = bundle.LoadAssetAsync("Map.json");
            var loadScriptsAsset = bundle.LoadAssetAsync("Scripts.bytes");
            var loadSceneAsset = bundle.LoadAssetAsync("Scene.prefab");
            yield return loadMapAsset;
            yield return loadScriptsAsset;
            yield return loadSceneAsset;

            TextAsset map = loadMapAsset.allAssets[0] as TextAsset;
            TextAsset txt = loadScriptsAsset.allAssets[0] as TextAsset;
            GameObject scene = loadSceneAsset.allAssets[0] as GameObject;

            Debug.Log(map.text);
            var assembly = System.Reflection.Assembly.Load(txt.bytes);
            System.IO.File.WriteAllBytes("C:/Users/sne/UnityProjects/ExternalCodeLauncher/log.txt", txt.bytes);
            var Map = JsonConvert.DeserializeObject<Dictionary<string, string>>(map.text);
            Debug.Log(Map.Keys.ToString());
            Debug.Log(Map.Values);

            // foreach (var type in types) {
                // GameObject.FindWithTag(type.Name).gameObject.AddComponent(type);
            // } 

            // scene.AddComponent(type0    );
            // Debug.Log(scene.GetComponentInChildren<MonoBehaviour>());
            // scene.AddComponent(type);
            // ins.GetComponentInChildren(type0).gameObject.AddComponent(type0);
            // Instantiate a GameObject and add a component with the loaded class
            var ins = Instantiate(scene, transform);
            Debug.Log(assembly.ImageRuntimeVersion);
            Debug.Log(assembly.ManifestModule);
            Debug.Log(assembly.Location);
            Debug.Log(assembly.CodeBase);
            Debug.Log(assembly.EntryPoint);

            foreach(var item in Map) 
            {   
                GameObject obj = GameObject.Find(item.Key);
                obj.AddComponent(assembly.GetType(item.Value));
            }

        }
    }
    // void Start()
    // {
    //     var bytes = System.IO.File.ReadAllBytes("C:/Users/sne/UnityProjects/ExternalCodeLauncher/Assets/Prefabs/SinMotion.bytes");
    //     var assembly = System.Reflection.Assembly.Load(bytes);
    //     var type = assembly.GetType("SinMotion");
    //     Debug.Log(type);
    //     // Instantiate a GameObject and add a component with the loaded class
    //     gameObject.AddComponent(type);
    // }

}