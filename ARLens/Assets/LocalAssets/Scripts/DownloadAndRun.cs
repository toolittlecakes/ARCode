using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadAndRun : MonoBehaviour
{


    private string url = "C:/Users/sne/UnityProjects/ARCode/ARScene/Assets/AssetBundles/content/earth";
    IEnumerator Start()
    {
        using (UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url))
        {
            yield return webRequest.SendWebRequest();
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);

            var loadDlls = bundle.LoadAllAssetsAsync<TextAsset>();
            yield return loadDlls;

            foreach (TextAsset dll in loadDlls.allAssets)
            {
                System.Reflection.Assembly.Load(dll.bytes);
            }


            var loadScenes = bundle.LoadAllAssetsAsync<GameObject>();
            yield return loadScenes;

            foreach (GameObject scene in loadScenes.allAssets)
            {
                var sceneInstant = Instantiate(scene, transform);
                Transform ARCodePlaceholder = sceneInstant.transform.Find("ARCodePlaceholder");
                ARCodePlaceholder.SetParent(transform);
                sceneInstant.transform.SetParent(ARCodePlaceholder);
                sceneInstant = ARCodePlaceholder.gameObject;
                sceneInstant.transform.Translate(1,1,1);
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