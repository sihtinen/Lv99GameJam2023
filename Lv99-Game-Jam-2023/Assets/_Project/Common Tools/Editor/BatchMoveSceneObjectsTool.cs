using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;

public class BatchMoveSceneObjectsTool : EditorWindow
{
    [SerializeField] private Vector3 m_moveVector = Vector3.zero;
    [SerializeField] private string m_validLayerName = "Default";
    [SerializeField] private bool m_rootObjectsOnly = true;

    private List<GameObject> m_validSceneObjects = new List<GameObject>();

    [MenuItem("Tools/Batch Move Scene Objects Tool")]
    static void CreateReplaceWithPrefab()
    {
        EditorWindow.GetWindow<BatchMoveSceneObjectsTool>();
    }

    private void OnEnable()
    {
        Selection.selectionChanged += this.onSelectionChanged;
        onSelectionChanged();
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= this.onSelectionChanged;
    }

    private void onSelectionChanged()
    {
        var _selectedObjects = Selection.gameObjects;

        m_validSceneObjects.Clear();
        m_validSceneObjects.AddRange(_selectedObjects);

        for (int i = m_validSceneObjects.Count; i-- > 0;)
        {
            var _obj = m_validSceneObjects[i];

            if (_obj.scene.isLoaded == false || _obj.scene.IsValid() == false)
            {
                m_validSceneObjects.RemoveAt(i);
                continue;
            }

            if (m_rootObjectsOnly && _obj.transform.parent != null)
            {
                m_validSceneObjects.RemoveAt(i);
                continue;
            }

            if (LayerMask.LayerToName(_obj.layer) != m_validLayerName)
            {
                m_validSceneObjects.RemoveAt(i);
                continue;
            }
        }
    }

    private void OnGUI()
    {
        m_moveVector = EditorGUILayout.Vector3Field("Move Vector", m_moveVector);
        m_validLayerName = EditorGUILayout.TextField("Valid Layer Name", m_validLayerName);
        m_rootObjectsOnly = EditorGUILayout.Toggle("Root Objects Only", m_rootObjectsOnly);

        if (m_validSceneObjects.Count == 0)
            return;

        EditorGUILayout.LabelField($"{m_validSceneObjects.Count} root objects selected");

        if (GUILayout.Button("Move"))
        {
            for (int i = 0; i < m_validSceneObjects.Count; i++)
            {
                var _obj = m_validSceneObjects[i];
                _obj.transform.position += m_moveVector;
                EditorUtility.SetDirty(_obj);
                EditorSceneManager.MarkSceneDirty(_obj.scene);

                EditorUtility.DisplayProgressBar(
                    "Batch moving root objects...",
                    $"Moving object: {_obj.name} ({i}/{m_validSceneObjects.Count})",
                    progress: (float)i/(float)m_validSceneObjects.Count);
            }

            EditorUtility.ClearProgressBar();
        }
    }
}