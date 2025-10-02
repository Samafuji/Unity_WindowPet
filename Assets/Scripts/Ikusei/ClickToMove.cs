using UnityEngine;

public class ClickToMoveAndRotate : MonoBehaviour
{
    public Animator animator;  // キャラクターにアタッチされたAnimatorコンポーネント

    public float speed = 5.0f; // キャラクターの移動速度
    public float rotationSpeed = 10.0f; // キャラクターの回転速度
    public float heightOffset = 0.15f; // クリック位置の高さを調整するオフセット
    public GameObject capsule; // 停止時に向くオブジェクト
    private Vector3 targetPosition; // 移動先の位置
    private bool isMoving = false; // キャラクターが移動中かどうか

    void Update()
    {
        // クリック位置の取得
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                targetPosition = hit.point;
                targetPosition.y += heightOffset; // クリック位置の高さを調整
                isMoving = true;
            }
        }

        // キャラクターの移動
        if (isMoving)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            animator.SetBool("Walk", true);

            // クリック位置に向かって回転
            RotateTowards(targetPosition);

            // 目標位置に到達したら移動を停止し、カプセルの方向に向かって回転
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                animator.SetBool("Walk", false);
                isMoving = false;
                RotateTowards(capsule.transform.position);
            }
        }
    }

    // 指定された位置に向かって回転するメソッド
    void RotateTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0; // Y軸の回転を無視
        if (direction == Vector3.zero) return; // 方向ベクトルがゼロの場合は回転しない

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }
}
