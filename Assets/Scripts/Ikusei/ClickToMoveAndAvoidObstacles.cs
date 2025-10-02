using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickToMoveAndAvoidObstacles : MonoBehaviour
{
    public Image crosshair; // クロスヘアのUI Image
    public GameObject characterObject; // キャラクターのゲームオブジェクト
    private Animator animator;  // キャラクターにアタッチされたAnimatorコンポーネント
    private NavMeshAgent agent; // NavMeshAgentコンポーネント

    public float rotationSpeed = 10.0f; // キャラクターの回転速度
    public float heightOffset = 0.0f; // クリック位置の高さを調整するオフセット
    public float maxSpeed = 3.0f; // キャラクターの最大速度
    public float acceleration = 8.0f; // キャラクターの加速度
    public GameObject capsule; // 停止時に向くオブジェクト

    private Vector3 tvPosition = new Vector3(31.07652f, -3.870851f, -8.609716f); // 位置をVector3で保持
    private Vector3 bedPosition = new Vector3(37.1434f, -3.870851f, -12.59303f);

    private Vector3 targetPosition; // 現在の目的地
    private bool isMoving = false; // キャラクターが移動中かどうか

    [SerializeField]
    private float waitTime; // 次の目的地に移動するまでの待機時間

    void Start()
    {
        animator = characterObject.GetComponent<Animator>();
        agent = characterObject.GetComponent<NavMeshAgent>();
        agent.speed = maxSpeed; // NavMeshAgentの最大速度を設定
        agent.acceleration = acceleration; // NavMeshAgentの加速度を設定

        // クロスヘアのImageコンポーネントがある場合、Raycast Targetを無効にする
        if (crosshair != null)
        {
            crosshair.raycastTarget = false;
        }
    }
    void Update()
    {
        // クリック位置の取得
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // ヒットしたオブジェクトのタグが"Floor"かどうかを確認
                if (hit.collider.CompareTag("Floor"))
                {
                    Vector3 targetPosition = hit.point;
                    targetPosition.y += heightOffset; // クリック位置の高さを調整
                    agent.SetDestination(targetPosition); // NavMeshAgentに目的地を設定
                }
            }
        }

        // NavMeshAgentが有効かどうかをチェック
        if (agent.enabled && agent.isOnNavMesh)
        {
            // 目標位置に向かって回転
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                animator.SetBool("Walk", true);
                resetAnimation();
                RotateTowards(agent.steeringTarget);
            }
            else
            {
                animator.SetBool("Walk", false);
                // 目標位置に到達したらカプセルの方向に向かって回転
                // 位置によってアニメーションを切り替え
                if (Vector3.Distance(characterObject.transform.position, tvPosition) < 0.1f)
                {
                    animator.SetBool("WatchTV", true);
                    Debug.Log("wwwwwwww");
                }
                else if (Vector3.Distance(characterObject.transform.position, bedPosition) < 0.1f)
                {
                    animator.SetBool("Sleep", true);
                    Debug.Log("sssssssss");
                }
                if (isMoving)
                {
                    isMoving = false;
                    waitTime = Random.Range(14, 26);
                }
            }

            if (!isMoving)
            {
                // 移動待機中の時間をカウントダウン
                waitTime -= Time.deltaTime;
                if (waitTime <= 0)
                {
                    SetNewTarget();
                }
            }
        }
    }


    public void SetPosition(Vector3 Position)
    {
        agent.SetDestination(Position); // NavMeshAgentに目的地を設定
    }
    void resetAnimation()
    {
        animator.SetBool("WatchTV", false);
        animator.SetBool("Sleep", false);
    }
    // 新しい目的地を設定するメソッド
    void SetNewTarget()
    {
        // ランダムにtvPositionかbedPositionを選択
        targetPosition = (Random.value > 0.5f) ? tvPosition : bedPosition;
        isMoving = true;
        Debug.Log($"New Target: {targetPosition}");
        agent.SetDestination(targetPosition);
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
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
