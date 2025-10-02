using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public Image crosshair; // クロスヘアのUI Image
    public GameObject characterObject; // キャラクターのゲームオブジェクト
    public GameObject capsule; // プレイヤーのオブジェクト

    public GameObject[] teleportPoints; // テレポート用のオブジェクト（敵2用）

    private Animator animator; // キャラクターにアタッチされたAnimatorコンポーネント
    private NavMeshAgent agent; // NavMeshAgentコンポーネント

    public float rotationSpeed = 10.0f; // キャラクターの回転速度
    public float heightOffset = 0.0f; // クリック位置の高さを調整するオフセット
    public float maxSpeed = 3.0f; // キャラクターの最大速度
    public float acceleration = 8.0f; // キャラクターの加速度

    private Vector3 initialPosition; // 敵の最初の位置
    private bool isMoving = false; // キャラクターが移動中かどうか
    private bool isChasing = false; // キャラクターが追跡中かどうか

    [SerializeField]
    private float waitTime = 1f; // 次の目的地に移動するまでの待機時間

    public enum EnemyType
    {
        PatrolRadius, // 敵1
        TeleportChaser, // 敵2
        FieldOfViewChaser // 敵3
    }

    public EnemyType enemyType;

    // 視界の設定（敵3用）
    public float viewAngle = 52f; // 左右30度
    public float viewDistance = 15f; // 視界の距離
    public LayerMask obstacleMask; // 障害物のレイヤー

    private void Start()
    {
        animator = characterObject.GetComponent<Animator>();
        agent = characterObject.GetComponent<NavMeshAgent>();
        agent.speed = maxSpeed; // NavMeshAgentの最大速度を設定
        agent.acceleration = acceleration; // NavMeshAgentの加速度を設定
        initialPosition = transform.position; // 初期位置を保存

        if (crosshair != null)
        {
            crosshair.raycastTarget = false;
        }

    }
    public float distanceThreshold = 2.0f; // capsuleとの距離しきい値
    void Update()
    {
        // 敵タイプに応じて動作を開始
        switch (enemyType)
        {
            case EnemyType.PatrolRadius:
                StartCoroutine(PatrolWithinRadius());
                break;
            case EnemyType.TeleportChaser:
                StartCoroutine(TeleportChase());
                break;
            case EnemyType.FieldOfViewChaser:
                StartCoroutine(FieldOfViewChase());
                break;
        }

        // capsuleまでの距離を計算
        float distanceToCapsule = Vector3.Distance(characterObject.transform.position, capsule.transform.position);

        // 追跡中かつcapsuleとの距離が一定以下になった場合、追跡を停止
        if (distanceToCapsule <= distanceThreshold)
        {
            isChasing = false;
            agent.ResetPath(); // NavMeshAgentのパスをリセットして停止
            Debug.Log("追跡を停止: capsuleに十分近づいた");
        }

        // NavMeshAgentが有効かどうかをチェック
        if (agent.enabled && agent.isOnNavMesh)
        {
            // 目標位置に向かって回転
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                animator.SetBool("Walk", true);
                RotateTowards(agent.steeringTarget);
            }
            else
            {
                animator.SetBool("Walk", false);
            }
        }
    }
    // 敵1: 半径15m以内にプレイヤーが入ったら追跡
    private IEnumerator PatrolWithinRadius()
    {
        float distanceToPlayer = Vector3.Distance(capsule.transform.position, transform.position);
        if (distanceToPlayer <= 15f)
        {
            agent.SetDestination(capsule.transform.position);
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        yield return new WaitForSeconds(0.5f);
    }

    // 敵2: 5つのオブジェクトの中からランダムにテレポートし、30秒後に元の位置に戻る
    private IEnumerator TeleportChase()
    {
        yield return new WaitForSeconds(60f); // 1分待機

        // ランダムなテレポートポイントを選択
        GameObject randomPoint = teleportPoints[Random.Range(0, teleportPoints.Length)];
        agent.Warp(randomPoint.transform.position);

        yield return new WaitForSeconds(30f); // 30秒待機

        // 初期位置に戻る
        agent.Warp(initialPosition);
    }

    // 敵3: 視界に入ったプレイヤーを追跡
    private IEnumerator FieldOfViewChase()
    {
        Vector3 directionToPlayer = (capsule.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Debug.Log(angleToPlayer);
        if ((angleToPlayer < viewAngle || isChasing) && Vector3.Distance(transform.position, capsule.transform.position) < viewDistance)
        {
            bool isWithinViewDistance = Vector3.Distance(transform.position, capsule.transform.position) < viewDistance;
            Debug.Log("Distance: " + isWithinViewDistance);

            RaycastHit hit;
            Vector3 rayStartPosition = transform.position + Vector3.up * 1.5f; // キャラクターの高さに応じて調整

            // 敵の位置からプレイヤーに向かってRayを飛ばし、障害物があるかチェック
            if (Physics.Raycast(rayStartPosition, directionToPlayer, out hit, viewDistance))
            {
                // Debug.DrawLineでRayを可視化 (赤色)
                Debug.DrawLine(rayStartPosition, hit.point, Color.red);

                // Rayがプレイヤーに当たった場合、追跡を開始
                if (hit.collider.gameObject == capsule)
                {
                    agent.SetDestination(capsule.transform.position);
                    isChasing = true;
                }
                else if (hit.collider.gameObject.transform.IsChildOf(characterObject.transform))
                {
                    // hitしたオブジェクトがcharacterObjectの子オブジェクトの場合は無視
                    Debug.Log("Ignored child object: " + hit.collider.gameObject.name);
                }
                else
                {
                    // Rayがプレイヤー以外に当たった場合、追跡を中止
                    isChasing = false;
                    Debug.Log(hit.collider.gameObject);
                }
                Debug.Log(isChasing);
            }
        }
        else
        {
            isChasing = false;
        }

        yield return new WaitForSeconds(0.5f);
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

    // Trigger に入ったときの処理
    void OnTriggerEnter(Collider other)
    {
        // capsule に接触した場合
        if (other.gameObject == capsule)
        {
            isChasing = false; // 追跡を停止
            Debug.Log("追跡を停止: キャラクターがcapsuleに接触");
        }
    }

}
