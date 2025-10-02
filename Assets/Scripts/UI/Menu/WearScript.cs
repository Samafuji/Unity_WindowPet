using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterVisibilityController : MonoBehaviour
{
    public GameObject[] characterParts;  // キャラクターの全てのパーツ
    public GameObject[] specificPartsToHide;  // 非表示にする特定のパーツ

    public Button showAllButton;
    public Button anotherButton;
    public Button hideSpecificButton;
    void Start()
    {
        // 各ボタンにクリックイベントを登録
        showAllButton.onClick.AddListener(ShowAllParts);
        anotherButton.onClick.AddListener(AnotherFunction);
        hideSpecificButton.onClick.AddListener(HideSpecificParts);
    }

    void ShowAllParts()
    {
        foreach (GameObject part in specificPartsToHide)
        {
            part.SetActive(true);
        }
    }

    void HideSpecificParts()
    {
        foreach (GameObject part in specificPartsToHide)
        {
            part.SetActive(false);
        }
    }

    void AnotherFunction()
    {
        // ここに別の機能を実装
    }
}
