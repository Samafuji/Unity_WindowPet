using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonScaleAndImageManager : MonoBehaviour
{
    [System.Serializable]
    public class ButtonGroupConfig
    {
        [Tooltip("Optional label to make it easier to identify the group in the inspector.")]
        public string groupName;

        [Tooltip("Buttons that belong to this group.")]
        public List<Button> buttons = new List<Button>();

        [Tooltip("Sprite used when a button is not selected. Leave empty to keep the original sprite.")]
        public Sprite normalSprite;

        [Tooltip("Sprite used when a button is selected. Leave empty to keep the original sprite.")]
        public Sprite selectedSprite;

        [Header("Scaling")]
        [Tooltip("Enable to apply scaling when a button is selected.")]
        public bool applyScale;

        [Tooltip("Scale applied when a button is selected.")]
        public float selectedScale = 1.1f;

        [Tooltip("Scale applied when a button is not selected.")]
        public float normalScale = 1.0f;

        [Header("Arrows")]
        [Tooltip("Enable to toggle arrow indicators that correspond to each button.")]
        public bool toggleArrowIndicators;

        [Tooltip("Arrow indicators that should mirror the selection state. The list should match the button count.")]
        public List<Image> arrowIndicators = new List<Image>();

        [Header("Selection Behaviour")]
        [Tooltip("Select and highlight the first button in the list when the scene starts.")]
        public bool selectFirstOnStart;

        [Tooltip("Center the selected button inside the associated ScrollRect viewport.")]
        public bool centerSelectedButton;
    }

    [SerializeField]
    private List<ButtonGroupConfig> buttonGroups = new List<ButtonGroupConfig>();

    [SerializeField]
    [Tooltip("ScrollRect used when centering buttons. Optional unless 'Center Selected Button' is enabled on a group.")]
    private ScrollRect scrollRect;

    private readonly Dictionary<Button, UnityAction> _registeredListeners = new Dictionary<Button, UnityAction>();

    private void OnEnable()
    {
        RegisterButtonListeners();
        InitializeGroups();
    }

    private void OnDisable()
    {
        foreach (KeyValuePair<Button, UnityAction> entry in _registeredListeners)
        {
            if (entry.Key != null)
            {
                entry.Key.onClick.RemoveListener(entry.Value);
            }
        }

        _registeredListeners.Clear();
    }

    private void RegisterButtonListeners()
    {
        for (int groupIndex = 0; groupIndex < buttonGroups.Count; groupIndex++)
        {
            ButtonGroupConfig group = buttonGroups[groupIndex];

            if (group == null)
            {
                continue;
            }

            foreach (Button button in group.buttons)
            {
                if (button == null || _registeredListeners.ContainsKey(button))
                {
                    continue;
                }

                int capturedGroupIndex = groupIndex;
                Button capturedButton = button;
                UnityAction handler = () => HandleButtonClick(capturedGroupIndex, capturedButton);

                button.onClick.AddListener(handler);
                _registeredListeners.Add(capturedButton, handler);
            }
        }
    }

    private void InitializeGroups()
    {
        for (int groupIndex = 0; groupIndex < buttonGroups.Count; groupIndex++)
        {
            ButtonGroupConfig group = buttonGroups[groupIndex];

            if (group == null || group.buttons.Count == 0)
            {
                continue;
            }

            Button defaultButton = group.selectFirstOnStart ? group.buttons[0] : null;

            for (int buttonIndex = 0; buttonIndex < group.buttons.Count; buttonIndex++)
            {
                Button button = group.buttons[buttonIndex];

                if (button == null)
                {
                    continue;
                }

                bool isSelected = button == defaultButton;
                ApplyButtonVisualState(group, button, isSelected);
                UpdateArrowIndicator(group, buttonIndex, isSelected);
            }
        }
    }

    private void HandleButtonClick(int groupIndex, Button clickedButton)
    {
        if (groupIndex < 0 || groupIndex >= buttonGroups.Count)
        {
            return;
        }

        ButtonGroupConfig group = buttonGroups[groupIndex];

        if (group == null)
        {
            return;
        }

        for (int buttonIndex = 0; buttonIndex < group.buttons.Count; buttonIndex++)
        {
            Button button = group.buttons[buttonIndex];

            if (button == null)
            {
                continue;
            }

            bool isSelected = button == clickedButton;
            ApplyButtonVisualState(group, button, isSelected);
            UpdateArrowIndicator(group, buttonIndex, isSelected);
        }

        if (group.centerSelectedButton)
        {
            if (scrollRect == null)
            {
                Debug.LogWarning($"[{nameof(ButtonScaleAndImageManager)}] ScrollRect is not assigned but 'Center Selected Button' is enabled for group '{group.groupName}'.");
            }
            else
            {
                CenterButtonInView(clickedButton);
            }
        }
    }

    private static void ApplyButtonVisualState(ButtonGroupConfig group, Button button, bool isSelected)
    {
        Image buttonImage = button.GetComponent<Image>();

        if (buttonImage != null)
        {
            Sprite targetSprite = isSelected ? group.selectedSprite : group.normalSprite;

            if (targetSprite != null)
            {
                buttonImage.sprite = targetSprite;
            }
        }

        if (group.applyScale)
        {
            float targetScale = isSelected ? group.selectedScale : group.normalScale;
            button.transform.localScale = Vector3.one * targetScale;
        }
    }

    private void UpdateArrowIndicator(ButtonGroupConfig group, int buttonIndex, bool isSelected)
    {
        if (!group.toggleArrowIndicators)
        {
            return;
        }

        if (buttonIndex >= group.arrowIndicators.Count)
        {
            Debug.LogWarning($"[{nameof(ButtonScaleAndImageManager)}] Arrow indicator count does not match button count in group '{group.groupName}'.");
            return;
        }

        Image arrow = group.arrowIndicators[buttonIndex];

        if (arrow != null)
        {
            arrow.gameObject.SetActive(isSelected);
        }
    }

    private void CenterButtonInView(Button button)
    {
        if (scrollRect == null)
        {
            return;
        }

        RectTransform contentRect = scrollRect.content;
        RectTransform viewportRect = scrollRect.viewport != null ? scrollRect.viewport : scrollRect.GetComponent<RectTransform>();
        RectTransform buttonRect = button != null ? button.GetComponent<RectTransform>() : null;

        if (contentRect == null || viewportRect == null || buttonRect == null)
        {
            return;
        }

        float contentWidth = contentRect.rect.width;
        float viewportWidth = viewportRect.rect.width;

        if (contentWidth <= viewportWidth)
        {
            contentRect.anchoredPosition = new Vector2(0f, contentRect.anchoredPosition.y);
            return;
        }

        Vector2 buttonLocalPosition = contentRect.InverseTransformPoint(buttonRect.transform.position);
        float buttonCenterLocalX = buttonLocalPosition.x;
        float halfViewportWidth = viewportWidth * 0.5f;

        float clampedPosition = Mathf.Clamp(buttonCenterLocalX - halfViewportWidth, 0f, contentWidth - viewportWidth);

        Vector2 currentAnchoredPosition = contentRect.anchoredPosition;
        currentAnchoredPosition.x = -clampedPosition;
        contentRect.anchoredPosition = currentAnchoredPosition;
    }
}
