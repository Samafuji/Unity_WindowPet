using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpenseManager : MonoBehaviour
{
    private const string ExpenseFileName = "expenses.json";
    private const string CategoryFileName = "categories.json";

    [Header("Prefabs & Containers")]
    public GameObject expensePrefab;
    public Transform scrollViewContent;

    [Header("Input Fields")]
    public TextMeshProUGUI categoryInputField;
    public TMP_InputField descriptionInputField;
    public TMP_InputField amountInputField;
    public Button DecideBTN;

    [Header("Optional Summary")]
    public TextMeshProUGUI totalAmountText;

    public CategorySelectionPanel categorySelectionPanel;

    private readonly List<ExpenseData> expenseList = new List<ExpenseData>();
    private readonly List<string> categoryList = new List<string>();
    private readonly Dictionary<ExpenseData, GameObject> expenseUIMap = new Dictionary<ExpenseData, GameObject>();

    private void Start()
    {
        if (DecideBTN != null)
        {
            DecideBTN.onClick.AddListener(OnAddExpense);
        }

        LoadExpenseData();
        LoadCategories();
        RebuildExpenseList();
    }

    public void OnAddExpense()
    {
        string category = categoryInputField != null ? categoryInputField.text.Trim() : string.Empty;
        string description = descriptionInputField != null ? descriptionInputField.text.Trim() : string.Empty;
        string amountText = amountInputField != null ? amountInputField.text.Trim() : string.Empty;

        if (string.IsNullOrEmpty(category) || category == "種類")
        {
            Debug.LogWarning("カテゴリーを選択してください");
            return;
        }

        if (string.IsNullOrEmpty(description))
        {
            Debug.LogWarning("内容を入力してください");
            return;
        }

        if (!TryParseAmount(amountText, out int amount))
        {
            Debug.LogWarning("金額の形式が正しくありません");
            return;
        }

        ExpenseData newExpense = new ExpenseData
        {
            date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture),
            category = category,
            description = description,
            amount = amount
        };

        expenseList.Add(newExpense);
        SortExpenses();
        RebuildExpenseList();
        SaveExpenseData();
        ClearInputFields();
    }

    private void RebuildExpenseList()
    {
        foreach (KeyValuePair<ExpenseData, GameObject> entry in expenseUIMap)
        {
            if (entry.Value != null)
            {
                Destroy(entry.Value);
            }
        }

        expenseUIMap.Clear();

        if (expensePrefab == null || scrollViewContent == null)
        {
            return;
        }

        foreach (ExpenseData expense in expenseList)
        {
            if (expense == null)
            {
                continue;
            }

            GameObject expenseItem = Instantiate(expensePrefab, scrollViewContent);
            ConfigureExpenseItem(expenseItem, expense);
            expenseUIMap[expense] = expenseItem;
        }

        UpdateTotalAmount();
    }

    private void ConfigureExpenseItem(GameObject expenseItem, ExpenseData expense)
    {
        if (expenseItem == null)
        {
            return;
        }

        ExpenseItemUI itemUI = expenseItem.GetComponent<ExpenseItemUI>();

        if (itemUI != null)
        {
            itemUI.Setup(expense);
        }

        Button categoryButton = itemUI != null && itemUI.categoryBTN != null
            ? itemUI.categoryBTN
            : expenseItem.transform.Find("CategoryButton")?.GetComponent<Button>();

        if (categoryButton != null)
        {
            categoryButton.onClick.RemoveAllListeners();
            categoryButton.onClick.AddListener(() =>
            {
                if (categorySelectionPanel != null)
                {
                    categorySelectionPanel.SetSelectedCategoryButton(categoryButton);
                    categorySelectionPanel.ShowPanel(categoryButton);
                }
            });
        }

        Button decideButton = expenseItem.transform.Find("DecideBTN")?.GetComponent<Button>();

        if (decideButton != null)
        {
            decideButton.onClick.RemoveAllListeners();
            decideButton.onClick.AddListener(() => OnApplyButtonClicked(expenseItem, expense));
        }

        Button deleteButton = expenseItem.transform.Find("DeleteButton")?.GetComponent<Button>();

        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(() => DeleteExpense(expense, expenseItem));
        }
    }

    public void SaveExpenseData()
    {
        string path = Path.Combine(Application.persistentDataPath, ExpenseFileName);
        ExpenseList wrapper = new ExpenseList { expenses = expenseList };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, json);
        UpdateTotalAmount();
    }

    public void LoadExpenseData()
    {
        expenseList.Clear();

        string path = Path.Combine(Application.persistentDataPath, ExpenseFileName);

        if (!File.Exists(path))
        {
            return;
        }

        string json = File.ReadAllText(path);

        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        ExpenseList wrapper = JsonUtility.FromJson<ExpenseList>(json);

        if (wrapper != null && wrapper.expenses != null)
        {
            expenseList.AddRange(wrapper.expenses);
        }

        SortExpenses();
    }

    public void OnApplyButtonClicked(GameObject expenseItem, ExpenseData expense)
    {
        if (expenseItem == null || expense == null)
        {
            return;
        }

        ExpenseItemUI itemUI = expenseItem.GetComponent<ExpenseItemUI>();
        bool updated = true;

        if (itemUI != null)
        {
            updated = itemUI.TryApplyChanges(out string errorMessage);

            if (!updated)
            {
                Debug.LogWarning(errorMessage);
            }
        }
        else
        {
            TMP_Text dateText = expenseItem.transform.Find("DateText")?.GetComponent<TMP_Text>();
            TMP_Text categoryText = expenseItem.transform.Find("CategoryButton/CategoryText")?.GetComponent<TMP_Text>();
            TMP_InputField descriptionText = expenseItem.transform.Find("DescriptionInputField")?.GetComponent<TMP_InputField>();
            TMP_InputField amountText = expenseItem.transform.Find("AmountInputField")?.GetComponent<TMP_InputField>();

            if (dateText != null)
            {
                expense.date = dateText.text;
            }

            if (categoryText != null)
            {
                expense.category = categoryText.text;
            }

            if (descriptionText != null)
            {
                expense.description = descriptionText.text;
            }

            if (amountText != null)
            {
                updated = TryParseAmount(amountText.text, out int parsedAmount);
                expense.amount = updated ? parsedAmount : expense.amount;

                if (!updated)
                {
                    amountText.text = expense.amount.ToString(CultureInfo.CurrentCulture);
                    Debug.LogWarning("金額の形式が正しくありません");
                }
            }
        }

        if (updated)
        {
            SortExpenses();
            RebuildExpenseList();
            SaveExpenseData();
        }
    }

    private void DeleteExpense(ExpenseData expense, GameObject expenseItem)
    {
        if (expenseItem != null)
        {
            Destroy(expenseItem);
        }

        expenseUIMap.Remove(expense);
        expenseList.Remove(expense);
        SaveExpenseData();
        RebuildExpenseList();
    }

    public void SaveCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category) || categoryList.Contains(category))
        {
            return;
        }

        categoryList.Add(category);
        SaveCategories();

        if (categorySelectionPanel != null)
        {
            categorySelectionPanel.AddCategoryButton(category);
        }
    }

    public void SaveCategories()
    {
        string path = Path.Combine(Application.persistentDataPath, CategoryFileName);
        CategoryList wrapper = new CategoryList { categories = categoryList };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, json);
    }

    public void LoadCategories()
    {
        categoryList.Clear();

        string path = Path.Combine(Application.persistentDataPath, CategoryFileName);

        if (!File.Exists(path))
        {
            return;
        }

        string json = File.ReadAllText(path);

        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        CategoryList wrapper = JsonUtility.FromJson<CategoryList>(json);

        if (wrapper != null && wrapper.categories != null)
        {
            categoryList.AddRange(wrapper.categories);
        }

        if (categorySelectionPanel != null)
        {
            foreach (string category in categoryList)
            {
                categorySelectionPanel.AddCategoryButton(category);
            }
        }
    }

    public void DeleteCategory(string category, GameObject buttonObj)
    {
        categoryList.Remove(category);

        if (buttonObj != null)
        {
            Destroy(buttonObj);
        }

        SaveCategories();
    }

    public void UpdateExpenseData(ExpenseData updatedExpense, int index)
    {
        string path = Path.Combine(Application.persistentDataPath, ExpenseFileName);

        if (!File.Exists(path))
        {
            return;
        }

        string json = File.ReadAllText(path);
        ExpenseList wrapper = JsonUtility.FromJson<ExpenseList>(json);

        if (wrapper == null || wrapper.expenses == null || index < 0 || index >= wrapper.expenses.Count)
        {
            return;
        }

        wrapper.expenses[index] = updatedExpense;
        string newJson = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, newJson);
    }

    private void ClearInputFields()
    {
        if (categoryInputField != null)
        {
            categoryInputField.text = "種類";
        }

        if (descriptionInputField != null)
        {
            descriptionInputField.text = string.Empty;
        }

        if (amountInputField != null)
        {
            amountInputField.text = string.Empty;
        }
    }

    private void UpdateTotalAmount()
    {
        if (totalAmountText == null)
        {
            return;
        }

        int total = 0;

        foreach (ExpenseData expense in expenseList)
        {
            if (expense != null)
            {
                total += expense.amount;
            }
        }

        totalAmountText.text = string.Format(CultureInfo.CurrentCulture, "合計: ¥{0:N0}", total);
    }

    private void SortExpenses()
    {
        expenseList.Sort((a, b) =>
        {
            DateTime aDate = ParseDateOrDefault(a?.date);
            DateTime bDate = ParseDateOrDefault(b?.date);

            int dateComparison = bDate.CompareTo(aDate);

            if (dateComparison != 0)
            {
                return dateComparison;
            }

            return string.Compare(a?.category, b?.category, StringComparison.CurrentCulture);
        });
    }

    private static DateTime ParseDateOrDefault(string dateText)
    {
        if (DateTime.TryParse(dateText, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime parsed))
        {
            return parsed;
        }

        return DateTime.MinValue;
    }

    private static bool TryParseAmount(string amountText, out int amount)
    {
        amount = 0;

        if (string.IsNullOrWhiteSpace(amountText))
        {
            return false;
        }

        string sanitized = amountText.Replace(",", string.Empty).Trim();

        if (!int.TryParse(sanitized, NumberStyles.Integer, CultureInfo.CurrentCulture, out int parsedAmount))
        {
            return false;
        }

        amount = Mathf.Max(0, parsedAmount);
        return true;
    }
}

[System.Serializable]
public class ExpenseData
{
    public string date;
    public string category;
    public string description;
    public int amount;
}

[System.Serializable]
public class ExpenseList
{
    public List<ExpenseData> expenses;
}

[System.Serializable]
public class CategoryList
{
    public List<string> categories;
}
