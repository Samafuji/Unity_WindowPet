using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button SleepBTN;
    public Button TrainBTN;
    public Button SkillBTN;
    public Button MedicalBTN;
    public Button HangOutBTN;
    public Button RaceBTN;

    public GameObject TarinImage;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        TrainBTN.onClick.AddListener(TogglePanel);
    }

    void TogglePanel()
    {
        // パネルのアクティブ状態を切り替える
        TarinImage.SetActive(true);
    }
}
