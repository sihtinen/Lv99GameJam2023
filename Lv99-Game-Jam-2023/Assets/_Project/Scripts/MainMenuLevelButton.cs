using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class MainMenuLevelButton : MonoBehaviour
{
    [SerializeField] private TMP_Text m_levelNameText = null;

    private SceneReference m_scene = null;

    public void BindToScene(SceneReference scene, string displayName)
    {
        m_scene = scene;
        m_levelNameText.SetText(displayName);
    }

    public void Button_LoadLevel()
    {
        SceneLoader.Instance?.LoadScene(m_scene);
    }
}
