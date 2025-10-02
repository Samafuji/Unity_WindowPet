using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpenseItemUI : MonoBehaviour
{
    public TextMeshProUGUI dateText;
    public Button categoryBTN;
    public TextMeshProUGUI categoryText;
    public TMP_InputField descriptionText;
    public TMP_InputField amountText;

    private ExpenseData currentExpense;
    private int expenseIndex;

    public void Setup(ExpenseData expense)
    {
        currentExpense = expense;
        dateText.text = expense.date;
        categoryText.text = expense.category;

        descriptionText.text = expense.description;
        amountText.text = expense.amount.ToString();
    }

    public void OnEditExpense()
    {
        // 編集画面のUIを呼び出し、ユーザーが内容を更新できるようにする
        // 編集が完了したら、ExpenseManagerのUpdateExpenseDataを呼び出してJSONを更新
    }

    public void OnDeleteExpense()
    {
        // 現在の項目を削除し、JSONファイルからも削除する処理を行う
        Destroy(gameObject);
    }
}
