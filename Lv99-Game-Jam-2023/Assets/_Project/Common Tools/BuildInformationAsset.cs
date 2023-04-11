using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class BuildInformationAsset : ScriptableObject
{
    public static string AssetName => "BuildInformation";
    public static string AssetDataPath => $"Assets/_Project/Resources/{AssetName}.asset";
    public string FullBuildVersion => BuildPrefix + Application.version + $".{BuildVersion.ToString("000")}";

    public string BuildPrefix = "yob-alpha-";
    public int BuildVersion = 0;
}