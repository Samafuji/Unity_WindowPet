using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelController : MonoBehaviour
{
    public GameObject SettingPanel;  // 表示・非表示を切り替えるパネル
    public Button toggleButton;  // 表示・非表示を切り替えるボタン

    public Button spawnButton; // ボタンの参照
    public GameObject phonePrefab; // phoneオブジェクトのPrefab
    public Transform cameraTransform; // カメラのTransform

    private GameObject spawnedPhone; // 生成されたphoneオブジェクトの参照

    void Start()
    {
        SettingPanel.SetActive(false);
        // ボタンのクリックイベントにリスナーを追加
        toggleButton.onClick.AddListener(TogglePanel);
        spawnButton.onClick.AddListener(SpawnOrDestroyPhone);
    }

    void Update()
    {
        // ESCキーが押されたらパネルを非表示にする
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     TogglePanel();
        // }
        // // imgPanelがアクティブな場合にパネル外をクリックしたときに非表示にする
        // if (imgPanel.activeSelf && Input.GetMouseButtonDown(0))
        // {
        //     if (!IsPointerOverUIObject(imgPanel))
        //     {
        //         imgPanel.SetActive(false);
        //     }
        // }
    }

    public void TogglePanel()
    {
        // パネルのアクティブ状態を切り替える
        SettingPanel.SetActive(!SettingPanel.activeSelf);
    }

    public void ExitGame()
    {
        // 退出游戏
        Application.Quit();
    }

    public void SpawnOrDestroyPhone()
    {
        if (spawnedPhone == null)
        {
            Vector3 spawnPosition = cameraTransform.position + cameraTransform.forward * 0.125f; // カメラの前に出現させる位置
            Quaternion spawnRotation = cameraTransform.rotation * Quaternion.Euler(0, 180, 180); ; // カメラの向きに合わせる
            spawnedPhone = Instantiate(phonePrefab, spawnPosition, spawnRotation); // オブジェクトを生成
        }
        else
        {
            Destroy(spawnedPhone); // 既に存在する場合は削除
            spawnedPhone = null;
        }
    }

    public bool IsPointerOverUIObject(GameObject panel)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject == panel)
            {
                return true;
            }
        }
        return false;
    }
}
