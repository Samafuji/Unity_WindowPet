using UnityEngine;
using TMPro;

public class CharacterStatus : MonoBehaviour
{
    public int speed = 10; // スピードを初期値10で設定
    public int stamina = 10; // 体力を初期値10で設定
    public int strength = 10; // 筋力を初期値10で設定
    public int tech = 10; // 技術を初期値10で設定

    public TMP_Text speedText;
    public TMP_Text staminaText;
    public TMP_Text strengthText;
    public TMP_Text techText;

    void Start()
    {
        // 初期化時にテキストを更新
        UpdateText();
    }

    void UpdateText()
    {
        // 各TMP_Textにステータスの値を表示する
        speedText.text = "Speed: " + speed.ToString();
        staminaText.text = "Stamina: " + stamina.ToString();
        strengthText.text = "Strength: " + strength.ToString();
        techText.text = "Tech: " + tech.ToString();
    }

    // ステータスを向上させるメソッド
    public void IncreaseSpeed(int speedUp)
    {
        speed += speedUp;
        UpdateText();
    }

    public void IncreaseStamina(int staminaUp)
    {
        stamina += staminaUp;
        UpdateText();
    }

    public void IncreaseStrength(int strengthUp)
    {
        strength += strengthUp;
        UpdateText();
    }

    public void IncreaseTech(int techUp)
    {
        tech += techUp;
        UpdateText();
    }
}
