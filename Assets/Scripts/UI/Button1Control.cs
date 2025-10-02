using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainButton : MonoBehaviour
{
    public List<Button> buttons;
    public List<GameObject> panels;
    public List<GameObject> panelsMenu;
    public Button additionalButton;
    public GameObject additionalPanel;
    public GameObject overlayPanel; // 遮罩层
    public Button backButton;
    public Button exitButton;

    private void Start()
    {
        // 检查按钮和面板的数量是否一致
        if (buttons.Count != panels.Count)
        {
            Debug.LogError("按钮和面板的数量不匹配！");
            return;
        }

        // 为每个按钮添加点击事件监听器
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i; // 本地变量用于捕获当前索引
            buttons[index].onClick.AddListener(() => TogglePanel(index));
        }

        // 确保所有面板初始为隐藏状态
        for (int i = 1; i < panels.Count; i++)
        {
            SetPanelActive(panels[i], false);
            SetPanelActive(panelsMenu[i], false);
        }

        // 确保额外的面板和遮罩层初始为隐藏状态
        SetPanelActive(additionalPanel, false);
        overlayPanel.SetActive(false);

        // 为额外的按钮添加点击事件监听器
        additionalButton.onClick.AddListener(ToggleAdditionalPanel);

        // 为返回和退出按钮添加点击事件监听器
        backButton.onClick.AddListener(HideOverlay);
        exitButton.onClick.AddListener(ExitGame);
    }

    public void TogglePanel(int index)
    {
        // 隐藏所有面板
        for (int i = 0; i < panels.Count; i++)
        {
            SetPanelActive(panels[i], false);
            SetPanelActive(panelsMenu[i], false);
        }

        // 显示被点击按钮对应的面板
        SetPanelActive(panels[index], true);
        SetPanelActive(panelsMenu[index], true);

        // 隐藏遮罩层
        overlayPanel.SetActive(false);
    }

    public void ToggleAdditionalPanel()
    {
        // 切换额外面板的显示状态
        bool isActive = additionalPanel.activeSelf;
        SetPanelActive(additionalPanel, !isActive);

        // 切换遮罩层的显示状态
        overlayPanel.SetActive(!isActive);
    }

    private void HideOverlay()
    {
        // 隐藏遮罩层和额外面板
        overlayPanel.SetActive(false);
        SetPanelActive(additionalPanel, false);
    }

    private void ExitGame()
    {
        // 退出游戏
        Application.Quit();
    }

    private void SetPanelActive(GameObject panel, bool isActive)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = isActive ? 1 : 0;
            canvasGroup.interactable = isActive;
            canvasGroup.blocksRaycasts = isActive;
        }
        else
        {
            panel.SetActive(isActive);
        }
    }
}
