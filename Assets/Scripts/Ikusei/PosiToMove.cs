// using UnityEngine;
// using System.Collections;

// public class PosiToMove : MonoBehaviour
// {
//     public Vector3 targetPosition; // The position to move to
//     public Vector3 targetRotation; // The rotation to achieve
//     public float moveSpeed = 3f; // Movement speed
//     public float rotationSpeed = 3f; // Rotation speed

//     public void MoveToPosition()
//     {
//         StartCoroutine(MoveAndRotate());
//     }

//     private IEnumerator MoveAndRotate()
//     {
//         while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
//         {
//             // Move towards the target position
//             transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

//             // Smoothly rotate towards the target rotation
//             Quaternion targetRot = Quaternion.Euler(targetRotation);
//             transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

//             yield return null;
//         }
//     }
// }
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class PosiToMove : MonoBehaviour
{
    public Vector3 targetPosition; // The position to move to
    public Vector3 targetRotation; // The rotation to achieve
    public float rotationSpeed = 3f; // Rotation speed

    private NavMeshAgent agent;
    public Animator animator;  // キャラクターにアタッチされたAnimatorコンポーネント

    public Vector3 newPosition = new Vector3(39.38907f, -0.17f, -11.69f); // 移動後の新しい位置
    public Vector3 newRotation = new Vector3(62.82f, 179.25f, 0f); // 移動後の新しい位置

    private void Start()
    {
        // Find the object named "MANUKA" in the scene
        GameObject manukaObject = GameObject.Find("MANUKA");

        if (manukaObject != null)
        {
            agent = manukaObject.GetComponent<NavMeshAgent>();
            animator = manukaObject.GetComponent<Animator>();
            if (agent == null)
            {
                Debug.LogError("NavMeshAgent component is missing from the MANUKA object.");
            }
        }
        else
        {
            Debug.LogError("MANUKA object not found in the scene.");
        }
    }

    public void MoveToPosition()
    {
        if (agent != null)
        {
            // Move the manukaObject to the target position
            agent.SetDestination(targetPosition);
            animator.SetBool("Walk", true);
            // StartCoroutine(RotateManukaObject());
        }
    }

    private IEnumerator RotateManukaObject()
    {
        Quaternion targetRot = Quaternion.Euler(targetRotation);

        while (Quaternion.Angle(agent.transform.rotation, targetRot) > 0.1f)
        {
            // Smoothly rotate the manukaObject towards the target rotation
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
