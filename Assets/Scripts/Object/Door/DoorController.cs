using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Transform doorTransform;  // ドアのTransform
    [SerializeField] private float openAngle = 90f;    // ドアが開く角度
    [SerializeField] private float closeAngle = 0f;    // ドアが閉じた角度
    [SerializeField] private float openSpeed = 2f;     // ドアが開く速さ
    [SerializeField] private bool isOpen = false;      // ドアの開閉状態

    private Quaternion openRotation;                   // ドアが開いた状態の回転
    private Quaternion closeRotation;                  // ドアが閉じた状態の回転

    // プレイヤーがインタラクトできる範囲かどうか
    [SerializeField] private float interactDistance = 2f;  // インタラクトできる距離
    private GameObject player;                              // プレイヤーの参照

    void Start()
    {
        // ドアの回転の初期設定
        closeRotation = Quaternion.Euler(0, closeAngle, 0);
        openRotation = Quaternion.Euler(0, openAngle, 0);

        // プレイヤーの参照取得
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        // プレイヤーとの距離を計算
        if (Vector3.Distance(player.transform.position, transform.position) < interactDistance)
        {
            // プレイヤーがEキーを押したときに開閉をトグル
            if (Input.GetKeyDown(KeyCode.F))
            {
                ToggleDoor();
            }
        }

        // ドアの回転を更新（スムーズに開閉するよう補間）
        if (isOpen)
        {
            doorTransform.rotation = Quaternion.Slerp(doorTransform.rotation, openRotation, Time.deltaTime * openSpeed);
        }
        else
        {
            doorTransform.rotation = Quaternion.Slerp(doorTransform.rotation, closeRotation, Time.deltaTime * openSpeed);
        }
    }

    // ドアの開閉をトグルするメソッド
    private void ToggleDoor()
    {
        isOpen = !isOpen;
    }

    // プレイヤーが範囲内にいるかの視覚的なデバッグ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
