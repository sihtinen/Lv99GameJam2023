using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SceneLoadTriggerArea : MonoBehaviour
{
    [SerializeField] private SceneReference m_sceneToLoad = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent(out PlayerCharacter _player) == false)
            return;

        var _sceneLoader = SceneLoader.Instance;
        if (_sceneLoader == null)
            return;

        _sceneLoader.LoadScene(m_sceneToLoad);
    }
}
