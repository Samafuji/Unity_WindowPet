using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FPSController : MonoBehaviour
{
    public GameObject cam;

    public float HeightminY = 0f;      // Ctrlを押しているときの最小の高さ（ローカル座標）
    private float defaultY;  // 通常時の元の高さ（ローカル座標）
    public float Heightspeed = 1000.0f;     // 高さの移動速度

    private float targetY;

    public Slider slider;
    public TMP_InputField inputField;

    private CharacterController controller;
    private Quaternion cameraRot, characterRot;
    private float sensitivity = 0.459f;


    public float moveSpeed = 3.0f;        // 通常の移動速度
    public float sprintSpeed = 20.0f;      // ダッシュ時の速度
    public float maxSpeed = 5.0f;         // 最大速度制限
    public float jumpPower = 100.0f;         // ジャンプの力
    public float friction = 0.9f;          // 壁に当たったときの摩擦軽減

    private float cameraSpeed = 3.0f;
    private bool isJump = false;
    private bool cursorLock = true;

    public bool IsFps = true;

    // 変数の宣言(角度の制限用)
    float minX = -90f, maxX = 90f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 物理マテリアルの初期化
        PhysicMaterial material = new PhysicMaterial();
        material.frictionCombine = PhysicMaterialCombine.Minimum; // 摩擦を最小限にする
        material.dynamicFriction = 0.0f;
        material.staticFriction = 0.0f;
        GetComponent<Collider>().material = material;
        // シーン全体の重力を増やす
        Physics.gravity = new Vector3(0, -20.0f, 0);  // 通常の約2倍の重力にする

        controller = GetComponent<CharacterController>();
        cameraRot = cam.transform.localRotation;
        characterRot = transform.localRotation;

        UpdateCursorLock();

        if (slider != null)
        {
            slider.value = sensitivity;
            slider.onValueChanged.AddListener(SetSensitivity_Slider);
        }
        if (inputField != null)
        {
            inputField.text = sensitivity.ToString();
            inputField.onEndEdit.AddListener(SetSensitivity_InputField);
        }

        defaultY = cam.transform.localPosition.y;
        targetY = defaultY;
    }

    private void Update()
    {
        UpdateCursorLock();

        if (!IsFps && !Input.GetMouseButton(1))
        {
            return;
        }

        if (cursorLock)
        {
            float xRot = Input.GetAxis("Mouse X") * sensitivity;
            float yRot = Input.GetAxis("Mouse Y") * sensitivity;

            cameraRot *= Quaternion.Euler(-yRot, 0, 0);
            characterRot *= Quaternion.Euler(0, xRot, 0);

            cameraRot = ClampRotation(cameraRot);

            cam.transform.localRotation = cameraRot;
            transform.localRotation = characterRot;
        }

        Transform camTransform = cam.transform;
        Vector3 localPosition = camTransform.localPosition;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            targetY = HeightminY;
        }
        else
        {
            targetY = defaultY;
        }

        localPosition.y = Mathf.MoveTowards(localPosition.y, targetY, Heightspeed * Time.deltaTime);
        camTransform.localPosition = localPosition;
    }
    void FixedUpdate()
    {
        float currentSpeed = moveSpeed;

        // Shiftが押されているときにスピードアップ
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }

        // 現在のY軸速度を保持
        float currentYVelocity = rb.velocity.y;

        // 方向キーによる移動制御
        Vector3 movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            movement += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement -= transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement -= transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += transform.right;
        }

        // 移動速度を適用 (X軸、Z軸のみ)
        Vector3 horizontalVelocity = movement.normalized * currentSpeed;

        // Y軸の速度を保持しながら、最終的な速度を設定
        rb.velocity = new Vector3(horizontalVelocity.x, currentYVelocity, horizontalVelocity.z);


        // 最大速度を制限
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        // // XZ方向の速度を制限する
        // Vector3 velocity = rb.velocity;
        // horizontalVelocity = new Vector3(velocity.x, 0, velocity.z); // Y軸を除外

        // // XZの最大速度を制限
        // horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeed);

        // // 最終的な速度をセット（Y軸速度は制限しない）
        // rb.velocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);

        // ジャンプ処理
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }

        // 摩擦を軽減して壁に当たっても停止しないようにする
        if (rb.velocity.magnitude < 0.1f)
        {
            rb.velocity = rb.velocity * friction;
        }
    }

    private Quaternion ClampRotation(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;
        angleX = Mathf.Clamp(angleX, minX, maxX);
        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

        return q;
    }

    private void UpdateCursorLock()
    {
        if (!IsFps)
        {
            if (Input.GetMouseButton(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            return;
        }
        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ToggleCursorLock()
    {
        cursorLock = !cursorLock;
        UpdateCursorLock();
    }

    public void SetCursorLock(bool lockState)
    {
        cursorLock = lockState;
        Cursor.visible = false; // 非表示に設定
    }

    private void SetSensitivity_Slider(float value)
    {
        sensitivity = value;
        if (inputField != null)
        {
            inputField.text = value.ToString("F3");
        }
    }

    private void SetSensitivity_InputField(string value)
    {
        if (float.TryParse(value, out float result))
        {
            sensitivity = result;
            if (slider != null)
            {
                slider.value = result;
            }
        }
    }
}

// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// public class FPSController : MonoBehaviour
// {
//     public GameObject cam;

//     public float HeightminY = 0f;      // Ctrlを押しているときの最小の高さ（ローカル座標）
//     private float defaultY;  // 通常時の元の高さ（ローカル座標）
//     public float Heightspeed = 1000.0f;     // 高さの移動速度

//     private float targetY;


//     public Slider slider;
//     public TMP_InputField inputField;

//     private CharacterController controller;
//     private Quaternion cameraRot, characterRot;
//     private float sensitivity = 0.459f;
//     private float speed = 3.0f;
//     private bool cursorLock = true;

//     public bool IsFps = true;

//     // 変数の宣言(角度の制限用)
//     float minX = -90f, maxX = 90f;

//     void Start()
//     {
//         controller = GetComponent<CharacterController>();
//         cameraRot = cam.transform.localRotation;
//         characterRot = transform.localRotation;

//         UpdateCursorLock();

//         if (slider != null)
//         {
//             slider.value = sensitivity;
//             slider.onValueChanged.AddListener(SetSensitivity_Slider);
//         }
//         if (inputField != null)
//         {
//             inputField.text = sensitivity.ToString();
//             inputField.onEndEdit.AddListener(SetSensitivity_InputField);
//         }

//         defaultY = cam.transform.localPosition.y;

//         // ターゲットの初期高さをdefaultYに設定
//         targetY = defaultY;
//     }

//     void Update()
//     {
//         UpdateCursorLock();

//         if (!IsFps && !Input.GetMouseButton(1))
//         {
//             return;
//         }

//         if (cursorLock)
//         {
//             float xRot = Input.GetAxis("Mouse X") * sensitivity;
//             float yRot = Input.GetAxis("Mouse Y") * sensitivity;

//             cameraRot *= Quaternion.Euler(-yRot, 0, 0);
//             characterRot *= Quaternion.Euler(0, xRot, 0);

//             cameraRot = ClampRotation(cameraRot);

//             cam.transform.localRotation = cameraRot;
//             transform.localRotation = characterRot;
//         }


//         Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");

//         if (Input.GetKey(KeyCode.E))
//         {
//             move.y += speed * 0.3f;
//         }
//         if (Input.GetKey(KeyCode.Q))
//         {
//             move.y -= speed * 0.3f;
//         }

//         if (Input.GetKey(KeyCode.LeftShift))
//         {
//             move *= 1.7f;
//         }
//         else
//         {
//             move *= 1f;
//         }


//         // CharacterControllerで移動
//         controller.Move(move * speed * Time.deltaTime);


//         // カメラのローカルTransformを取得
//         Transform camTransform = cam.transform;

//         // 現在のカメラのローカル位置を取得
//         Vector3 localPosition = camTransform.localPosition;

//         // Ctrlキーが押されているときに、yをminYにゆっくり移動
//         if (Input.GetKey(KeyCode.LeftControl))
//         {
//             targetY = HeightminY;
//         }
//         else
//         {
//             // Ctrlキーが押されていないときは、yをdefaultYに戻す
//             targetY = defaultY;
//         }

//         // 一次関数的に一定の速度でtargetYに移動
//         localPosition.y = Mathf.MoveTowards(localPosition.y, targetY, Heightspeed * Time.deltaTime);

//         // カメラのローカル位置を更新
//         camTransform.localPosition = localPosition;
//     }

//     private void UpdateCursorLock()
//     {
//         if (!IsFps)
//         {
//             if (Input.GetMouseButton(1))
//             {
//                 Cursor.lockState = CursorLockMode.Locked;
//                 Cursor.visible = false;
//             }
//             else
//             {
//                 Cursor.lockState = CursorLockMode.None;
//                 Cursor.visible = true;
//             }
//             return;
//         }
//         if (cursorLock)
//         {
//             Cursor.lockState = CursorLockMode.Locked;
//             Cursor.visible = false;
//         }
//         else
//         {
//             Cursor.lockState = CursorLockMode.None;
//             Cursor.visible = true;
//         }
//     }

//     public void ToggleCursorLock()
//     {
//         cursorLock = !cursorLock;
//         UpdateCursorLock();
//     }

//     public void SetCursorLock(bool lockState)
//     {
//         cursorLock = lockState;
//         Cursor.visible = false; // 非表示に設定
//     }

//     // 角度制限関数
//     private Quaternion ClampRotation(Quaternion q)
//     {
//         q.x /= q.w;
//         q.y /= q.w;
//         q.z /= q.w;
//         q.w = 1f;

//         float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;
//         angleX = Mathf.Clamp(angleX, minX, maxX);
//         q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

//         return q;
//     }

//     private void SetSensitivity_Slider(float value)
//     {
//         sensitivity = value;
//         if (inputField != null)
//         {
//             inputField.text = value.ToString("F3");
//         }
//     }

//     private void SetSensitivity_InputField(string value)
//     {
//         if (float.TryParse(value, out float result))
//         {
//             sensitivity = result;
//             if (slider != null)
//             {
//                 slider.value = result;
//             }
//         }
//     }
// }
