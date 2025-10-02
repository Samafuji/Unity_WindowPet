using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClickerGame : MonoBehaviour
{
    public TMP_Text moneyText;
    public Button clickButton;
    public Button upgradeClickButton;
    public Button upgradeTimeButton;
    public TMP_Text clickUpgradeCostText;
    public TMP_Text timeUpgradeCostText;

    private float money = 0f;
    private float clickValue = 1f;
    private float timeValue = 1f;
    private float timeInterval = 1f;
    private float timer = 0f;

    private float clickUpgradeCost = 10f;
    private float timeUpgradeCost = 10f;

    void Start()
    {
        clickButton.onClick.AddListener(OnClick);
        upgradeClickButton.onClick.AddListener(UpgradeClick);
        upgradeTimeButton.onClick.AddListener(UpgradeTime);
        UpdateMoneyText();
        UpdateUpgradeCostText();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeInterval)
        {
            timer = 0f;
            money += timeValue;
            UpdateMoneyText();
        }
    }

    void OnClick()
    {
        money += clickValue;
        UpdateMoneyText();
    }

    void UpgradeClick()
    {
        if (money >= clickUpgradeCost)
        {
            money -= clickUpgradeCost;
            clickValue *= 2;
            clickUpgradeCost *= 2; // アップグレードごとにコストを2倍にする
            UpdateMoneyText();
            UpdateUpgradeCostText();
        }
    }

    void UpgradeTime()
    {
        if (money >= timeUpgradeCost)
        {
            money -= timeUpgradeCost;
            timeValue *= 2;
            timeUpgradeCost *= 2; // アップグレードごとにコストを2倍にする
            UpdateMoneyText();
            UpdateUpgradeCostText();
        }
    }

    void UpdateMoneyText()
    {
        moneyText.text = money.ToString("F0");
    }

    void UpdateUpgradeCostText()
    {
        clickUpgradeCostText.text = "Click Upgrade Cost: " + clickUpgradeCost.ToString("F0");
        timeUpgradeCostText.text = "Time Upgrade Cost: " + timeUpgradeCost.ToString("F0");
    }
}
