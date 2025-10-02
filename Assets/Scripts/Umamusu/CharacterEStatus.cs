using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterEStatus : MonoBehaviour
{
    private float zecchou = 0.0f;

    public int Koukando = 10; // スピードを初期値10で設定
    private int Hazukashido = 0; // 体力を初期値10で設定
    public int Kando = 10; // 筋力を初期値10で設定
    public int Mune = 10; // 技術を初期値10で設定
    public int Shita = 10; // 技術を初期値10で設定

    public TMP_Text ZecchouText;
    public TMP_Text KoukandoText;
    public TMP_Text HazukashidoText;
    public TMP_Text KandoText;
    public TMP_Text MuneText;
    public TMP_Text ShitaText;

    public Slider ToumeidoSlider;
    public Material targetMaterial; // The material whose alpha you want to control

    void Start()
    {
        // 初期化時にテキストを更新
        UpdateText();

        // Ensure the slider is properly initialized
        if (ToumeidoSlider != null && targetMaterial != null)
        {
            // Set the initial alpha value based on the slider value
            UpdateAlpha(ToumeidoSlider.value);

            // Add a listener to call UpdateAlpha whenever the slider value changes
            ToumeidoSlider.onValueChanged.AddListener(UpdateAlpha);
        }
    }

    void Update()
    {
    }

    void UpdateText()
    {
        // 各TMP_Textにステータスの値を表示する
        ZecchouText.text = zecchou.ToString("F2");
        KoukandoText.text = Koukando.ToString();
        HazukashidoText.text = Hazukashido.ToString();
        KandoText.text = Kando.ToString();
        MuneText.text = Mune.ToString();
        ShitaText.text = Shita.ToString();
    }
    // Method to update the alpha value
    void UpdateAlpha(float value)
    {
        // Assuming your shader uses a property called "_Alpha" for transparency
        targetMaterial.SetFloat("_Alpha", value);
    }



    // ステータスを向上させるメソッド
    public void IncreaseZecchou(float zecchouup)
    {
        zecchou += zecchouup;
        UpdateText();
    }

    public void IncreaseKoukando(int speedUp)
    {
        Koukando += speedUp;
        UpdateText();
    }

    public void SetHazukashido(int SetHazukashido)
    {
        Hazukashido = SetHazukashido;
        UpdateText();
    }
    public void IncreaseHazukashido(int IncreaseHazukashido)
    {
        Hazukashido += IncreaseHazukashido;

        UpdateText();
    }

    public void IncreaseKando(int strengthUp)
    {
        Kando += strengthUp;
        UpdateText();
    }

    public void IncreaseMune(int techUp)
    {
        Mune += techUp;
        UpdateText();
    }
    public void IncreaseShita(int techUp)
    {
        Shita += techUp;
        UpdateText();
    }
}
