using UnityEngine;
using UnityEngine.UI;

public class Button2MenuController : MonoBehaviour
{
    public GameObject[] objects;  // 5つのオブジェクト
    public Button[] buttons;      // 5つのボタン

    void Start()
    {
        // 各ボタンにリスナーを追加
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;  // キャプチャ変数
            buttons[i].onClick.AddListener(() => ShowOnlySelectedObject(index));
        }
    }

    // 選択されたオブジェクトだけを表示し、他のオブジェクトを非表示にするメソッド
    void ShowOnlySelectedObject(int index)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            if (i == index)
            {
                objects[i].SetActive(true);
            }
            else
            {
                objects[i].SetActive(false);
            }
        }
    }
}
