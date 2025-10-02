using UnityEngine;

public class FlexibleStickInsertion : MonoBehaviour
{
    public Transform hole;
    public Transform[] segments; // 棒のセグメント
    public float attractionSpeed = 5f; // 吸い付くスピード
    public float fixedDistanceMultiplier = 20f; // 固定ボーンを戻すための距離係数
    public AnimationCurve curve; // 曲がる動きを制御するカーブ

    private int currentFixedSegment; // 現在固定されているセグメント
    private bool isInserted = false;
    private Vector3[] initialPositions;

    void Start()
    {
        // セグメントの初期位置を保存
        initialPositions = new Vector3[segments.Length];
        for (int i = 0; i < segments.Length; i++)
        {
            initialPositions[i] = segments[i].localPosition;
        }

        // 最後のセグメントからスタート
        currentFixedSegment = segments.Length - 1;
    }

    void Update()
    {
        // 現在固定されているセグメントが穴に近づいたかを確認
        if (!isInserted && Vector3.Distance(segments[currentFixedSegment].position, hole.position) < 0.15f)
        {
            isInserted = true;
        }
        if (!isInserted)
        {
            Debug.Log(Vector3.Distance(segments[currentFixedSegment].position, hole.position));
        }
        Debug.Log(currentFixedSegment);
        if (isInserted)
        {
            // 現在固定されているセグメントを穴に吸い付ける
            segments[currentFixedSegment].position = Vector3.Lerp(segments[currentFixedSegment].position, hole.position, Time.deltaTime * attractionSpeed);
            segments[currentFixedSegment].rotation = Quaternion.Lerp(segments[currentFixedSegment].rotation, hole.rotation, Time.deltaTime * attractionSpeed);

            // 次のセグメントが穴に入る条件をチェック
            if (currentFixedSegment > 0 && Vector3.Distance(segments[currentFixedSegment - 1].position, hole.position) < 0.1f)
            {
                // 次のセグメントを固定対象に変更
                currentFixedSegment--;
                // isInserted = false; // 新しいセグメントの挿入準備
            }
            if (currentFixedSegment < segments.Length - 1)
            {
                Debug.Log(Vector3.Distance(segments[currentFixedSegment].position, segments[currentFixedSegment - 1].position) + "&" + fixedDistanceMultiplier * Vector3.Distance(initialPositions[currentFixedSegment], initialPositions[currentFixedSegment - 1]));
            }
            if (currentFixedSegment < segments.Length - 1 && Vector3.Distance(segments[currentFixedSegment].position, segments[currentFixedSegment + 1].position) > fixedDistanceMultiplier * Vector3.Distance(initialPositions[currentFixedSegment], initialPositions[currentFixedSegment + 1]))
            {
                if (Vector3.Distance(segments[currentFixedSegment].position, segments[currentFixedSegment + 1].position) > Vector3.Distance(initialPositions[currentFixedSegment], initialPositions[currentFixedSegment + 1]))
                {
                    Debug.Log("Bigger");
                }
                else
                {
                    Debug.Log("Smaller");
                }

                // currentFixedSegment++;
            }

            Vector3 targetPosition = Vector3.Lerp(initialPositions[currentFixedSegment], segments[currentFixedSegment].localPosition, curve.Evaluate(0.5f));
            segments[currentFixedSegment].localPosition = Vector3.Lerp(segments[currentFixedSegment].localPosition, targetPosition, Time.deltaTime * attractionSpeed);

            // 他のセグメントを元の長さ（初期位置）に戻す
            for (int i = segments.Length - 1; i > currentFixedSegment; i--)
            {
                segments[i].localPosition = Vector3.Lerp(segments[i].localPosition, initialPositions[i], Time.deltaTime * attractionSpeed);
            }
            // 他のセグメントを柔軟に動かす
            // for (int i = segments.Length - 1; i > currentFixedSegment; i--)
            // {
            //     float t = (float)(segments.Length - 1 - i) / (segments.Length - 1 - currentFixedSegment);
            //     Vector3 targetPosition = Vector3.Lerp(initialPositions[i], segments[currentFixedSegment].localPosition, curve.Evaluate(t));
            //     segments[i].localPosition = Vector3.Lerp(segments[i].localPosition, targetPosition, Time.deltaTime * attractionSpeed);
            // }

            // // 0から順番に他のセグメントを柔軟に動かす
            // for (int i = 0; i < currentFixedSegment; i++)
            // {
            //     float t = (float)i / (currentFixedSegment);
            //     Vector3 targetPosition = Vector3.Lerp(initialPositions[i], segments[currentFixedSegment].localPosition, curve.Evaluate(t));
            //     segments[i].localPosition = Vector3.Lerp(segments[i].localPosition, targetPosition, Time.deltaTime * attractionSpeed);
            // }
        }
    }
}