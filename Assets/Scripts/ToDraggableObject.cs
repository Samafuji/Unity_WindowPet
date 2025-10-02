using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;
    private Plane dragPlane;

    void Start()
    {
        mainCamera = Camera.main;
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
    }

    void OnMouseUp()
    {
        // ドラッグの終了
        isDragging = false;
    }
}