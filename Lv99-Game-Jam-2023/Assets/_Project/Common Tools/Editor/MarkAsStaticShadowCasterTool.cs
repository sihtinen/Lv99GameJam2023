using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using UnityEditor;
using UnityEditor.SceneManagement;

public class MarkAsStaticShadowCasterTool : EditorWindow
{
    [MenuItem("Tools/Batch Mark Renderers As Static Shadow Caster Tool")]
    private static void openWindow()
    {
        var _newWindow = GetWindow<MarkAsStaticShadowCasterTool>();
        _newWindow.Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Process Opened Scene"))
            processScene();

        if (GUILayout.Button("Remove Duplicate Materials"))
            scanForDuplicateMaterials();
    }

    private void processScene()
    {
        var _allRenderers = FindObjectsOfType<MeshRenderer>();

        for (int i = 0; i < _allRenderers.Length; i++)
        {
            var _rend = _allRenderers[i];

            if (_rend.gameObject.isStatic && _rend.staticShadowCaster == false)
            {
                _rend.staticShadowCaster = true;

                EditorUtility.SetDirty(_rend);
                EditorSceneManager.MarkSceneDirty(_rend.gameObject.scene);

                Debug.Log($"Marked renderer {_rend.gameObject.name} as static mesh renderer");
            }
        }
    }

    private void scanForDuplicateMaterials()
    {
        var _allRenderers = FindObjectsOfType<MeshRenderer>();

        for (int i = 0; i < _allRenderers.Length; i++)
        {
            var _rend = _allRenderers[i];

            if (_rend.sharedMaterials.Length <= 1)
                continue;

            var _mats = _rend.sharedMaterials;
            _mats = _mats.Distinct().ToArray();
            _rend.sharedMaterials = _mats;

            EditorUtility.SetDirty(_rend);
        }
    }
}
