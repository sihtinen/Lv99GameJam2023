using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SceneLoader : SingletonBehaviour<SceneLoader>
{
    private Coroutine m_activeCoroutine = null;
    private SceneReference m_sceneToLoad = null;

    public void LoadScene(SceneReference sceneRef)
    {
        if (m_activeCoroutine != null)
            return;

        m_sceneToLoad = sceneRef;

        var _transitionScreen = TransitionScreen.Instance;
        _transitionScreen.OnScreenObscured += this.onScreenObscured_LoadScene;
        _transitionScreen.StartTransition();
    }

    public void ReloadScene()
    {
        if (m_activeCoroutine != null)
            return;

        var _transitionScreen = TransitionScreen.Instance;
        _transitionScreen.OnScreenObscured += this.onScreenObscured_ReloadScene;
        _transitionScreen.StartTransition();
    }

    private void onScreenObscured_LoadScene()
    {
        var _transitionScreen = TransitionScreen.Instance;
        _transitionScreen.AllowTransition = false;
        _transitionScreen.OnScreenObscured -= this.onScreenObscured_LoadScene;

        m_activeCoroutine = StartCoroutine(coroutine_loadScene(m_sceneToLoad.ScenePath));
    }

    private void onScreenObscured_ReloadScene()
    {
        var _transitionScreen = TransitionScreen.Instance;
        _transitionScreen.AllowTransition = false;
        _transitionScreen.OnScreenObscured -= this.onScreenObscured_ReloadScene;

        string _currentScenePath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
        m_activeCoroutine = StartCoroutine(coroutine_loadScene(_currentScenePath));
    }

    private IEnumerator coroutine_loadScene(string scenePath)
    {
        var _loadOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenePath, UnityEngine.SceneManagement.LoadSceneMode.Single);

        while (_loadOp.isDone == false)
            yield return null;

        var _transitionScreen = TransitionScreen.Instance;
        _transitionScreen.AllowTransition = true;

        m_activeCoroutine = null;
    }
}