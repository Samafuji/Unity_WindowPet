using UnityEngine;
using TMPro;

public class AmountInputField : MonoBehaviour
{
    public TMP_InputField amountInputField;

    private void Start()
    {
        amountInputField.characterValidation = TMP_InputField.CharacterValidation.Integer;
    }
}
