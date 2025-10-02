using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ScriptManager : MonoBehaviour
{
    public Transform capsuleTransform; // 移動させたいCapsuleのTransform
    public GameObject characterObject; // キャラクターのゲームオブジェクト

    public ShowTooltip showTooltip;
    public ClickToMoveAndAvoidObstacles clickToMoveAndAvoidObstacles; // 無効にしたいスクリプト1
    public NavMeshAgent navMeshAgent; // NavMeshAgent コンポーネント
    public LookAtTargetOffset lookAtTargetOffset;
    public FPSController fpsController;


    // Start is called before the first frame update
    void Start()
    {
        // コンポーネントを characterObject から取得する
        clickToMoveAndAvoidObstacles = characterObject.GetComponent<ClickToMoveAndAvoidObstacles>();
        navMeshAgent = characterObject.GetComponent<NavMeshAgent>();
        lookAtTargetOffset = characterObject.GetComponent<LookAtTargetOffset>();

        fpsController = capsuleTransform.GetComponent<FPSController>();
    }


    public void MouseMode(bool mouseMode)
    {
        if (fpsController != null)
        {
            fpsController.IsFps = !mouseMode;
        }
    }
    public void SituationScripts()
    {
        if (clickToMoveAndAvoidObstacles != null)
        {
            clickToMoveAndAvoidObstacles.enabled = false;
        }
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }
        if (fpsController != null)
        {
            fpsController.IsFps = false;
        }
    }
    public void MoveScripts()
    {
        if (showTooltip != null)
        {
            showTooltip.enabled = true;
        }
        if (clickToMoveAndAvoidObstacles != null)
        {
            clickToMoveAndAvoidObstacles.enabled = true;
        }
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;
        }
        if (fpsController != null)
        {
            fpsController.IsFps = true;
        }
        showTooltip.returnCanvas();
    }
}
