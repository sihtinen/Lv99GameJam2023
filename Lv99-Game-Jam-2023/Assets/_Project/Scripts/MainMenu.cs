using System.Collections.Generic;

using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private SceneReference m_firstGameScene = null;
    [SerializeField] private List<SceneReference> m_allScenes = new();
    [Space]
    [SerializeField] private Canvas m_mainMenuCanvas = null;
    [SerializeField] private Canvas m_levelSelectCanvas = null;
    [SerializeField] private RectTransform m_levelSelectVerticalGroup = null;
    [SerializeField] private MainMenuLevelButton m_levelButtonPrefab = null;

    private void Awake()
    {
        m_mainMenuCanvas.enabled = true;
        m_levelSelectCanvas.enabled = false;
    }

    private void Start()
    {
        for (int i = 0; i < m_allScenes.Count; i++)
        {
            var _buttonObj = Instantiate(m_levelButtonPrefab.gameObject, parent: m_levelSelectVerticalGroup);
            _buttonObj.TryGetComponent(out MainMenuLevelButton _button);
            _button.BindToScene(m_allScenes[i], $"Level {i+1}");
        }
    }

    public void Button_Play()
    {
        SceneLoader.Instance?.LoadScene(m_firstGameScene);
    }

    public void Button_LevelSelect()
    {
        m_mainMenuCanvas.enabled = false;
        m_levelSelectCanvas.enabled = true;
    }

    public void Button_ToMainMenu()
    {
        m_mainMenuCanvas.enabled = true;
        m_levelSelectCanvas.enabled = false;
    }
}
