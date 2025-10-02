using UnityEngine;
using UnityEngine.UI;

public class MainButtonController : MonoBehaviour
{
    public GameObject subButtonPrefab;  // サブボタンのプレハブ
    public Transform subButtonParent;   // サブボタンの親オブジェクト

    private void Start()
    {
        // メインボタンのクリックイベントを設定
        Button mainButton = GetComponent<Button>();
        mainButton.onClick.AddListener(ShowSubButtons);
    }

    void ShowSubButtons()
    {
        // 5つのサブボタンを生成
        for (int i = 0; i < 5; i++)
        {
            GameObject subButton = Instantiate(subButtonPrefab, subButtonParent);
            subButton.GetComponentInChildren<Text>().text = "SubButton " + (i + 1);
            subButton.GetComponent<Button>().onClick.AddListener(() => OnSubButtonClick(subButton));
        }
    }

    void OnSubButtonClick(GameObject subButton)
    {
        // サブボタンクリック時の処理
        Debug.Log(subButton.GetComponentInChildren<Text>().text + " clicked");
        // 必要に応じて、対応するUI要素を表示する処理を追加
    }
}
