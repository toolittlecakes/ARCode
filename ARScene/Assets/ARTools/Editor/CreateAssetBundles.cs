using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class CreateAssetBundles
{


    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {

        var dllNamesAndLabels = FindAllAsmdefsWithLabels();

        CreateBytesDllAssets(dllNamesAndLabels);

        BuildPipeline.BuildAssetBundles(
            "Assets/AssetBundles", 
            // BuildAssetBundleOptions.UncompressedAssetBundle,
            BuildAssetBundleOptions.None,
            BuildTarget.Android
        );

        DeleteBytesDllAssets(dllNamesAndLabels);


    }


    static Dictionary<string, string> FindAllAsmdefsWithLabels()
    {
        Dictionary<string, string> asmdefNames = new Dictionary<string, string>();
        var labels = AssetDatabase.GetAllAssetBundleNames();
        foreach (var label in labels)
        {
            var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(label);
            foreach (var assetPath in assetPaths)
            {
                if (assetPath.EndsWith(".asmdef"))
                {
                    asmdefNames.Add(
                        Path.GetFileNameWithoutExtension(assetPath),
                        label
                    );
                }
            }
        }
        return asmdefNames;
    }

    static void CreateBytesDllAssets(Dictionary<string, string> dllNamesAndLabels)
    {
        foreach (var dllNameAndLabel in dllNamesAndLabels)
        {
            var dllName = dllNameAndLabel.Key;
            var dllLabel = dllNameAndLabel.Value;

            var dllPath = Path.Combine("Library/ScriptAssemblies", Path.ChangeExtension(dllName, "dll"));
            Debug.Log(dllPath);
            var dstPath = Path.Combine("Assets", Path.ChangeExtension(dllName, "bytes"));
            Debug.Log(dstPath);


            File.Copy(dllPath, dstPath, true);
            AssetDatabase.ImportAsset(dstPath, ImportAssetOptions.ImportRecursive);
            AssetDatabase.SaveAssets();


            AssetImporter assetImporter = AssetImporter.GetAtPath(dstPath);
            assetImporter.assetBundleName = dllLabel;
            assetImporter.SaveAndReimport();
        }
    }
    static void DeleteBytesDllAssets(Dictionary<string, string> dllNamesAndLabels)
    {
        foreach (var dllName in dllNamesAndLabels.Keys)
        {
            AssetDatabase.DeleteAsset(Path.Combine("Assets", Path.ChangeExtension(dllName, "bytes")));

        }
    }
   
}
