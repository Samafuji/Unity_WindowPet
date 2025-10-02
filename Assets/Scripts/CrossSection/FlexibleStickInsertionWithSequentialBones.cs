using System.Collections.Generic;
using UnityEngine;

public class FlexibleStickInsertionWithSequentialBones : MonoBehaviour
{
    public Transform rootBone;
    public Transform hole;
    public float attractionSpeed = 5f;
    public AnimationCurve curve;
    public float fixedDistanceMultiplier = 2.5f;

    private Transform[] segments;
    private int currentFixedSegment;
    private bool isInserted = false;
    private Vector3[] initialPositions;
    private float[] boneLengths;

    void Start()
    {
        InitializeSegments();

        initialPositions = new Vector3[segments.Length];
        boneLengths = new float[segments.Length - 1];

        for (int i = 0; i < segments.Length; i++)
        {
            initialPositions[i] = segments[i].localPosition;

            if (i < segments.Length - 1)
            {
                boneLengths[i] = Vector3.Distance(segments[i].localPosition, segments[i + 1].localPosition);
            }
        }

        currentFixedSegment = segments.Length - 1;
    }

    void Update()
    {
        if (!isInserted && Vector3.Distance(segments[currentFixedSegment].position, hole.position) < 0.75f)
        {
            isInserted = true;
        }
        if (!isInserted)
        {
            Debug.Log(Vector3.Distance(segments[currentFixedSegment].position, hole.position));
        }

        if (isInserted)
        {
            segments[currentFixedSegment].position = Vector3.Lerp(segments[currentFixedSegment].position, hole.position, Time.deltaTime * attractionSpeed);
            segments[currentFixedSegment].rotation = Quaternion.Lerp(segments[currentFixedSegment].rotation, hole.rotation, Time.deltaTime * attractionSpeed);

            if (currentFixedSegment > 0 && Vector3.Distance(segments[currentFixedSegment - 1].position, hole.position) < 0.1f)
            {
                currentFixedSegment--;
            }

            // 新しく追加した抜ける条件
            if (currentFixedSegment == segments.Length - 1 && Vector3.Distance(segments[currentFixedSegment].position, hole.position) > 1.5f)
            {
                isInserted = false;
            }
            if (currentFixedSegment < segments.Length - 1 && Vector3.Distance(segments[currentFixedSegment].position, hole.position) > 1.5f)
            {
                Debug.Log(Vector3.Distance(segments[currentFixedSegment].position, hole.position));
                currentFixedSegment++;
            }

            UpdateSegmentPositions();

            for (int i = segments.Length - 1; i > currentFixedSegment; i--)
            {
                segments[i].localPosition = Vector3.Lerp(segments[i].localPosition, initialPositions[i], Time.deltaTime * attractionSpeed);
            }
        }
    }

    void UpdateSegmentPositions()
    {
        for (int i = 1; i <= currentFixedSegment; i++)
        {
            float t = (float)i / currentFixedSegment;
            Vector3 targetPosition = Vector3.Lerp(rootBone.localPosition, segments[currentFixedSegment].localPosition, curve.Evaluate(t));
            segments[i].localPosition = Vector3.Lerp(segments[i].localPosition, targetPosition, Time.deltaTime * attractionSpeed);

            Vector3 direction = (segments[i - 1].localPosition - segments[i].localPosition).normalized;
            segments[i].localPosition = segments[i - 1].localPosition - direction * boneLengths[i - 1];
        }
    }

    void InitializeSegments()
    {
        if (rootBone == null)
        {
            Debug.LogError("rootBone is not assigned!");
            return;
        }

        List<Transform> allSegments = new List<Transform>();
        GetAllChildTransforms(rootBone, allSegments);

        segments = allSegments.ToArray();

        Debug.Log("Segments count: " + segments.Length);
    }

    void GetAllChildTransforms(Transform parent, List<Transform> list)
    {
        foreach (Transform child in parent)
        {
            list.Add(child);
            GetAllChildTransforms(child, list);
        }
    }
}


// using UnityEngine;
// using System.Collections.Generic; // これを追加
// public class FlexibleStickInsertionWithSequentialBones : MonoBehaviour
// {
//     public Transform rootBone; // ルートボーンを指定
//     public Transform hole;
//     public float attractionSpeed = 5f; // 吸い付くスピード
//     public AnimationCurve curve; // 曲がる動きを制御するカーブ
//     public float fixedDistanceMultiplier = 2.5f; // 固定ボーンを戻すための距離係数

