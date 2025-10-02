using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonScaleAndImageManager : MonoBehaviour
{
    [Header("Group 0 - Sprite Swap")]
    public List<Button> buttons0 = new List<Button>();
    public Sprite selectedSprite0;
    public Sprite normalSprite0;

    [Header("Group 1 - Sprite Swap With Arrows")]
    public List<Button> buttons1 = new List<Button>();
    public List<Image> arrow1 = new List<Image>();
    public Sprite selectedSprite1;
    public Sprite normalSprite1;

    [Header("Group 2 - Sprite Swap With Scaling")]
    public List<Button> buttons20 = new List<Button>();
    public List<Button> buttons21 = new List<Button>();
    public List<Button> buttons22 = new List<Button>();
    public List<Button> buttons23 = new List<Button>();
    public Sprite selectedSprite2;
    public Sprite normalSprite2;
    public float selectedScale2 = 1.13f;
    public float normalScale2 = 1.0f;

    [Header("Group 3 - Sprite Swap, Scaling & Centering")]
    public List<Button> buttons3 = new List<Button>();
    public Sprite selectedSprite3;
    public Sprite normalSprite3;
    public float selectedScale3 = 1.1f;
    public float normalScale3 = 0.7f;
    public ScrollRect scrollRect;

    private readonly List<List<Button>> _buttons2Groups = new List<List<Button>>();
    private readonly Dictionary<Button, UnityAction> _buttonListeners = new Dictionary<Button, UnityAction>();

    private void Awake()
    {
        CacheGroup(buttons20);
        CacheGroup(buttons21);
        CacheGroup(buttons22);
        CacheGroup(buttons23);
    }

    private void OnEnable()
    {
        CacheGroup(buttons20);
        CacheGroup(buttons21);
        CacheGroup(buttons22);
        CacheGroup(buttons23);

        if (!Application.isPlaying)
        {
            ApplyInitialVisualState();
            return;
        }

        RegisterGroup(buttons0, HandleGroup0Click);
        RegisterGroup(buttons1, HandleGroup1Click);

        foreach (List<Button> group in _buttons2Groups)
        {
            List<Button> capturedGroup = group;
            RegisterGroup(capturedGroup, button => HandleGroup2Click(capturedGroup, button));
        }

        RegisterGroup(buttons3, HandleGroup3Click);

        ApplyInitialVisualState();
    }

    private void OnDisable()
    {
        if (!Application.isPlaying)
        {
            _buttonListeners.Clear();
            return;
        }

        foreach (KeyValuePair<Button, UnityAction> entry in _buttonListeners)
        {
            if (entry.Key != null)
            {
                entry.Key.onClick.RemoveListener(entry.Value);
            }
        }

        _buttonListeners.Clear();
    }

    private void CacheGroup(List<Button> group)
    {
        if (group != null && group.Count > 0 && !_buttons2Groups.Contains(group))
        {
            _buttons2Groups.Add(group);
        }
    }

    private void RegisterGroup(List<Button> buttons, UnityAction<Button> onClick)
    {
        if (buttons == null || buttons.Count == 0 || onClick == null)
        {
            return;
        }

        foreach (Button button in buttons)
        {
            if (button == null || _buttonListeners.ContainsKey(button))
            {
                continue;
            }

            Button capturedButton = button;
            UnityAction handler = () => onClick(capturedButton);
            button.onClick.AddListener(handler);
            _buttonListeners.Add(capturedButton, handler);
        }
    }

    private void ApplyInitialVisualState()
    {
        if (buttons0.Count > 0)
        {
            UpdateSpriteGroup(buttons0, buttons0[0], normalSprite0, selectedSprite0);
        }

        if (buttons1.Count > 0)
        {
            UpdateSpriteGroup(buttons1, buttons1[0], normalSprite1, selectedSprite1);
            UpdateArrowIndicators(0);
        }
        else
        {
            UpdateArrowIndicators(-1);
        }

        foreach (List<Button> group in _buttons2Groups)
        {
            if (group.Count > 0)
            {
                UpdateScaledSpriteGroup(group, group[0], normalSprite2, selectedSprite2, normalScale2, selectedScale2);
            }
        }

        if (buttons3.Count > 0)
        {
            UpdateScaledSpriteGroup(buttons3, buttons3[0], normalSprite3, selectedSprite3, normalScale3, selectedScale3);
        }
    }

    private void HandleGroup0Click(Button clickedButton)
    {
        UpdateSpriteGroup(buttons0, clickedButton, normalSprite0, selectedSprite0);
    }

    private void HandleGroup1Click(Button clickedButton)
    {
        int index = buttons1.IndexOf(clickedButton);
        UpdateSpriteGroup(buttons1, clickedButton, normalSprite1, selectedSprite1);
        UpdateArrowIndicators(index);
    }

    private void HandleGroup2Click(List<Button> group, Button clickedButton)
    {
        UpdateScaledSpriteGroup(group, clickedButton, normalSprite2, selectedSprite2, normalScale2, selectedScale2);
    }

    private void HandleGroup3Click(Button clickedButton)
    {
        UpdateScaledSpriteGroup(buttons3, clickedButton, normalSprite3, selectedSprite3, normalScale3, selectedScale3);
        CenterButtonInView(clickedButton);
    }

    private static void UpdateSpriteGroup(List<Button> buttons, Button selectedButton, Sprite normalSprite, Sprite selectedSprite)
    {
        if (buttons == null)
        {
            return;
        }

        foreach (Button button in buttons)
        {
            if (button == null)
            {
                continue;
            }

            Image image = TryGetButtonImage(button);

            if (image == null)
            {
                continue;
            }

            if (button == selectedButton)
            {
                if (selectedSprite != null)
                {
                    image.sprite = selectedSprite;
                }
            }
            else if (normalSprite != null)
            {
                image.sprite = normalSprite;
            }
        }
    }

    private static void UpdateScaledSpriteGroup(List<Button> buttons, Button selectedButton, Sprite normalSprite, Sprite selectedSprite, float normalScale, float selectedScale)
    {
        if (buttons == null)
        {
            return;
        }

        foreach (Button button in buttons)
        {
            if (button == null)
            {
                continue;
            }

            bool isSelected = button == selectedButton;
            UpdateSpriteForState(button, isSelected, normalSprite, selectedSprite);

            float targetScale = isSelected ? selectedScale : normalScale;
            button.transform.localScale = Vector3.one * targetScale;
        }
    }

    private static void UpdateSpriteForState(Button button, bool isSelected, Sprite normalSprite, Sprite selectedSprite)
    {
        Image image = TryGetButtonImage(button);

        if (image == null)
        {
            return;
        }

        if (isSelected)
        {
            if (selectedSprite != null)
            {
                image.sprite = selectedSprite;
            }
        }
        else if (normalSprite != null)
        {
            image.sprite = normalSprite;
        }
    }

    private void UpdateArrowIndicators(int selectedIndex)
    {
        if (arrow1 == null || arrow1.Count == 0)
        {
            return;
        }

        if (selectedIndex < 0 || selectedIndex >= arrow1.Count)
        {
            selectedIndex = -1;
        }

        for (int i = 0; i < arrow1.Count; i++)
        {
            Image arrow = arrow1[i];

            if (arrow == null)
            {
                continue;
            }

            bool shouldBeActive = i == selectedIndex && selectedIndex >= 0;

            if (arrow.gameObject.activeSelf != shouldBeActive)
            {
                arrow.gameObject.SetActive(shouldBeActive);
            }
        }
    }

    private void CenterButtonInView(Button button)
    {
        if (scrollRect == null || button == null)
        {
            return;
        }

        RectTransform contentRect = scrollRect.content;
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        RectTransform viewportRect = scrollRect.viewport != null ? scrollRect.viewport : scrollRect.GetComponent<RectTransform>();

        if (contentRect == null || buttonRect == null || viewportRect == null)
        {
            return;
        }

        float contentWidth = contentRect.rect.width;
        float viewportWidth = viewportRect.rect.width;

        if (contentWidth <= viewportWidth)
        {
            Vector2 anchoredPosition = contentRect.anchoredPosition;
            anchoredPosition.x = 0f;
            contentRect.anchoredPosition = anchoredPosition;
            return;
        }

        Vector2 buttonLocalPosition = contentRect.InverseTransformPoint(buttonRect.position);
        float halfViewportWidth = viewportWidth * 0.5f;
        float targetCenter = Mathf.Clamp(buttonLocalPosition.x, halfViewportWidth, contentWidth - halfViewportWidth);
        float newX = halfViewportWidth - targetCenter;

        Vector2 newAnchoredPosition = contentRect.anchoredPosition;
        newAnchoredPosition.x = newX;
        contentRect.anchoredPosition = newAnchoredPosition;
    }

    private static Image TryGetButtonImage(Button button)
    {
        if (button == null)
        {
            return null;
        }

        Image image = button.targetGraphic as Image;

        if (image == null)
        {
            image = button.GetComponent<Image>();
        }

        return image;
    }
}
