using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    public string sceneName;
    public Button switchSceneButton;

    void Start()
    {
        switchSceneButton.onClick.AddListener(OnSwitchSceneButtonClicked);
    }

    void OnSwitchSceneButtonClicked()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            // 指定されたシーン名に切り替える
            SceneManager.LoadScene(sceneName);
        }
    }
}
