using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonScaleAndImageManager : MonoBehaviour
{
    public List<Button> buttons0;  // UnityのInspectorでボタンを設定
    public List<Button> buttons1;  // UnityのInspectorでボタンを設定
    public List<Button> buttons20;  // UnityのInspectorでボタンを設定
    public List<Button> buttons21;  // UnityのInspectorでボタンを設定
    public List<Button> buttons22;  // UnityのInspectorでボタンを設定
    public List<Button> buttons23;  // UnityのInspectorでボタンを設定
    private List<List<Button>> buttons2Group;  // 合体したボタンリスト
    public List<Button> buttons3;  // UnityのInspectorでボタンを設定

    public List<Image> arrow1;     // UnityのInspectorで矢印を設定

    public Sprite selectedSprite0; // 選択されたときの画像
    public Sprite normalSprite0;   // 通常の画像

    public Sprite selectedSprite1; // 選択されたときの画像
    public Sprite normalSprite1;   // 通常の画像

    public Sprite selectedSprite2; // 選択されたときの画像
    public Sprite normalSprite2;   // 通常の画像

    public Sprite selectedSprite3; // 選択されたときの画像
    public Sprite normalSprite3;   // 通常の画像

    public ScrollRect scrollRect;  // Scroll Rect コンポーネント

    private const float selectedScale2 = 1.13f;
    private const float normalScale2 = 1.0f;

    private const float selectedScale3 = 1.1f;
    private const float normalScale3 = 0.7f;

    void Start()
    {
        foreach (Button button in buttons0)
        {
            button.onClick.AddListener(() => UiClick0(button));
        }
        foreach (Button button in buttons1)
        {
            button.onClick.AddListener(() => UiClick1(button));
        }

        buttons2Group = new List<List<Button>> { buttons20, buttons21, buttons22, buttons23 };
        foreach (List<Button> buttons in buttons2Group)
        {
            foreach (Button button in buttons)
            {
                button.onClick.AddListener(() => UiClick2(button, buttons));
            }
        }

        foreach (Button button in buttons3)
        {
            button.onClick.AddListener(() => UiClick3(button));
        }
        // 初期状態として最初のボタンに対応する矢印のみを表示
        for (int i = 0; i < arrow1.Count; i++)
        {
            arrow1[i].gameObject.SetActive(i == 0);
        }
    }

    void UiClick0(Button clickedButton)
    {
        foreach (Button button in buttons0)
        {
            Image buttonImage = button.GetComponent<Image>();

            if (button == clickedButton)
            {
                buttonImage.sprite = selectedSprite0;
            }
            else
            {
                buttonImage.sprite = normalSprite0;
            }
        }
    }

    void UiClick1(Button clickedButton)
    {
        for (int i = 0; i < buttons1.Count; i++)
        {
            Image buttonImage = buttons1[i].GetComponent<Image>();

            if (buttons1[i] == clickedButton)
            {
                buttonImage.sprite = selectedSprite1;
                arrow1[i].gameObject.SetActive(true);  // 矢印を表示
            }
            else
            {
                buttonImage.sprite = normalSprite1;
                arrow1[i].gameObject.SetActive(false);  // 矢印を非表示
            }
        }
    }

    void UiClick2(Button clickedButton, List<Button> buttons2)
    {
        foreach (Button button in buttons2)
        {
            Image buttonImage = button.GetComponent<Image>();

            if (button == clickedButton)
            {
                button.transform.localScale = new Vector3(selectedScale2, selectedScale2, selectedScale2);
                buttonImage.sprite = selectedSprite2;
            }
            else
            {
                button.transform.localScale = new Vector3(normalScale2, normalScale2, normalScale2);
                buttonImage.sprite = normalSprite2;
            }
        }
    }

    void UiClick3(Button clickedButton)
    {
        foreach (Button button in buttons3)
        {
            Image buttonImage = button.GetComponent<Image>();

            if (button == clickedButton)
            {
                button.transform.localScale = new Vector3(selectedScale3, selectedScale3, selectedScale3);
                buttonImage.sprite = selectedSprite3;
                CenterButtonInView(clickedButton); // ボタンを中央にスクロールする
            }
            else
            {
                button.transform.localScale = new Vector3(normalScale3, normalScale3, normalScale3);
                buttonImage.sprite = normalSprite3;
            }
        }
    }

    void CenterButtonInView(Button button)
    {
        RectTransform contentRect = scrollRect.content;
        RectTransform buttonRect = button.GetComponent<RectTransform>();

        float contentWidth = contentRect.rect.width;
        float viewportWidth = scrollRect.viewport.rect.width;

        // ボタンの中心を計算
        float buttonCenterPosition = buttonRect.localPosition.x + contentRect.localPosition.x;

        // 新しいアンカー位置を計算
        float newAnchoredPositionX = buttonCenterPosition - (viewportWidth / 2);

        // コンテンツの幅とビューポートの幅を考慮して範囲を制限
        newAnchoredPositionX = Mathf.Clamp(newAnchoredPositionX, 0, contentWidth - viewportWidth);

        // 新しいアンカー位置を設定
        Vector2 newAnchoredPosition = new Vector2(-newAnchoredPositionX, contentRect.anchoredPosition.y);
        contentRect.anchoredPosition = newAnchoredPosition;
    }
}
