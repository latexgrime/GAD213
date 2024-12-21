using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    public class AssetBundleBuilder : EditorWindow
    {
        [MenuItem("Tools/Build Asset Bundles")]
        static void BuildAllAssetBundles()
        {
            string assetBundleDirectory = "Assets/AssetBundles";
            if (!Directory.Exists(assetBundleDirectory))
            {
                Debug.Log("No AssetBundles folder found in Assets. Creating one...");
                Directory.CreateDirectory(assetBundleDirectory);
            }

            BuildPipeline.BuildAssetBundles(
                assetBundleDirectory,
                BuildAssetBundleOptions.None,
                BuildTarget.StandaloneWindows64);
            
            Debug.Log($"Asset bundles built successfully in {assetBundleDirectory}");
        }
    }
}
