using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using Item = System.Collections.Generic.KeyValuePair<string, string>;
public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        var dir = Directory.CreateDirectory("Assets/DllBytes");
        CreateBytesDllAssets();

        BuildPipeline.BuildAssetBundles(
            "Assets/AssetBundles",
            // BuildAssetBundleOptions.UncompressedAssetBundle,
            BuildAssetBundleOptions.None,
            BuildTarget.Android
        );

        Directory.Delete("Assets/DllBytes", true);
    }


    static void CreateBytesDllAssets()
    {
        var labels = AssetDatabase.GetAllAssetBundleNames();
        foreach (var label in labels)
        {
            var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(label);
            foreach (var assetPath in assetPaths)
            {
                if (assetPath.EndsWith(".asmdef"))
                {
                    ImportDllAsBytes(Path.GetFileNameWithoutExtension(assetPath), label);
                }
                Debug.Log(Path.GetFileNameWithoutExtension(assetPath));
            }
            ImportDllAsBytes("ARCodeScripts", label, label.Split('/')[1]);
        }
    }
    static void ImportDllAsBytes(string name, string label, string suffix = "")
    {

        var dllPath = Path.Combine("Library/ScriptAssemblies", Path.ChangeExtension(name, "dll"));
        Debug.Log(dllPath);
        
        var bytesPath = Path.Combine("Assets/DllBytes/", Path.ChangeExtension(name + suffix, "bytes"));
        Debug.Log(bytesPath);


        File.Copy(dllPath, bytesPath, true);

        AssetDatabase.ImportAsset(bytesPath, ImportAssetOptions.DontDownloadFromCacheServer);
        AssetDatabase.SaveAssets();


        AssetImporter assetImporter = AssetImporter.GetAtPath(bytesPath);
        assetImporter.assetBundleName = label;
        assetImporter.name = name;
        assetImporter.SaveAndReimport();

    }



}