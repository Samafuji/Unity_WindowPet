using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public FPSController fpsController;
    public PanelController panelController;
    public SettingController settingController;
    public ClotheController clotheController;
    public ScriptManager scriptManager;
    public ShowTooltip showTooltip;
    public GameObject settingPanel; // 設定パネルをここで参照

    void Update()
    {
        // ESCキーが押されたときの挙動
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingPanel.activeSelf)
            {
                // 設定パネルが開いているときはそれだけを閉じる
                settingPanel.SetActive(false);
                Debug.Log("settingPanel:CLose");
            }
            else if (showTooltip.Communicate.activeSelf)
            {
                showTooltip.Communicate.SetActive(false);
                scriptManager.MouseMode(false);
                scriptManager.clickToMoveAndAvoidObstacles.enabled = true;
            }
            else if (clotheController.ClothePanel.activeSelf || clotheController.ClothePanelYukata.activeSelf)
            {
                // 通常のパネルが開いている場合はそれを閉じてFPSのカーソルをロック
                // panelController.TogglePanel(false); // パネルを非表示にするメソッドを呼び出す
                // clotheController.TogglePanel();
                clotheController.ClothePanel.SetActive(false);
                clotheController.ClothePanelYukata.SetActive(false);
                Debug.Log("ClothePanel:CLose");
            }
            else if (panelController.SettingPanel.activeSelf)
            {
                // 通常のパネルが開いている場合はそれを閉じてFPSのカーソルをロック
                // panelController.TogglePanel(false); // パネルを非表示にするメソッドを呼び出す
                panelController.TogglePanel(); // パネルを非表示にするメソッドを呼び出す
                fpsController.SetCursorLock(true); // FPSのカーソルロックを有効にする
                Debug.Log("PhonePanel:CLose");
            }
            else
            {
                // どのパネルも開いていない場合はFPSのカーソルロックを切り替える
                panelController.TogglePanel(); // パネルを非表示にするメソッドを呼び出す
                fpsController.ToggleCursorLock();
                Debug.Log("Setting:open");
            }
        }

        // パネル外がクリックされた場合の挙動
        if (panelController.SettingPanel.activeSelf && !settingController.imgPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (!panelController.IsPointerOverUIObject(panelController.SettingPanel))
            {
                panelController.TogglePanel(); // パネルを非表示にする
                fpsController.SetCursorLock(true); // FPSのカーソルロックを有効にする
                if (clotheController.ClothePanel.activeSelf || clotheController.ClothePanelYukata.activeSelf)
                {
                    clotheController.ClothePanel.SetActive(false);
                    clotheController.ClothePanelYukata.SetActive(false);
                }
            }
        }
    }
}
