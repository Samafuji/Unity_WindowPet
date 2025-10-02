using UnityEngine;

public class IKControlWithAnimation : MonoBehaviour
{
    protected Animator animator;

    public bool ikActive = true;
    public Transform rightHandObj = null;
    public Transform leftHandObj = null;
    public Transform lookObj = null;

    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;
    private Plane dragPlane;
    private Transform currentTarget;

    void Start()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == rightHandObj || hit.transform == leftHandObj)
                {
                    isDragging = true;
                    currentTarget = hit.transform;
                    dragPlane = new Plane(mainCamera.transform.forward, currentTarget.position);
                    float distance;
                    dragPlane.Raycast(ray, out distance);
                    offset = currentTarget.position - ray.GetPoint(distance);

                    // 検出したターゲットの名前をログに出力
                    Debug.Log("Detected target: " + hit.transform.name);
                }
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            float distance;
            if (dragPlane.Raycast(ray, out distance))
            {
                currentTarget.position = ray.GetPoint(distance) + offset;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            currentTarget = null;
        }
    }

    void OnAnimatorIK()
    {
        if (animator)
        {
            if (ikActive)
            {
                if (lookObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }

                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }

                if (leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }
}
