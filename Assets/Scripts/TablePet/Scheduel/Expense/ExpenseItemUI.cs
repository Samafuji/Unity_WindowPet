using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpenseItemUI : MonoBehaviour
{
    public TextMeshProUGUI dateText;
    public Button categoryBTN;
    public TextMeshProUGUI categoryText;
    public TMP_InputField descriptionText;
    public TMP_InputField amountText;

    private ExpenseData currentExpense;

    public void Setup(ExpenseData expense)
    {
        currentExpense = expense;
        RefreshFromData();
    }

    public bool TryApplyChanges(out string errorMessage)
    {
        errorMessage = string.Empty;

        if (currentExpense == null)
        {
            errorMessage = "データが見つかりません";
            return false;
        }

        if (dateText != null)
        {
            currentExpense.date = dateText.text;
        }

        if (categoryText != null)
        {
            currentExpense.category = categoryText.text;
        }

        if (descriptionText != null)
        {
            currentExpense.description = descriptionText.text;
        }

        if (amountText != null)
        {
            string sanitized = amountText.text.Replace(",", string.Empty).Trim();

            if (!int.TryParse(sanitized, NumberStyles.Integer, CultureInfo.CurrentCulture, out int parsedAmount))
            {
                errorMessage = "金額の形式が正しくありません";
                amountText.text = currentExpense.amount.ToString(CultureInfo.CurrentCulture);
                return false;
            }

            currentExpense.amount = Mathf.Max(0, parsedAmount);
            amountText.text = currentExpense.amount.ToString(CultureInfo.CurrentCulture);
        }

        return true;
    }

    public void RefreshFromData()
    {
        if (currentExpense == null)
        {
            return;
        }

        if (dateText != null)
        {
            dateText.text = currentExpense.date;
        }

        if (categoryText != null)
        {
            categoryText.text = currentExpense.category;
        }

        if (descriptionText != null)
        {
            descriptionText.text = currentExpense.description;
        }

        if (amountText != null)
        {
            amountText.text = currentExpense.amount.ToString(CultureInfo.CurrentCulture);
        }
    }
}
