using UnityEngine;

public class PetMovement : MonoBehaviour
{
    private Animator animator;
    private MyMouse myMouse;
    private bool isDragging = false;
    private bool isDraggingLeft = false;
    private Vector3 offset;
    public GameObject uiCanvas; // UI Canvas对象
    public float rotationSpeed = 5.0f; // 旋转速度

    public RadarMenu radarMenu; // RadarMenu スクリプトへの参照

    private void Awake()
    {
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        // 获取MyMouse组件
        myMouse = FindObjectOfType<MyMouse>();
        // RadarMenu が見つからない場合のエラーメッセージを出力
        if (radarMenu == null)
        {
            Debug.LogError("RadarMenu component not found in the scene.");
        }
        // 订阅事件
        myMouse.MouseKeyDownEvent += OnMouseKeyDown;
        myMouse.MouseKeyUpEvent += OnMouseKeyUp;
        myMouse.MouseDragEvent += OnMouseDrag;
        myMouse.MouseKeyClickEvent += OnMouseClick;

        // 隐藏UI
        uiCanvas.SetActive(false);
    }

    void Update()
    {
        // 使角色始终面向相机
        // FaceCamera();
        if (radarMenu.isMoving)
        {
            radarMenu.MoveCharacter();
            if (isDragging)
            {
                radarMenu.isMoving = false;
                animator.SetBool("Walk", false);
                animator.SetBool("BeDragged", true);
            }
        }
    }

    void OnDestroy()
    {
        // 取消订阅事件
        myMouse.MouseKeyDownEvent -= OnMouseKeyDown;
        myMouse.MouseKeyUpEvent -= OnMouseKeyUp;
        myMouse.MouseDragEvent -= OnMouseDrag;
        myMouse.MouseKeyClickEvent -= OnMouseClick;
    }

    private void OnMouseKeyDown(MouseKey key, Vector3 pos)
    {
        if (key == MouseKey.Left)
        {
            Ray ray = Camera.main.ScreenPointToRay(pos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.transform.IsChildOf(transform)) // 检查点击的是否是当前对象或其子对象
                {
                    isDragging = true;
                    offset = transform.position - myMouse.MousePosToWorldPos(pos);

                }
            }
        }
        if (key == MouseKey.Right)
        {
            Ray ray = Camera.main.ScreenPointToRay(pos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.transform.IsChildOf(transform)) // 检查点击的是否是当前对象或其子对象
                {
                    isDraggingLeft = true;
                    radarMenu.KeyDown(MouseKey.Right, myMouse.MousePosToWorldPos(pos));
                }
            }
        }
    }

    private void OnMouseKeyUp(MouseKey key, Vector3 pos)
    {
        if (key == MouseKey.Left && isDragging)
        {
            isDragging = false;
            animator.SetBool("BeDragged", false);
        }
        if (key == MouseKey.Right)
        {
            radarMenu.KeyUp(MouseKey.Right, transform.position);
        }
    }

    private void OnMouseDrag(MouseKey key, Vector3 pos)
    {
        if (key == MouseKey.Left && isDragging)
        {
            Vector3 worldPos = myMouse.MousePosToWorldPos(pos);
            transform.position = worldPos + offset; // 更新当前对象的位置

            if (!animator.GetBool("BeDragged"))
            {
                // Debug.Log("Draggggggggggggged");
                animator.SetBool("BeDragged", true);
                animator.SetBool("Break", false);
            }

            // 更新UI位置
            if (uiCanvas.activeSelf)
            {
                UpdateUiPosition();
            }
        }
        if (key == MouseKey.Right && isDraggingLeft)
        {
            radarMenu.Drag(MouseKey.Right, myMouse.MousePosToWorldPos(pos));
        }
    }

    private void OnMouseClick(MouseKey key)
    {
        if (key == MouseKey.Left)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.transform.IsChildOf(transform)) // 确认点击的是当前对象或其子对象
                {
                    HandleColliderClick(hit.collider);
                }
            }
        }
    }

    private void HandleColliderClick(Collider collider)
    {
        switch (collider.name)
        {
            case "000_face01":
                // 头部点击的处理逻辑
                Debug.Log("头部被点击");
                break;
            case "Hand":
                // 手部点击的处理逻辑
                Debug.Log("手部被点击");
                break;
            case "00D_body01":
                // 身体点击的处理逻辑
                Debug.Log("身体被点击");
                break;
            case "Leg":
                // 腿部点击的处理逻辑
                Debug.Log("腿部被点击");
                break;
            case "00F_足":
                // 脚部点击的处理逻辑
                Debug.Log("脚部被点击");
                break;
            case "LowerLeg_L" or "LowerLeg_R" or "UpperLeg_L" or "UpperLeg_R_" or "Manuka_tail.001" or "Manuka_tail.003" or "Manuka_tail.005" or "Manuka_hair_root" or "Breast_L" or "Breast_R":
                // 脚部点击的处理逻辑
                Debug.Log("脚部被点击");

                // uiCanvas.SetActive(!uiCanvas.activeSelf);

                // if (uiCanvas.activeSelf)
                // {
                //     UpdateUiPosition();
                // }
                break;
            default:
                Debug.Log("其他部位被点击");
                break;
        }
    }

    private void UpdateUiPosition()
    {
        Vector3 uiPosition;
        // 将UI位置设置到对象的右边
        if (transform.position.x > 0)
        {
            uiPosition = transform.position + new Vector3(-5.5f, 3.2f, 0); // 这里可以调整偏移量
            // Debug.Log(transform.position);
        }
        else
        {
            uiPosition = transform.position + new Vector3(5.5f, 3.2f, 0); // 这里可以调整偏移量
            uiCanvas.transform.position = uiPosition;
        }
        uiCanvas.transform.position = uiPosition;
    }

    private void FaceCamera()
    {
        // 使角色始终面向相机
        Vector3 direction = (Camera.main.transform.position - transform.position).normalized;
        direction.y = 0; // 保持角色的水平朝向

        transform.rotation = Quaternion.LookRotation(direction);
        // Quaternion targetRotation = Quaternion.LookRotation(direction);
        // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
