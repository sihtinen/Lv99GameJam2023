using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class ColliderBatchEditTool : EditorWindow
{
    [System.Serializable]
    public class SearchObjectWrapper
    {
        public GameObject TargetObject = null;
        public bool HasCollider = false;
        public string ColliderTypeString;
    }

    public enum ColliderType
    {
        Box,
        Mesh,
    }

    public List<string> FilterKeywords = new List<string>
    {
        "Terrain",
        "LOD0",
    };

    public ColliderType AddCollider = ColliderType.Mesh;

    public List<SearchObjectWrapper> FoundObjectWrappers = new List<SearchObjectWrapper>();

    private SerializedObject m_serializedObject = null;

    [MenuItem("Tools/Collider Batch Edit Tool")]
    private static void openWindow()
    {
        var _newWindow = GetWindow<ColliderBatchEditTool>();
        _newWindow.Show();
    }

    private void OnEnable()
    {
        m_serializedObject = new SerializedObject(this);
    }

    private void OnGUI()
    {
        buttonsSection();
        filterSection();
        searchResultsSection();

        m_serializedObject.ApplyModifiedProperties();
    }

    private void filterSection()
    {
        GUILayout.Space(15);

        GUILayout.Label("Filter Settings", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_serializedObject.FindProperty("FilterKeywords"));
    }

    private void buttonsSection()
    {
        GUILayout.Space(15);

        EditorGUILayout.PropertyField(m_serializedObject.FindProperty("AddCollider"));

        if (FoundObjectWrappers.Count > 0)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add colliders"))
            {
                removeColliders();
                addColliders();
                scanObjects();
            }
            if (GUILayout.Button("Remove colliders"))
            {
                removeColliders();
                scanObjects();
            }

            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Scan/validate scene objects"))
            scanObjects();
    }

    private Vector2 m_scrollPos;

    private void searchResultsSection()
    {
        if (FoundObjectWrappers.Count == 0)
            return;

        GUILayout.Space(10);

        GUILayout.Label("Found Objects", EditorStyles.boldLabel);

        GUILayout.Space(3);

        GUILayout.BeginVertical("box");

        m_scrollPos = GUILayout.BeginScrollView(
            m_scrollPos, 
            false, 
            true,
            GUILayout.MinHeight(200),
            GUILayout.MaxHeight(1000),
            GUILayout.ExpandHeight(true));

        for (int i = 0; i < FoundObjectWrappers.Count; i++)
        {
            var _wrapper = FoundObjectWrappers[i];

            GUILayout.BeginHorizontal();
            GUILayout.Label(_wrapper.TargetObject.name, GUILayout.MinWidth(200), GUILayout.Height(15));
            GUILayout.Label(_wrapper.ColliderTypeString, GUILayout.MinWidth(70), GUILayout.Height(15));

            GUILayout.Space(90);

            if (GUILayout.Button("Select", GUILayout.Height(15), GUILayout.MaxWidth(80)))
                Selection.activeObject = _wrapper.TargetObject;

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void scanObjects()
    {
        FoundObjectWrappers.Clear();

        GameObject[] _allGameObjects = FindObjectsOfType<GameObject>();

        if (_allGameObjects == null || _allGameObjects.Length == 0)
            return;

        for (int i = 0; i < FilterKeywords.Count; i++)
            FilterKeywords[i] = FilterKeywords[i].ToLowerInvariant();

        for (int i = 0; i < _allGameObjects.Length; i++)
        {
            var _currentObject = _allGameObjects[i];
            string _objectNameLowerCase = _currentObject.name.ToLowerInvariant();

            bool _isValidName = true;

            for (int ii = 0; ii < FilterKeywords.Count; ii++)
            {
                if (_objectNameLowerCase.Contains(FilterKeywords[ii]) == false)
                {
                    _isValidName = false;
                    break;
                }
            }

            if (_isValidName == false)
                continue;

            Collider _coll = _currentObject.GetComponent<Collider>();

            var _newWrapper = new SearchObjectWrapper()
            {
                TargetObject = _currentObject,
                HasCollider = _coll != null,
                ColliderTypeString = "-",
            };

            if (_newWrapper.HasCollider)
            {
                switch (_coll)
                {
                    case BoxCollider _boxColl:
                        _newWrapper.ColliderTypeString = "Box";
                        break;
                    case MeshCollider _meshColl:
                        _newWrapper.ColliderTypeString = "MeshCollider";
                        break;
                    default:
                        _newWrapper.ColliderTypeString = "Other";
                        break;
                }
            }

            FoundObjectWrappers.Add(_newWrapper);
        }
    }

    private void removeColliders()
    {
        for (int i = 0; i < FoundObjectWrappers.Count; i++)
        {
            var _wrapper = FoundObjectWrappers[i];
            var _foundColliders = _wrapper.TargetObject.GetComponents<Collider>();

            if (_foundColliders == null || _foundColliders.Length == 0)
                continue;

            foreach (var _coll in _foundColliders)
                DestroyImmediate(_coll);
        }
    }

    private void addColliders()
    {
        for (int i = 0; i < FoundObjectWrappers.Count; i++)
        {
            var _wrapper = FoundObjectWrappers[i];

            switch (AddCollider)
            {
                case ColliderType.Box:
                    _wrapper.TargetObject.AddComponent<BoxCollider>();
                    break;
                case ColliderType.Mesh:
                    _wrapper.TargetObject.AddComponent<MeshCollider>();
                    break;
            }
        }
    }
}
