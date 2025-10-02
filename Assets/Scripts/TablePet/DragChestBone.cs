using System.Collections.Generic;
using UnityEngine;

public class DragChestBones : MonoBehaviour
{
    // 胸のボーンリスト（左胸と右胸）
    [SerializeField] private List<Transform> _chestBones;

    // 回転速度
    [SerializeField, Range(0.1f, 10f)] private float _rotationSpeed = 2f;

    // スプリングの戻り速度
    [SerializeField, Range(0.00001f, 1f)] private float _springReturnSpeed = 0.1f;

    // マウスの初期位置
    private Vector3 _initialMousePosition;

    // 選択された胸のボーン
    private Transform _selectedBone = null;

    // ボーンの初期回転
    private Quaternion _initialRotation;

    // ドラッグ中かどうかのフラグ
    private bool _isDragging = false;

    // ドラッグ終了時の目標回転
    private Quaternion _targetRotation;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Raycastを使ってクリックしたオブジェクトを判定
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // クリックされたボーンがリストに含まれているかチェック
                if (_chestBones.Contains(hit.transform))
                {
                    // ドラッグ開始
                    _isDragging = true;
                    _selectedBone = hit.transform;
                    _initialMousePosition = Input.mousePosition;
                    _initialRotation = _selectedBone.rotation;
                }
            }
        }

        if (_isDragging)
        {
            if (Input.GetMouseButton(0))
            {
                // マウスの移動量を計算
                Vector3 mouseDelta = Input.mousePosition - _initialMousePosition;

                // Y軸の回転（左右のドラッグで胸のボーンを回転）
                float rotationY = -mouseDelta.x * _rotationSpeed;

                // X軸の回転（上下のドラッグで胸のボーンを回転）
                float rotationX = mouseDelta.y * _rotationSpeed;

                // 各軸の回転を個別に適用
                Quaternion rotationXQuat = Quaternion.AngleAxis(rotationX, _selectedBone.right);
                Quaternion rotationYQuat = Quaternion.AngleAxis(rotationY, _selectedBone.forward);

                // 新しい回転を計算（X軸とY軸を個別に適用）
                Quaternion newRotation = rotationYQuat * rotationXQuat * _initialRotation;

                // 選択されたボーンに回転を適用
                _selectedBone.rotation = newRotation;
            }

            // マウスボタンを離したらドラッグ終了
            if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
                _targetRotation = _initialRotation; // 元の回転を目標に設定
            }
        }

        if (!_isDragging && _selectedBone != null)
        {
            // スプリングのように元の位置に戻る
            _selectedBone.rotation = Quaternion.Slerp(_selectedBone.rotation, _targetRotation, _springReturnSpeed);

            // ほぼ元の回転に戻ったら、選択解除
            if (Quaternion.Angle(_selectedBone.rotation, _targetRotation) < 0.1f)
            {
                _selectedBone = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_chestBones != null)
        {
            Gizmos.color = Color.blue;
            foreach (var bone in _chestBones)
            {
                if (bone != null)
                {
                    // 各胸のボーンの方向を視覚化
                    Gizmos.DrawLine(bone.position, bone.position + bone.forward * 2.0f);
                }
            }
        }
    }
}
