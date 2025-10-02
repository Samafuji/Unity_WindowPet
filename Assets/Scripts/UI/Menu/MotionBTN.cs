using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMotionController : MonoBehaviour
{
    public Animator animator;  // キャラクターにアタッチされたAnimatorコンポーネント

    public List<Button> buttons;
    public Button motionResetButton;

    public List<int> poseIndex = new List<int> { 50010, 50020, 50030, 50040, 50050, 50060, 50070, 50080 };
    public int resetPoseIndex = -1;

    void Start()
    {
        // 各ボタンにクリックイベントを登録
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;  // Local copy of the loop variable
            buttons[i].onClick.AddListener(() => TriggerMotion(poseIndex[index]));
        }
        motionResetButton.onClick.AddListener(() => TriggerMotion(resetPoseIndex));
    }

    void ResetParameters()
    {
        animator.SetBool("Walk", false);
        animator.SetBool("Pose", false);
        animator.SetFloat("Blend", 0);
        animator.SetInteger("PoseIndex", resetPoseIndex);  // デフォルト値にリセット
    }

    void TriggerMotion(int poseIndex)
    {
        ResetParameters();
        animator.SetInteger("PoseIndex", poseIndex);
    }
}
