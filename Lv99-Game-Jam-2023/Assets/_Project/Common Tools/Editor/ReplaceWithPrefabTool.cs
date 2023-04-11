using UnityEngine;
using UnityEditor;

public class ReplaceWithPrefabTool : EditorWindow
{
    [SerializeField] private GameObject prefab;

    [MenuItem("Tools/Replace With Prefab")]
    static void CreateReplaceWithPrefab()
    {
        EditorWindow.GetWindow<ReplaceWithPrefabTool>();
    }

    private void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        if (prefab != null && GUILayout.Button("Replace"))
        {
            var selection = Selection.gameObjects;

            for (var i = selection.Length - 1; i >= 0; --i)
            {
                var selected = selection[i];

                PrefabAssetType assetPrefabType = PrefabUtility.GetPrefabAssetType(prefab);

                GameObject newObject;

                switch (assetPrefabType)
                {
                    case PrefabAssetType.Regular:
                    case PrefabAssetType.Variant:
                        newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                        break;

                    default:
                        newObject = Instantiate(prefab);
                        newObject.name = prefab.name;
                        break;
                }

                if (newObject == null)
                {
                    Debug.LogError("Error instantiating prefab");
                    break;
                }

                Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
                newObject.transform.parent = selected.transform.parent;
                newObject.transform.localPosition = selected.transform.localPosition;
                newObject.transform.localRotation = selected.transform.localRotation;
                newObject.transform.localScale = selected.transform.localScale;
                newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
                Undo.DestroyObjectImmediate(selected);
            }
        }

        GUI.enabled = false;
        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
    }
}