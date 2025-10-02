using UnityEngine;
using UnityEngine.AI;

public class RandomPositionMovement : MonoBehaviour
{
    public Animator animator;  // キャラクターにアタッチされたAnimatorコンポーネント
    public float rotationSpeed = 10.0f; // キャラクターの回転速度
    public float heightOffset = 0.15f; // クリック位置の高さを調整するオフセット
    public float maxSpeed = 2.0f; // キャラクターの最大速度
    public float acceleration = 8.0f; // キャラクターの加速度
    public GameObject capsule; // 停止時に向くオブジェクト
    public NavMeshAgent agent; // NavMeshAgentコンポーネント

    private Vector3 tvPositionVec = new Vector3(31.07652f, -3.870851f, -8.609716f);
    private Vector3 bedPositionVec = new Vector3(37.1434f, -3.870851f, -12.59303f);
    public Transform tvPosition; // テレビの前の位置
    public Transform bedPosition; // ベッドの前の位置

    private Transform targetPosition; // 現在の目的地
    private bool isMoving = false; // キャラクターが移動中かどうか
    private float waitTime; // 次の目的地に移動するまでの待機時間

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgentコンポーネントを取得
        agent.speed = maxSpeed; // NavMeshAgentの最大速度を設定
        agent.acceleration = acceleration; // NavMeshAgentの加速度を設定
        agent.stoppingDistance = 0.1f; // ストップディスタンスを設定 (例: 0.1f)

        // テレビとベッドの位置を設定
        if (tvPosition != null)
            tvPosition.position = tvPositionVec;
        if (bedPosition != null)
            bedPosition.position = bedPositionVec;

        SetNewTarget(); // 最初の目的地を設定
    }

    void Update()
    {
        if (isMoving)
        {
            agent.SetDestination(targetPosition.position);

            if (agent.remainingDistance > agent.stoppingDistance + 0.1f)
            {
                // 移動中のアニメーション
                animator.SetBool("Sleep", false);
                animator.SetBool("WatchTV", false);
                animator.SetBool("Walk", true);
                RotateTowards(agent.steeringTarget);
            }
            else
            {
                // 目的地に到着
                animator.SetBool("Walk", false);
                isMoving = false;

                // 位置によってアニメーションを切り替え
                if (targetPosition == tvPosition)
                {
                    animator.SetTrigger("WatchTV");
                }
                else if (targetPosition == bedPosition)
                {
                    animator.SetTrigger("Sleep");
                }

                // 次の移動までのランダムな待機時間を設定
                waitTime = Random.Range(11, 22);
            }
        }
        else
        {
            // 移動待機中の時間をカウントダウン
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                SetNewTarget();
            }
        }
    }

    // 新しい目的地を設定するメソッド
    void SetNewTarget()
    {
        // ランダムにtvPositionかbedPositionを選択
        targetPosition = (Random.value > 0.5f) ? tvPosition : bedPosition;
        isMoving = true;
         Debug.Log($"New Target: {targetPosition.position}");
        agent.SetDestination(targetPosition.position);
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
