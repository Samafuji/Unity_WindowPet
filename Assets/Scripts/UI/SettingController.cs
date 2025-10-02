using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    public GameObject imgPanel;  // 表示・非表示を切り替えるパネル
    public Button toggleButton;  // 表示・非表示を切り替えるボタン

    private void Start()
    {
        toggleButton.onClick.AddListener(TogglePanel);
    }

    public void TogglePanel()
    {
        // パネルのアクティブ状態を切り替える
        imgPanel.SetActive(!imgPanel.activeSelf);
    }
}