//     private Transform[] segments; // 棒のセグメント
//     private int currentFixedSegment; // 現在固定されているセグメント
//     private bool isInserted = false;
//     private Vector3[] initialPositions;
//     private float[] boneLengths;

//     void Start()
//     {
//         // rootBoneの子オブジェクトを取得し、segments配列に設定
//         InitializeSegments();

//         // セグメントの初期位置を保存 (ワールド座標)
//         initialPositions = new Vector3[segments.Length];
//         boneLengths = new float[segments.Length - 1];

//         for (int i = 0; i < segments.Length; i++)
//         {
//             initialPositions[i] = segments[i].localPosition;

//             // 各セグメント間の長さを計算
//             if (i < segments.Length - 1)
//             {
//                 boneLengths[i] = Vector3.Distance(segments[i].position, segments[i + 1].position);
//             }
//         }

//         // 最後のセグメントからスタート
//         currentFixedSegment = segments.Length - 1;
//     }

//     void Update()
//     {
//         // 現在固定されているセグメントが穴に近づいたかを確認
//         if (!isInserted && Vector3.Distance(segments[currentFixedSegment].position, hole.position) < 0.15f)
//         {
//             isInserted = true;
//         }

//         if (!isInserted)
//         {
//             Debug.Log(Vector3.Distance(segments[currentFixedSegment].position, hole.position));
//         }
//         Debug.Log("/////////////////////////////////////////////////////////////////" + currentFixedSegment);
//         if (isInserted)
//         {
//             // 現在固定されているセグメントを穴に吸い付ける
//             segments[currentFixedSegment].position = Vector3.Lerp(segments[currentFixedSegment].position, hole.position, Time.deltaTime * attractionSpeed);
//             segments[currentFixedSegment].rotation = Quaternion.Lerp(segments[currentFixedSegment].rotation, hole.rotation, Time.deltaTime * attractionSpeed);

//             // 次のセグメントが穴に入る条件をチェック
//             if (currentFixedSegment > 0 && Vector3.Distance(segments[currentFixedSegment - 1].position, hole.position) < 0.1f)
//             {
//                 // 次のセグメントを固定対象に変更
//                 currentFixedSegment--;
//             }

//             if (currentFixedSegment < segments.Length - 1)
//             {
//                 Debug.Log(Vector3.Distance(segments[currentFixedSegment].position, segments[currentFixedSegment - 1].position) + "<>" + fixedDistanceMultiplier * boneLengths[currentFixedSegment]);
//             }
//             // 5倍以上離れている場合、固定セグメントを一つ前に戻す
//             if (currentFixedSegment < segments.Length - 1 && Vector3.Distance(segments[currentFixedSegment].position, segments[currentFixedSegment - 1].position) > fixedDistanceMultiplier * boneLengths[currentFixedSegment])
//             {
//                 currentFixedSegment++;
//             }

//             Vector3 targetPosition = Vector3.Lerp(initialPositions[currentFixedSegment], segments[currentFixedSegment].localPosition, curve.Evaluate(0.5f));
//             segments[currentFixedSegment].localPosition = Vector3.Lerp(segments[currentFixedSegment].localPosition, targetPosition, Time.deltaTime * attractionSpeed);

//             // 他のセグメントを元の長さ（初期位置）に戻す
//             for (int i = segments.Length - 1; i > currentFixedSegment; i--)
//             {
//                 segments[i].localPosition = Vector3.Lerp(segments[i].localPosition, initialPositions[i], Time.deltaTime * attractionSpeed);
//             }
//         }
//     }
//     void InitializeSegments()
//     {
//         // rootBoneの子オブジェクトを再帰的に取得してsegments配列に設定
//         if (rootBone == null)
//         {
//             Debug.LogError("rootBone is not assigned!");
//             return;
//         }

//         // 再帰的にすべての子オブジェクトを取得
//         List<Transform> allSegments = new List<Transform>();
//         GetAllChildTransforms(rootBone, allSegments);

//         segments = allSegments.ToArray();

//         Debug.Log("Segments count: " + segments.Length); // 確認用のデバッグ出力
//     }

//     void GetAllChildTransforms(Transform parent, List<Transform> list)
//     {
//         foreach (Transform child in parent)
//         {
//             list.Add(child);
//             // 再帰的に子の子も取得
//             GetAllChildTransforms(child, list);
//         }
//     }

// }
