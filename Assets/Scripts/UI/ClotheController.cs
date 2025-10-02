using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClotheController : MonoBehaviour
{
    public GameObject ClothePanel;  // 表示・非表示を切り替えるパネル
    public GameObject ClothePanelYukata;  // 表示・非表示を切り替えるパネル
    public Button toggleButton0_1;  // 表示・非表示を切り替えるボタン
    public Button toggleButton0_2;  // 表示・非表示を切り替えるボタン
    public Button toggleButton2;  // 表示・非表示を切り替えるボタン

    private bool IsNormal = true;

    private void Start()
    {
        ClothePanel.SetActive(false);
        ClothePanelYukata.SetActive(false);

        toggleButton0_1.onClick.AddListener(Toggle);
        toggleButton0_2.onClick.AddListener(TogglePanel);
        toggleButton2.onClick.AddListener(TogglePanel2);
    }

    public void Toggle()
    {
        // 現在表示中のパネルを切り替える
        if (IsNormal)
        {
            ClothePanel.SetActive(true);
            ClothePanelYukata.SetActive(false);
        }
        else
        {
            ClothePanel.SetActive(false);
            ClothePanelYukata.SetActive(true);
        }
        Debug.Log(IsNormal);
    }
    public void TogglePanel()
    {
        IsNormal = false;
        Toggle();
    }
    public void TogglePanel2()
    {
        IsNormal = true;
        Toggle();
    }
}
