using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;

public class ExpenseManager : MonoBehaviour
{
    public GameObject expensePrefab;
    public Transform scrollViewContent;

    public TextMeshProUGUI categoryInputField;
    public TMP_InputField descriptionInputField;
    public TMP_InputField amountInputField;
    public Button DecideBTN;

    public CategorySelectionPanel categorySelectionPanel;

    private List<ExpenseData> expenseList = new List<ExpenseData>();
    private List<string> categoryList = new List<string>();

    private void Start()
    {
        DecideBTN.onClick.AddListener(OnAddExpense);
        LoadExpenseData();
        LoadCategories();
    }

    public void OnAddExpense()
    {
        if (string.IsNullOrEmpty(categoryInputField.text) || string.IsNullOrEmpty(descriptionInputField.text) || string.IsNullOrEmpty(amountInputField.text))
        {
            Debug.LogWarning("すべての項目を入力してください");
            return;
        }

        ExpenseData newExpense = new ExpenseData
        {
            date = System.DateTime.Now.ToString("yyyy-MM-dd"),
            category = categoryInputField.text,
            description = descriptionInputField.text,
            amount = int.Parse(amountInputField.text)
        };


        expenseList.Add(newExpense);
        SaveExpenseData();
        // スクロールビューに新しい項目を追加
        GameObject expenseItem = Instantiate(expensePrefab, scrollViewContent);

        Button toggleButton = expenseItem.transform.Find("CategoryButton").GetComponent<Button>();
        toggleButton.onClick.AddListener(() => categorySelectionPanel.ShowPanel(toggleButton));

        Button DecidedButton = expenseItem.transform.Find("DecideBTN").GetComponent<Button>();
        DecidedButton.onClick.AddListener(() => OnApplyButtonClicked(expenseItem, newExpense));

        Button DeleteButton = expenseItem.transform.Find("DeleteButton").GetComponent<Button>();
        DeleteButton.onClick.AddListener(() => DeleteExpense(newExpense, expenseItem));

        expenseItem.GetComponent<ExpenseItemUI>().Setup(newExpense);

        // 入力フィールドをクリア
        categoryInputField.text = "種類";
        descriptionInputField.text = "";
        amountInputField.text = "";
    }

    public void SaveExpenseData()
    {
        string path = Application.persistentDataPath + "/expenses.json";

        string newJson = JsonUtility.ToJson(new ExpenseList { expenses = expenseList });
        File.WriteAllText(path, newJson);
    }

    public void LoadExpenseData()
    {
        string path = Application.persistentDataPath + "/expenses.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            expenseList = JsonUtility.FromJson<ExpenseList>(json).expenses;

            foreach (var expense in expenseList)
            {
                GameObject expenseItem = Instantiate(expensePrefab, scrollViewContent);

                // TextMeshProUGUI timerText = expenseItem.transform.Find("TitleText").GetComponent<TextMeshProUGUI>();

                Button toggleButton = expenseItem.transform.Find("CategoryButton").GetComponent<Button>();
                toggleButton.onClick.AddListener(() => categorySelectionPanel.ShowPanel(toggleButton));

                Button DecidedButton = expenseItem.transform.Find("DecideBTN").GetComponent<Button>();
                DecidedButton.onClick.AddListener(() => OnApplyButtonClicked(expenseItem, expense));

                Button DeleteButton = expenseItem.transform.Find("DeleteButton").GetComponent<Button>();
                DeleteButton.onClick.AddListener(() => DeleteExpense(expense, expenseItem));

                expenseItem.GetComponent<ExpenseItemUI>().Setup(expense);
            }
        }
    }

    public void OnApplyButtonClicked(GameObject expenseItem, ExpenseData expense)
    {
        // 必要なUI要素を取得
        TMP_Text dateText = expenseItem.transform.Find("DateText").GetComponent<TMP_Text>();
        TMP_Text categoryText = expenseItem.transform.Find("CategoryButton/CategoryText").GetComponent<TMP_Text>();
        TMP_InputField descriptionText = expenseItem.transform.Find("DescriptionInputField").GetComponent<TMP_InputField>();
        TMP_InputField amountText = expenseItem.transform.Find("AmountInputField").GetComponent<TMP_InputField>();

        // データを更新
        expense.date = dateText.text;
        expense.category = categoryText.text;
        expense.description = descriptionText.text;

        int.TryParse(amountText.text, out expense.amount);

        // データを保存
        SaveExpenseData();
    }

    void DeleteExpense(ExpenseData expense, GameObject expenseItem)
    {
        expenseList.Remove(expense);
        Destroy(expenseItem);
        SaveExpenseData();
    }

    public void SaveCategory(string category)
    {
        if (!categoryList.Contains(category))
        {
            categoryList.Add(category);
            SaveCategories();
            categorySelectionPanel.AddCategoryButton(category);
        }
    }

    public void SaveCategories()
    {
        string path = Application.persistentDataPath + "/categories.json";
        string json = JsonUtility.ToJson(new CategoryList { categories = categoryList });
        File.WriteAllText(path, json);
    }

    public void LoadCategories()
    {
        string path = Application.persistentDataPath + "/categories.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            categoryList = JsonUtility.FromJson<CategoryList>(json).categories;

            foreach (var category in categoryList)
            {
                categorySelectionPanel.AddCategoryButton(category);
            }
        }
    }

    public void DeleteCategory(string category, GameObject buttonObj)
    {
        categoryList.Remove(category);
        Destroy(buttonObj);
        SaveCategories();
    }

    public void UpdateExpenseData(ExpenseData updatedExpense, int index)
    {
        string path = Application.persistentDataPath + "/expenses.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            expenseList = JsonUtility.FromJson<ExpenseList>(json).expenses;
            expenseList[index] = updatedExpense;

            string newJson = JsonUtility.ToJson(new ExpenseList { expenses = expenseList });
            File.WriteAllText(path, newJson);
        }
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
