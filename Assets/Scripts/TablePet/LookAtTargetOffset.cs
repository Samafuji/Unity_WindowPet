using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ターゲットに振り向くスクリプト（オフセット考慮＋回転制限＋制限外回転無効化＋制限外からデフォルトへ戻る＋首ボーンのローカルY軸基準）
/// </summary>
public class LookAtTargetOffset : MonoBehaviour
{
    [SerializeField] private Transform _self;                      // 自身のTransform
    [SerializeField] private Transform _target;                    // ターゲットのTransform
    [SerializeField] private Transform _rootBone;                  // ルートボーンのTransform
    [SerializeField] private Transform _neckBone;                  // 首のボーン（回転基準として使用）
    [SerializeField, Range(0f, 180f)] private float _maxRotationAngle = 52f;  // 回転制限角度（正面方向からの最大許容角度）
    [SerializeField, Range(0.01f, 1f)] private float _rotationSpeed = 0.045f; // 回転のスムーズさ
    [SerializeField] private Transform _animatorDefaultTransform;  // アニメーターのデフォルト回転
    [SerializeField] private Animator _animator;                   // アニメーター

    private Vector3 _forward;                                      // 前方の基準となるローカル空間ベクトル
    private Quaternion _currentRotation;
    private bool _isOutsideLimit = false;

    public bool isStopped = false;                                 // 停止フラグ

    private Vector3 _lastNeckPosition;                             // 前フレームの首の位置
    private Quaternion _lastNeckRotation;                          // 前フレームの首の回転

    private Quaternion StoppedRotation;
    private bool hasMoved = false;
    private void Start()
    {
        // ルートボーンの -x 軸を forward ベクトルとして使用
        _forward = _rootBone.right;

        // 初期回転を保存
        _currentRotation = _self.rotation;
        StoppedRotation = _currentRotation;

        // 首の初期位置と回転を保存
        _lastNeckPosition = _neckBone.position;
        _lastNeckRotation = _neckBone.rotation;
    }

    private void LateUpdate()
    {
        // 停止フラグが立っている場合、現在の回転角度を維持
        if (isStopped)
        {
            _currentRotation = StoppedRotation; // 現在の回転を保存
        }
        else
        {
            // ターゲットへの向きベクトル計算
            var dir = _target.position - _self.position;

            // 首ボーンのローカルY軸を「上方向」として使用
            var neckUp = _neckBone.up;

            // ターゲットの方向への回転を、首ボーンのローカルY軸に基づいて計算
            var lookAtRotation = Quaternion.LookRotation(dir, neckUp);

            // 回転補正（ルートボーンの方向に基づく）
            var offsetRotation = Quaternion.FromToRotation(_forward, Vector3.forward);

            // 最終的な望む回転
            var desiredRotation = lookAtRotation * offsetRotation;

            // 現在の正面方向とターゲット方向の角度を計算
            float angleToTarget = Vector3.Angle(_neckBone.forward, dir);

            // 角度が制限を超えている場合
            if (angleToTarget > _maxRotationAngle)
            {
                // 制限を超えたので、アニメーターのデフォルト回転にゆっくり戻る
                _isOutsideLimit = true;
                Quaternion defaultRotation = _neckBone.rotation;
                _currentRotation = Quaternion.Slerp(_currentRotation, defaultRotation, _rotationSpeed);
            }
            else
            {
                // 制限内に戻ったのでターゲットを向くようにする
                _isOutsideLimit = false;
                _currentRotation = Quaternion.Slerp(_currentRotation, desiredRotation, _rotationSpeed);
            }
            StoppedRotation = _currentRotation;
        }

        // 回転を適用
        _self.rotation = _currentRotation;
        // 現フレームの位置と回転を保存
        _lastNeckPosition = _neckBone.position;
        _lastNeckRotation = _neckBone.rotation;
    }

    private void OnDrawGizmos()
    {
        if (_neckBone != null)
        {
            // 首のボーンのローカルY軸に沿った「上方向」をGizmosで描画
            Gizmos.color = Color.green;
            Vector3 upDirection = _neckBone.up * 2.0f; // 線の長さを調整
            Gizmos.DrawLine(_neckBone.position, _neckBone.position + upDirection);

            // 正面方向を視覚化
            Gizmos.color = Color.yellow;
            Vector3 forward = _neckBone.forward * 2.0f; // 線の長さを調整
            Gizmos.DrawLine(_neckBone.position, _neckBone.position + forward);

            // 制限角度を視覚化
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_neckBone.position, Quaternion.Euler(0, _maxRotationAngle, 0) * _neckBone.forward * 2.0f);
            Gizmos.DrawRay(_neckBone.position, Quaternion.Euler(0, -_maxRotationAngle, 0) * _neckBone.forward * 2.0f);
        }
    }

    public IEnumerator StopAnimation()
    {
        isStopped = false;
        yield return new WaitForSeconds(1f);
        isStopped = true;
    }
}
