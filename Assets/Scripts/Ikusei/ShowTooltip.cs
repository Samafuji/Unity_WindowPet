using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;

public class ShowTooltip : MonoBehaviour
{
    public GameObject tooltipImage;
    public GameObject HoverChara;

    public GameObject contact;
    public GameObject Communicate;

    public Vector2 offset = new Vector2(10f, 10f); // Offset for tooltip position
    private bool isMouseOver = false;
    private bool isMouseOverChara = false;
    private RectTransform tooltipRectTransform;
    private RectTransform HoverCharaRectTransform;

    public Animator curtainAnimator; // 布のアニメーター

    public Canvas currentCanvas;
    public Canvas CanvasCommunication;


    public Transform capsuleTransform; // 移動させたいCapsuleのTransform
    public GameObject characterObject; // キャラクターのゲームオブジェクト
    [SerializeField, Range(0.01f, 5f)] private float detectionRadius = 3.3f; // The distance within which clicks are allowed

    public ScriptManager scriptManager; // 無効にしたいスクリプト1

    public RuntimeAnimatorController newAnimatorController; // 新しいアニメーターコントローラーをアサインする


    private void Start()
    {

        if (tooltipImage != null)
        {
            tooltipRectTransform = tooltipImage.GetComponent<RectTransform>();
            tooltipImage.SetActive(false); // Tooltip hidden by default
        }
        CanvasCommunication.gameObject.SetActive(false);
        // if (HoverChara != null)
        // {
        //     HoverCharaRectTransform = HoverChara.GetComponent<RectTransform>();
        //     HoverChara.SetActive(false); // Tooltip hidden by default
        // }

        // HoverCharaの子オブジェクトからcontactとCommunicateを取得
        contact = HoverChara.transform.Find("contact").gameObject;
        Communicate = HoverChara.transform.Find("Communicate").gameObject;

    }

    private void Update()
    {
        CheckMouseOver();

        if (isMouseOver)
        {
            UpdateTooltipPosition();
        }
    }

    private void CheckMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the object hit has the PosiToMove script
            PosiToMove posiToMove = hit.collider.GetComponentInParent<PosiToMove>();

            if (posiToMove != null)
            {
                ShowTooltipImage(true);
                isMouseOver = true;

                // Handling the left-click action
                if (Input.GetMouseButtonDown(0))
                {
                    // PerformAction(posiToMove);
                }
            }
            else
            {
                ShowTooltipImage(false);
                isMouseOver = false;
            }


