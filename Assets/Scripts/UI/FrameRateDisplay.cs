using UnityEngine;
using TMPro; // TextMeshProを使用する場合

public class FrameRateDisplay : MonoBehaviour
{
    public TextMeshProUGUI frameRateText; // UIに表示するためのTextMeshProUGUI
    private float deltaTime = 0.0f;

    void Update()
    {
        // フレームごとの経過時間を計算
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        // フレームレートをテキストに表示
        frameRateText.text = string.Format("{0:0.} FPS", fps);
    }
}
