using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class CategorySelectionPanel : MonoBehaviour
{
    public GameObject panel;

    public TMP_InputField newCategoryInputField;
    public Transform categoryButtonContainer;
    public Button DecidedButton;

    public GameObject categoryButtonPrefab;
    public ExpenseManager expenseManager;

    private Button selectedCategoryButton;

    private void Start()
    {
        DecidedButton.onClick.AddListener(AddNewCategory);
    }

    public void ShowPanel(Button button)
    {
        selectedCategoryButton = button;
        panel.SetActive(true);
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }

    public void AddNewCategory()
    {
        string newCategory = newCategoryInputField.text;
        if (!string.IsNullOrEmpty(newCategory))
        {
            expenseManager.SaveCategory(newCategory);
            newCategoryInputField.text = "";
            selectedCategoryButton.GetComponentInChildren<TextMeshProUGUI>().text = newCategory;
        }

        HidePanel();
    }

    public void AddCategoryButton(string category)
    {
        GameObject buttonObj = Instantiate(categoryButtonPrefab, categoryButtonContainer);
        Button button = buttonObj.transform.Find("CategoryBTN").GetComponent<Button>();
        button.GetComponentInChildren<TextMeshProUGUI>().text = category;
        button.onClick.AddListener(() => OnCategorySelected(category));

        Button DeleteBTN = buttonObj.transform.Find("DeleteBTN").GetComponent<Button>();
        DeleteBTN.onClick.AddListener(() => expenseManager.DeleteCategory(category, buttonObj));
    }


    public void OnCategorySelected(string category)
    {
        if (selectedCategoryButton != null)
        {
            selectedCategoryButton.GetComponentInChildren<TextMeshProUGUI>().text = category;
            selectedCategoryButton = null;
        }
        HidePanel();
    }

    public void SetSelectedCategoryButton(Button button)
    {
        selectedCategoryButton = button;
    }
}
