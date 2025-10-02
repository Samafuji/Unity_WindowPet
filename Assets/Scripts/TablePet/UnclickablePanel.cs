using UnityEngine;
using UnityEngine.UI;

public class UnclickablePanel : MonoBehaviour
{
    public GameObject panel; // Assign your panel in the inspector

    void Start()
    {
        SetPanelUnclickable(panel);
    }

    private void SetPanelUnclickable(GameObject panel)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>();
        }

        // Make panel not interactable
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false; // Allows clicks to pass through
    }
}
