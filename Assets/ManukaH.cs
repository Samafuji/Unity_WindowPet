using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ManukaH : MonoBehaviour
{
    public GameObject Manuka;
    public GameObject Mann;
    private Animator animatorManuka;
    private Animator animatorMann;


    // Find the J_Bip_C_Neck transform within Mann
    public Transform neckTransform;
    public GameObject capsule;      // Assign the capsule GameObject in the Inspector
    private Transform cameraTransform;  // Assuming the camera is a direct child of the capsule
    // private Vector3 positionOffset = new Vector3(0f, 0.44452f, 0f);  // Optional: Offset for capsule's position
    private Vector3 positionOffset = new Vector3(0f, 0.2f, 0f);  // Optional: Offset for capsule's position
    private Vector3 rotationOffset = new Vector3(0f, 180f, 0f);  // Optional: Offset for capsule's rotation


    public Button Button1;
    public Button Button2;
    public Button Button3;
    public Button ButtonTimeStop;
    public Slider animationSpeedSlider; // Slider to control the animation speed


    public Button ManInspectorButton;

    public float increaseAmount = 0.1f; // Amount to increase the slider value each second
    public float interval = 1.0f; // Interval in seconds for the increase

    private bool isIncreasing = false; // To track if the Coroutine is running
    public CharacterEStatus characterEStatus; // Status
    public ScriptManager scriptManager; // 無効にしたいスクリプト1

    // Start is called before the first frame update
    void Start()
    {
        animatorManuka = Manuka.GetComponent<Animator>();
        animatorMann = Mann.GetComponent<Animator>();
        Mann.SetActive(false);

        Button1.onClick.AddListener(Button1Function);
        Button2.onClick.AddListener(Button2Function);
        Button3.onClick.AddListener(Button3Function);

        if (animationSpeedSlider != null)
        {
            // Set the slider's min and max values
            animationSpeedSlider.minValue = 0.5f;
            animationSpeedSlider.maxValue = 1.2f;

            // Set the initial animation speed based on the slider value
            UpdateAnimationSpeed(1);

            // Add a listener to call UpdateAnimationSpeed whenever the slider value changes
            animationSpeedSlider.onValueChanged.AddListener(UpdateAnimationSpeed);
        }

        // Find the camera within the capsule
        cameraTransform = capsule.transform.Find("Main Camera");  // Assuming the camera is a direct child of the capsule
        ManInspectorButton.onClick.AddListener(MoveToManInspector);

        ButtonTimeStop.onClick.AddListener(StopManuka);

    }
    void Update()
    {
        // Check the poseIndex value in each frame
        if (animatorManuka != null)
        {
            int poseIndex = animatorManuka.GetInteger("PoseIndex");
            if (poseIndex == 111 || poseIndex == 112 || poseIndex == 113)
            {
                // Start the Coroutine to increase the slider value if not already running
                if (!isIncreasing)
                {
                    // StartCoroutine(IncreaseSliderValue());
                    StartCoroutine(IncreaseZecchouContinuously());
                }
            }
            else
            {
                if (isIncreasing)
                {
                    UpdateAnimationSpeed(1);
                    StopCoroutine(IncreaseZecchouContinuously());
                    isIncreasing = false;
                }
                // // Stop increasing when poseIndex is not 111, 112, or 113
                // StopCoroutine(IncreaseSliderValue());
                // isIncreasing = false;
            }
        }
    }

    public void StopManuka()
    {
        int poseIndex = animatorManuka.GetInteger("PoseIndex");
        if (scriptManager.lookAtTargetOffset.isStopped)
        {
            if (poseIndex == 0) { animatorManuka.speed = 1; }
            else
            {
                UpdateAnimationSpeed(animationSpeedSlider.value);
            }
            if (scriptManager.fpsController.IsFps)
            {
                scriptManager.clickToMoveAndAvoidObstacles.enabled = true;
                scriptManager.navMeshAgent.enabled = true;
            }
            scriptManager.lookAtTargetOffset.isStopped = false;
        }
        else
        {
            animatorManuka.speed = 0;
            scriptManager.clickToMoveAndAvoidObstacles.enabled = false;
            scriptManager.navMeshAgent.enabled = false;
            scriptManager.lookAtTargetOffset.isStopped = true;
        }
    }
    public void MoveToManInspector()
    {
        if (neckTransform != null && capsule != null)
        {
            // Set capsule's position to neck's forward direction with an optional offset
            capsule.transform.position = neckTransform.position + positionOffset - neckTransform.forward * 0.3f;

            // Set the rotation to only affect the Y-axis (keeping X and Z as 0)
            Quaternion targetRotation = Quaternion.LookRotation(neckTransform.forward);
            Vector3 eulerRotation = targetRotation.eulerAngles;
            eulerRotation.x = -35.35f;
            eulerRotation.z = 0;

            // Apply the rotation to the camera, including any additional rotation offsets
            cameraTransform.rotation = Quaternion.Euler(eulerRotation) * Quaternion.Euler(rotationOffset);

        }
        else
        {
            Debug.LogError("Neck transform or capsule is not assigned or found.");
        }
    }

    public void Button1Function()
    {
        if (!Mann.activeSelf)
        {
            Mann.SetActive(true);
        }
        UpdateAnimationSpeed(animationSpeedSlider.value);

        animatorManuka.SetInteger("PoseIndex", 111);
        animatorMann.SetInteger("PoseIndex", 111);

        Manuka.transform.position = new Vector3(29.00626f, -3.870851f, -11.18505f);
        Manuka.transform.rotation = Quaternion.Euler(new Vector3(0f, 190f, 0f));

        Mann.transform.position = new Vector3(29.198f, -3.838f, -10.746f);
        Mann.transform.rotation = Quaternion.Euler(new Vector3(0f, 191.6f, 0f));

        StartCoroutine(scriptManager.lookAtTargetOffset.StopAnimation());
    }
    public void Button2Function()
    {
        if (!Mann.activeSelf)
        {
            Mann.SetActive(true);
        }
        UpdateAnimationSpeed(animationSpeedSlider.value);

        animatorManuka.SetInteger("PoseIndex", 112);
        animatorMann.SetInteger("PoseIndex", 112);

        Manuka.transform.position = new Vector3(29.00626f, -3.870851f, -11.18505f);
        Manuka.transform.rotation = Quaternion.Euler(new Vector3(0f, 190f, 0f));

        Mann.transform.position = new Vector3(29.213f, -3.838f, -10.621f);
        Mann.transform.rotation = Quaternion.Euler(new Vector3(0f, 188.8f, 0f));

        StartCoroutine(scriptManager.lookAtTargetOffset.StopAnimation());
    }
    public void Button3Function()
    {
        if (!Mann.activeSelf)
        {
            Mann.SetActive(true);
        }
        UpdateAnimationSpeed(animationSpeedSlider.value);

        animatorManuka.SetInteger("PoseIndex", 113);
        animatorMann.SetInteger("PoseIndex", 113);

        Manuka.transform.position = new Vector3(29.00626f, -3.870851f, -11.18505f);
        Manuka.transform.rotation = Quaternion.Euler(new Vector3(0f, 5.9f, 0f));

        Mann.transform.position = new Vector3(29.111f, -3.838f, -10.33f);
        Mann.transform.rotation = Quaternion.Euler(new Vector3(0f, 180.7f, 0f));

        StartCoroutine(scriptManager.lookAtTargetOffset.StopAnimation());
    }

    void UpdateAnimationSpeed(float value)
    {
        if (animatorManuka != null)
        {
            animatorManuka.speed = value;
        }

        if (animatorMann != null)
        {
            animatorMann.speed = value;
        }
    }

    IEnumerator IncreaseSliderValue()
    {
        isIncreasing = true;

        while (true)
        {
            // Increase the slider value by the specified amount
            animationSpeedSlider.value += increaseAmount;

            // Ensure the slider value remains within its min and max range
            animationSpeedSlider.value = Mathf.Clamp(animationSpeedSlider.value, animationSpeedSlider.minValue, animationSpeedSlider.maxValue);

            // Wait for the specified interval before repeating
            yield return new WaitForSeconds(interval);
        }
    }
    private IEnumerator IncreaseZecchouContinuously()
    {
        isIncreasing = true;
        while (true)
        {
            characterEStatus.IncreaseZecchou(animationSpeedSlider.value);
            yield return new WaitForSeconds(1.0f); // Wait for 1 second before increasing again
        }
    }
}
