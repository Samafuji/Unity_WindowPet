using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToDraggableObject : MonoBehaviour
{
    private Camera mainCamera;
    public Camera SubCamera;
    private bool isDragging = false;
    private Vector3 offset;
    private Plane dragPlane;
    private Vector3 lastMousePosition;

    void Start()
    {
        mainCamera = Camera.main;
        lastMousePosition = Input.mousePosition;
    }

    void Update()
    {
        // ドラッグ中のオブジェクトの位置を更新
        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            float enter = 0.0f;
            if (dragPlane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                transform.position = hitPoint + offset;

                // ドラッグ量に応じて回転を計算
                Vector3 deltaMouse = Input.mousePosition - lastMousePosition;

                float rotationAmount = deltaMouse.x * 0.03f; // Adjust the sensitivity here 
                SubCamera.transform.Rotate(Vector3.up, rotationAmount, Space.World); // transform.Rotate(Vector3.up, rotationAmount);

                // カメラの右方向ベクトルに基づいて移動量を計算
                // float moveAmount = deltaMouse.x * 0.0007f; // Adjust the sensitivity here
                // Vector3 rightDirection = mainCamera.transform.right;
                // SubCamera.transform.Translate(rightDirection * moveAmount, Space.World);


                lastMousePosition = Input.mousePosition;
            }
        }
    }

    void OnMouseDown()
    {
        // ドラッグの開始
        Debug.Log("OnMouseDown triggered");  // クリックが検出されたことを確認
        isDragging = true;
        dragPlane = new Plane(mainCamera.transform.forward, transform.position);
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        float enter = 0.0f;
        if (dragPlane.Raycast(ray, out enter))
        {
            offset = transform.position - ray.GetPoint(enter);
            Debug.Log($"Clicked on: {gameObject.name} with tag: {gameObject.tag}");
        }
        lastMousePosition = Input.mousePosition;
    }

    void OnMouseUp()
    {
        // ドラッグの終了
        isDragging = false;
    }
}
