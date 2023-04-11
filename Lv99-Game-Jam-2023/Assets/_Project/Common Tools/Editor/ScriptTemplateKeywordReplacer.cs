using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using UnityEditor;


namespace Tensori.CommonTools.Editor
{
    public class ScriptTemplateKeywordReplacer : AssetModificationProcessor
    {
        private static readonly string PROJECT_NAME = "Tensori";

        public static void OnWillCreateAsset(string metaFilePath)
        {
            string _fileName = Path.GetFileNameWithoutExtension(metaFilePath);

            if (_fileName.EndsWith(".cs") == false)
                return;

            string _actualFilePath = $"{Path.GetDirectoryName(metaFilePath)}{Path.DirectorySeparatorChar}{_fileName}";
            string _content = File.ReadAllText(_actualFilePath);
            string _newcontent = _content.Replace("#PROJECTNAME#", PROJECT_NAME);

            if (_content != _newcontent)
            {
                File.WriteAllText(_actualFilePath, _newcontent);
                AssetDatabase.Refresh();
            }
        }
    }
}
