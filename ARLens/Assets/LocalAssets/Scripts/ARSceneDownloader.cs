using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ARLens
{

    public class ARSceneDownloader : ScriptableObject
    {
        private GameObject arScene_ = null;
        private bool showPlaceholder_ = true;
        public ARSceneDownloader()
        {}

        public IEnumerator Download(string url)
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


                // var loadScenes = bundle.LoadAllAssetsAsync<GameObject>();
                var loadScenes = bundle.LoadAllAssetsAsync<GameObject>();
                yield return loadScenes;


                foreach (GameObject scene in loadScenes.allAssets) // TODO: fix (works only with one)
                {
                    arScene_ = Instantiate(scene);
                    Transform ARCodePlaceholder = arScene_.transform.Find("ARCodePlaceholder");
                    if (!showPlaceholder_)
                    {
                        ARCodePlaceholder.GetComponent<Renderer>().enabled = false;
                    }

                    ARCodePlaceholder.SetParent(null);
                    arScene_.transform.SetParent(ARCodePlaceholder);


                    var pos = arScene_.transform.localPosition;
                    var rot = arScene_.transform.localRotation;
                    var localScale = arScene_.transform.localScale;
                    var scale = new Vector3(1f/localScale.x, 1f/localScale.y, 1f/localScale.z);
                    pos.Scale(scale);

                    arScene_.transform.SetParent(null);
                    Destroy(ARCodePlaceholder.gameObject);

                    arScene_.transform.SetPositionAndRotation(pos, rot);
                    arScene_.SetActive(false);
                }
                bundle.Unload(false);
            }
        }

        public GameObject result { get { return arScene_; } }

    }
}
