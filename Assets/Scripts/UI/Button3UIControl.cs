using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public List<Button> buttons;  // UnityのInspectorでボタンを設定


    public Sprite selectedSprite; // 選択されたときの画像
    public Sprite normalSprite;   // 通常の画像

    private const float selectedScale3 = 1.1f;
    private const float normalScale3 = 0.7f;
    public ScrollRect scrollRect;  // Scroll Rect コンポーネント
    // Start is called before the first frame update
    void Start()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => UiClick(button));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UiClick(Button clickedButton)
    {
        foreach (Button button in buttons)
        {
            Image buttonImage = button.GetComponent<Image>();

            if (button == clickedButton)
            {
                button.transform.localScale = new Vector3(selectedScale3, selectedScale3, selectedScale3);
                buttonImage.sprite = selectedSprite;
                CenterButtonInView(clickedButton); // ボタンを中央にスクロールする
            }
            else
            {
                button.transform.localScale = new Vector3(normalScale3, normalScale3, normalScale3);
                buttonImage.sprite = normalSprite;
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
