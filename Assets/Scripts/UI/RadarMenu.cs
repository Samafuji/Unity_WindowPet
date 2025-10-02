using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RadarMenu : MonoBehaviour
{
    public GameObject Chara;
    private Animator animator;  // CharaにアタッチされたAnimatorコンポーネント
    public float speed = 5.0f; // キャラクターの移動速度
    public bool isMoving = false; // キャラクターが移動中かどうか

    private bool MainOrBreak = false;
    private Vector3 targetPositionToMain = new Vector3(4.67f, -2.45f, -6.24f);
    private Vector3 targetPositionToBreak = new Vector3(7.360677f, -8.616814f, -6.24f);
    private Vector3 RotatePosition = new Vector3(50.0f, -2.99f, -6.24f);

    public Canvas canvas;
    public GameObject uiCanvas; // レーダーパネル
    public GameObject radarPanel; // レーダーパネル
    public Button[] buttons; // レーダーの各ボタン

    private bool isRadarActive = false;
    private Vector3 initialMousePosition;

    void Start()
    {
        // CharaからAnimatorコンポーネントを取得
        animator = Chara.GetComponent<Animator>();
        radarPanel.SetActive(false);
        buttons[0].onClick.AddListener(OpenPanelBTN);
        buttons[3].onClick.AddListener(TakeBreakBTN);
    }
    private void Update()
    {

    }

    public void KeyDown(MouseKey mouseKey, Vector3 mousePos)
    {
        if (mouseKey == MouseKey.Right)
        {
            isRadarActive = true;
            radarPanel.SetActive(true);
            initialMousePosition = mousePos;

            float planeDistance = canvas.planeDistance;
            var refPos = new Vector3(mousePos.x, mousePos.y, planeDistance);
            var pos = Camera.main.ScreenToWorldPoint(refPos);
            // pos = Chara.transform.position;
            // Debug.Log(refPos);
            radarPanel.transform.position = new Vector3(refPos.x, refPos.y, -8f);
            // radarPanel.transform.position = refPos;
        }
    }

    public void KeyUp(MouseKey mouseKey, Vector3 mousePos)
    {
        if (mouseKey == MouseKey.Right && isRadarActive)
        {
            isRadarActive = false;
            radarPanel.SetActive(false);
            SelectButton();
        }
    }

    public void Drag(MouseKey mouseKey, Vector3 mousePos)
    {
        if (mouseKey == MouseKey.Right && isRadarActive)
        {
            UpdateButtonHighlight(mousePos);
        }
    }
    void UpdateButtonHighlight(Vector3 mousePos)
    {
        Vector3 direction = mousePos - initialMousePosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }

        if (angle > 0 && angle <= 60)
        {
            buttons[5].interactable = true;
        }
        else if (angle > 60 && angle <= 120)
        {
            buttons[0].interactable = true;
        }
        else if (angle > 120 && angle <= 180)
        {
            buttons[1].interactable = true;
        }

        else if (angle > -180 && angle <= -120)
        {
            buttons[2].interactable = true;
        }
        else if (angle > -120 && angle <= -60)
        {
            buttons[3].interactable = true;
        }
        else if (angle > -60 && angle <= 0)
        {
            buttons[4].interactable = true;
        }
        // Debug.Log("angle: " + angle);
    }

    void SelectButton()
    {
        foreach (Button button in buttons)
        {
            if (button.interactable)
            {
                button.onClick.Invoke();
                break;
            }
        }
    }



    ///BTN Functions
    void OpenPanelBTN()
    {
        uiCanvas.SetActive(!uiCanvas.activeSelf);
        animator.SetBool("Show", uiCanvas.activeSelf);
        UpdateUiPosition();
        Debug.Log("OpenPanelBTN");
    }

    void TakeBreakBTN()
    {
        if (Vector3.Distance(Chara.transform.position, targetPositionToBreak) < 0.1f)
        {
            MainOrBreak = true;
            animator.SetBool("Jump", true);
        }
        else
        {
            MainOrBreak = false;
            animator.SetBool("Jump", false);
        }
        uiCanvas.SetActive(false);
        animator.SetBool("Show", false);
        isMoving = true;
        Debug.Log("TakeBreakBTN");
    }

    public void MoveCharacter()
    {
        float step = speed * Time.deltaTime;

        if (MainOrBreak)
        {
            Chara.transform.position = Vector3.MoveTowards(Chara.transform.position, new Vector3(7.33f, -1.49f, -6.24f), step * 3);

            if (Vector3.Distance(Chara.transform.position, new Vector3(7.33f, -1.49f, -6.24f)) < 0.1f)
            {
                // if (animator.GetBool("Walk"))
                // {
                //     // Debug.Log("NootWalk");
                //     animator.SetBool("Walk", false);
                //     animator.SetBool("Break", false);
                //     animator.SetBool("Jump", false);
                // }

                animator.SetBool("Walk", false);
                animator.SetBool("Break", false);
                animator.SetBool("Jump", false);
                // Chara.transform.rotation = Quaternion.Euler(0, 180, 0);
                isMoving = false;
                MainOrBreak = false;
            }
            // else
            // {
            //     Chara.transform.rotation = Quaternion.Euler(0, 90, 0);
            //     if (!animator.GetBool("Walk"))
            //     {
            //         // Debug.Log("Walk");
            //         animator.SetBool("Walk", true);
            //     }
            //     RotateTowards(RotatePosition);
            // }
        }
        // if (!MainOrBreak)
        else
        {
            Chara.transform.position = Vector3.MoveTowards(Chara.transform.position, targetPositionToBreak, step);

            if (Vector3.Distance(Chara.transform.position, targetPositionToBreak) < 0.1f)
            {
                if (animator.GetBool("Walk"))
                {
                    // Debug.Log("NootWalk");
                    animator.SetBool("Walk", false);
                    animator.SetBool("Break", true);
                }

                // Chara.transform.rotation = Quaternion.Euler(0, 180, 0);
                isMoving = false;
                MainOrBreak = true;
            }
            else
            {
                // Chara.transform.rotation = Quaternion.Euler(0, 90, 0);
                if (!animator.GetBool("Walk"))
                {
                    // Debug.Log("Walk");
                    animator.SetBool("Walk", true);
                }
                // RotateTowards(RotatePosition);
            }
        }
    }

    // 指定された位置に向かって回転するメソッド
    void RotateTowards(Vector3 target)
    {
        Vector3 direction = (target - Chara.transform.position).normalized;
        direction.y = 0; // Y軸の回転を無視
        if (direction == Vector3.zero) return; // 方向ベクトルがゼロの場合は回転しない

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Chara.transform.rotation = Quaternion.Slerp(Chara.transform.rotation, lookRotation, Time.deltaTime * speed);
    }
    private void UpdateUiPosition()
    {
        Vector3 uiPosition;
        // 将UI位置设置到对象的右边
        if (transform.position.x > 0)
        {
            uiPosition = Chara.transform.position + new Vector3(-5.5f, 3.2f, 0); // 这里可以调整偏移量
            // Debug.Log(transform.position);
        }
        else
        {
            uiPosition = Chara.transform.position + new Vector3(5.5f, 3.2f, 0); // 这里可以调整偏移量
            uiCanvas.transform.position = uiPosition;
        }
        uiCanvas.transform.position = uiPosition;
    }
}
