using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BuildIncrementor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        var _infoAsset = AssetDatabase.LoadAssetAtPath<BuildInformationAsset>(BuildInformationAsset.AssetDataPath);

        if (_infoAsset == null)
            return;

        _infoAsset.BuildVersion++;
        EditorUtility.SetDirty(_infoAsset);
    }
}