            // Check if the clicked object is a child of the characterObject and within the detection radius
            if (hit.collider != null && hit.collider.transform.IsChildOf(characterObject.transform))
            {
                float distance = Vector3.Distance(capsuleTransform.position, characterObject.transform.position);

                if (distance <= detectionRadius)
                {
                    if (!Communicate.activeSelf)
                    {
                        contact.SetActive(true);
                    }
                    HandleColliderClick(hit.collider);

                    // Handling the left-click action) && isMouseOverChara
                    if (Input.GetKey(KeyCode.F))
                    {
                        // PerformActionFrontChara();
                        Communicate.SetActive(true);
                        scriptManager.clickToMoveAndAvoidObstacles.enabled = false;
                        scriptManager.MouseMode(true);
                        contact.SetActive(false);
                    }
                }
                else
                {
                    // Debug.Log("Character is too far away for interaction. : " + distance);
                    contact.SetActive(false);
                    isMouseOverChara = false;
                }
            }
            else
            {
                contact.SetActive(false);
                isMouseOverChara = false;
            }

        }
        else
        {
            contact.SetActive(false);
            ShowTooltipImage(false);
            isMouseOver = false;
        }
    }

    private void ShowTooltipImage(bool show)
    {
        if (tooltipImage != null)
        {
            tooltipImage.SetActive(show);
        }
    }

    private void UpdateTooltipPosition()
    {
        if (tooltipImage != null)
        {
            Vector2 mousePosition = Input.mousePosition;
            tooltipRectTransform.position = mousePosition + offset;
        }
    }

    private void PerformAction(PosiToMove posiToMove)
    {
        Debug.Log("Object clicked!");
        scriptManager.clickToMoveAndAvoidObstacles.SetPosition(posiToMove.newPosition);

        // オブジェクトに関連した他の処理
        StartCoroutine(TriggerEffectAndMoveCapsule(posiToMove));


        // Hide the tooltip
        ShowTooltipImage(false);

        // Disable this script to prevent further interactions
        this.enabled = false;
    }
    public void PerformActionFrontChara()
    {
        Debug.Log("Object clicked!");

        // clickToMoveAndAvoidObstacles.SetPosition(posiToMove.newPosition);

        Communicate.SetActive(false);

        // オブジェクトに関連した他の処理
        StartCoroutine(TriggerEffectFrontChara());

        contact.SetActive(false);

        // Disable this script to prevent further interactions
        this.enabled = false;
    }

    private IEnumerator TriggerEffectAndMoveCapsule(PosiToMove posiToMove)
    {
        // 1秒待つ
        yield return new WaitForSeconds(1f);

        // 布のアニメーションをトリガー
        if (curtainAnimator != null)
        {
            curtainAnimator.SetTrigger("Drop");
        }

        // 布のアニメーションが終わるまで待つ
        yield return new WaitForSeconds(2f);

        // Capsuleの位置を新しい位置に変更
        // if (capsuleTransform != null)
        // {
        //     capsuleTransform.position = posiToMove.newPosition;
        //     capsuleTransform.rotation = Quaternion.Euler(posiToMove.newRotation);
        // }

        // UIを切り替える
        SwitchCanvas();

        // スクリプトの有効/無効を切り替える
        scriptManager.SituationScripts();

        // アニメーターの切り替え
        if (characterObject != null)
        {
            Animator characterAnimator = characterObject.GetComponent<Animator>();
            if (characterAnimator != null && newAnimatorController != null)
            {
                characterAnimator.runtimeAnimatorController = newAnimatorController;
            }// Ensure the position is updated first
            characterObject.transform.position = new Vector3(39.101f, -2.974f, -11.968f);

            // Apply LookAt, then reset rotation if necessary
            characterObject.transform.LookAt(new Vector3(0f, 0f, 0f));

            // Reset rotation to (0, 0, 0) if needed
            characterObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            Debug.Log("rotation: " + characterObject.transform.rotation.eulerAngles);

        }
    }

    private IEnumerator TriggerEffectFrontChara()
    {
        // 1秒待つ
        yield return new WaitForSeconds(1f);

        // 布のアニメーションをトリガー
        if (curtainAnimator != null)
        {
            curtainAnimator.SetTrigger("Drop");
        }

        // 布のアニメーションが終わるまで待つ
        yield return new WaitForSeconds(2f);

        // Capsuleの位置を新しい位置に変更
        // if (capsuleTransform != null)
        // {
        //     // Calculate the new position 1 unit forward from the character's current position
        //     Vector3 newPosition = characterObject.transform.position + characterObject.transform.forward * 2f;
        //     capsuleTransform.position = newPosition;
        //     Debug.Log(newPosition);

        //     // Calculate the rotation by adding 180 degrees to the y-axis
        //     Quaternion newRotation = characterObject.transform.rotation * Quaternion.Euler(0, 180, 0);
        //     capsuleTransform.rotation = newRotation;
        // }


        // UIを切り替える
        SwitchCanvas();

        // スクリプトの有効/無効を切り替える
        scriptManager.SituationScripts();

        // // アニメーターの切り替え
        // if (characterObject != null)
        // {
        //     Animator characterAnimator = characterObject.GetComponent<Animator>();
        //     if (characterAnimator != null && newAnimatorController != null)
        //     {
        //         characterAnimator.runtimeAnimatorController = newAnimatorController;
        //     }// Ensure the position is updated first
        //     characterObject.transform.position = new Vector3(39.101f, -2.974f, -11.968f);

        //     // Apply LookAt, then reset rotation if necessary
        //     characterObject.transform.LookAt(new Vector3(0f, 0f, 0f));

        //     // Reset rotation to (0, 0, 0) if needed
        //     characterObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        //     Debug.Log("rotation: " + characterObject.transform.rotation.eulerAngles);

        // }
    }

    private void SwitchCanvas()
    {
        if (currentCanvas != null)
        {
            currentCanvas.gameObject.SetActive(false);
        }
        if (CanvasCommunication != null)
        {
            CanvasCommunication.gameObject.SetActive(true);
        }
    }
    public void returnCanvas()
    {
        if (currentCanvas != null)
        {
            currentCanvas.gameObject.SetActive(true);
        }
        if (CanvasCommunication != null)
        {
            CanvasCommunication.gameObject.SetActive(false);
        }
    }

    private void HandleColliderClick(Collider collider)
    {
        switch (collider.name)
        {
            case "000_face01":
                Debug.Log("头部被点击");
                break;
            case "Hand":
                Debug.Log("手部被点击");
                break;
            case "00D_body01":
                Debug.Log("身体被点击");
                break;
            case "Leg":
                Debug.Log("腿部被点击");
                break;
            case "00F_足":
                Debug.Log("脚部被点击");
                break;
            case "LowerLeg_L":
                isMouseOverChara = true;
                break;
            case "LowerLeg_R":
                isMouseOverChara = true;
                break;
            case "UpperLeg_L":
                isMouseOverChara = true;
                break;
            case "UpperLeg_R_":
                isMouseOverChara = true;
                break;
            case "Manuka_tail.001":
                isMouseOverChara = true;
                break;
            case "Manuka_tail.003":
                isMouseOverChara = true;
                break;
            case "Manuka_tail.005":
                isMouseOverChara = true;
                break;
            case "Manuka_hair_root":
                isMouseOverChara = true;
                break;
            case "Breast_L":
                isMouseOverChara = true;
                break;
            case "Breast_R":
                isMouseOverChara = true;
                break;
            default:
                Debug.Log("其他部位被点击");
                break;
        }
    }
}