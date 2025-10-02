using UnityEngine;
using UnityEngine.UI;

public class VerticalScrollButtons : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public RectTransform[] buttons;
    public float movementRange = 300f; // ボタンの移動範囲
    public float centerX = 0f; // ボタンのX座標の中心位置
    public float centerY = 0f; // ボタンのY座標の中心位置

    private void Update()
    {
        float contentHeight = content.rect.height - scrollRect.viewport.rect.height;
        if (contentHeight <= 0) return;

        float scrollPosition = scrollRect.verticalNormalizedPosition * contentHeight;

        for (int i = 0; i < buttons.Length; i++)
        {
            float positionFactor = (scrollPosition / contentHeight) + i / (float)buttons.Length;
            float angle = positionFactor * 2 * Mathf.PI;
            float y = centerY + movementRange * Mathf.Sin(angle);
            buttons[i].localPosition = new Vector2(centerX, y);

            // 透明度の調整
            float alpha = CalculateAlpha(angle);
            SetButtonAlpha(buttons[i], alpha);
            float size = CalculateSize(angle);
            buttons[i].transform.localScale = new Vector3(size, size, size);

            // z位置の調整
            float z = -size; // スケールに基づいてz位置を設定
            buttons[i].localPosition = new Vector3(centerX, y, z);

            // 透明度が0の場合はボタンを非表示にする
            if (alpha == 0)
            {
                buttons[i].gameObject.SetActive(false);
            }
            else
            {
                buttons[i].gameObject.SetActive(true);
            }
        }
    }

    private float CalculateAlpha(float angle)
    {
        // 角度をラジアンから度に変換
        float degrees = angle * Mathf.Rad2Deg;
        // 度数を0から360の範囲に正規化
        degrees = (degrees + 360) % 360;

        if (degrees >= 120 && degrees <= 240)
        {
            // 120度から240度の間で透明度を最大にする
            return 1;
        }

        if (degrees >= 75 && degrees <= 285)
        {
            // 75度から285度の間で透明度を線形に変化させる
            return Mathf.Cos((degrees - 225) * Mathf.Deg2Rad / 5 * 3);
        }
        return 0f;
    }

    private float CalculateSize(float angle)
    {
        // 角度をラジアンから度に変換
        float degrees = angle * Mathf.Rad2Deg;
        // 度数を0から360の範囲に正規化
        degrees = (degrees + 360) % 360;

        if (degrees >= 75 && degrees <= 285)
        {
            // 75度から285度の間でサイズを線形に変化させる
            return Mathf.Cos((degrees - 165) * Mathf.Deg2Rad / 5 * 3.5f);
        }
        return 0f;
    }

    private void SetButtonAlpha(RectTransform button, float alpha)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = alpha;
            buttonImage.color = color;
        }

        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            Color textColor = buttonText.color;
            textColor.a = alpha;
            buttonText.color = textColor;
        }

        // ボタンに付属している画像の透明度も設定する
        Image[] childImages = button.GetComponentsInChildren<Image>();
        foreach (Image img in childImages)
        {
            if (img != buttonImage) // ボタン自身のImageは既に設定されているため
            {
                Color imgColor = img.color;
                imgColor.a = alpha;
                img.color = imgColor;
            }
        }
    }
}
