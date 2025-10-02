using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPControl : MonoBehaviour
{
    float x, y, z;
    float speed = 0.03f;

    public GameObject cam;
    Quaternion cameraRot, characterRot;
    float Xsensityvity = 3f, Ysensityvity = 3f;

    float zoomSpeed = 2f;

    bool cursorLock = true;

    //変数の宣言(角度の制限用)
    float minX = -90f, maxX = 90f;

    // Start is called before the first frame update
    void Start()
    {
        cameraRot = cam.transform.localRotation;
        characterRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(1)) // 右クリックが押されている間
        {
            float xRot = Input.GetAxis("Mouse X") * Ysensityvity;
            float yRot = Input.GetAxis("Mouse Y") * Xsensityvity;

            cameraRot *= Quaternion.Euler(-yRot, 0, 0);
            characterRot *= Quaternion.Euler(0, xRot, 0);

            //Updateの中で作成した関数を呼ぶ
            cameraRot = ClampRotation(cameraRot);

            cam.transform.localRotation = cameraRot;
            transform.localRotation = characterRot;


            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false; // カーソルを非表示にする
        }

        else if (Input.GetMouseButton(2)) // マウスミドルボタンが押されている間
        {
            x = -Input.GetAxis("Mouse X") * speed;
            y = -Input.GetAxis("Mouse Y") * speed;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false; // カーソルを非表示にする


            transform.position += cam.transform.right * x + cam.transform.up * y;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true; // カーソルを表示する
        }

        Move();


        // マウスホイール入力によるズーム調整
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        transform.position += cam.transform.forward * scrollInput * zoomSpeed;

    }
    public void Move()
    {
        x = 0;
        y = 0;
        z = 0;

        x = Input.GetAxisRaw("Horizontal") * speed;
        z = Input.GetAxisRaw("Vertical") * speed;

        // Eキーを押したときにyをプラス
        if (Input.GetKey(KeyCode.E))
        {
            y += speed;
        }

        // Qキーを押したときにyをマイナス
        if (Input.GetKey(KeyCode.Q))
        {
            y -= speed;
        }

        transform.position += cam.transform.forward * z + cam.transform.right * x + Vector3.up * y;
    }
    //角度制限関数の作成
    public Quaternion ClampRotation(Quaternion q)
    {
        //q = x,y,z,w (x,y,zはベクトル（量と向き）：wはスカラー（座標とは無関係の量）)

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;

        angleX = Mathf.Clamp(angleX, minX, maxX);

        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

        return q;
    }
}
