using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace ARLens
{

    public class ARSceneDownloader : ScriptableObject
    {
        private GameObject arScene_ = null;
        private bool showPlaceholder_ = false;

        private static Dictionary<string, AssetBundle> cache = new Dictionary<string, AssetBundle>();


        public IEnumerator Download(string[] args)
        {
            IEnumerator bundleLoader = GetBundle(args[0]);
            yield return bundleLoader;

            AssetBundle bundle = (AssetBundle)bundleLoader.Current;

            var loadDlls = bundle.LoadAllAssetsAsync<TextAsset>();
            yield return loadDlls;

            Assembly assembly_with_placeholder = null;

            foreach (TextAsset dll in loadDlls.allAssets)
            {

                Assembly assembly = Assembly.Load(dll.bytes);
                if (assembly.GetType("ARCodeScripts.ARCodePlaceholderScript") != null) // TODO: make static string for script name
                {
                    assembly_with_placeholder = assembly;
                }
            }
            Debug.Log(assembly_with_placeholder);

            // assert(assembly_with_placeholder != null)

            var loadScenes = bundle.LoadAllAssetsAsync<GameObject>();
            yield return loadScenes;

            foreach (GameObject scene in loadScenes.allAssets) // TODO: fix (works only with one)
            {
                arScene_ = Instantiate(scene);
                Debug.Log(scene);

                // transfer args to plaseholder
                Transform ARCodePlaceholder = arScene_.transform.Find("ARCodePlaceholder");
                PropertyInfo placeholder_args = assembly_with_placeholder
                    .GetType("ARCodeScripts.ARCodePlaceholderScript").GetProperty("args");
                placeholder_args.SetValue(
                        ARCodePlaceholder.gameObject.GetComponent("ARCodeScripts.ARCodePlaceholderScript"),
                        args
                    );


                ARCodePlaceholder.GetComponent<Renderer>().enabled = showPlaceholder_;

                ARCodePlaceholder.SetParent(null);
                arScene_.transform.SetParent(ARCodePlaceholder);

                ARCodePlaceholder.position = new Vector3 { };
                ARCodePlaceholder.rotation = new Quaternion { };
                ARCodePlaceholder.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                arScene_ = ARCodePlaceholder.gameObject;
                arScene_.SetActive(false);
            }
            // bundle.Unload(false);
        }

        static IEnumerator GetBundle(string url)
        {
            // url = "file:///C:/Users/%D0%9D%D0%B8%D0%BA%D0%BE%D0%BB%D0%B0%D0%B9/UnityProjects/ARCode/ARScene/Assets/AssetBundles/content/earth";
            AssetBundle bundle;
            if (cache.ContainsKey(url))
            {
                // Возможно узкое место из-за механизмов работы ассетов. Может быть дублирование ресурсов, если бандл очистится из кэша. При очистке кэша точно надоо будет делать Unload(true).
                bundle = cache[url];
            }
            else
            {
                using (UnityWebRequest loadRequest = UnityWebRequestAssetBundle.GetAssetBundle(url))
                {
                    yield return loadRequest.SendWebRequest();
                    bundle = DownloadHandlerAssetBundle.GetContent(loadRequest);
                }
                cache[url] = bundle;
            }
            yield return bundle;
        }

        public GameObject result { get { return arScene_; } }
    }
}
