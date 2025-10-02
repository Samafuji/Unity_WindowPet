using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUp : MonoBehaviour
{
    public CharacterStatus characterStatus; // キャラクターのステータスを参照

    public Button speedButton;
    public Button staminaButton;
    public Button strengthButton;
    public Button techButton;
    int speedUp = 10;
    int staminaUp = 10;
    int strengthUp = 10;
    int techUp = 10;
    void Start()
    {
        // ボタンに対応するメソッドを設定
        speedButton.onClick.AddListener(() => characterStatus.IncreaseSpeed(speedUp));
        staminaButton.onClick.AddListener(() => characterStatus.IncreaseStamina(staminaUp));
        strengthButton.onClick.AddListener(() => characterStatus.IncreaseStrength(strengthUp));
        techButton.onClick.AddListener(() => characterStatus.IncreaseTech(techUp));
    }
}