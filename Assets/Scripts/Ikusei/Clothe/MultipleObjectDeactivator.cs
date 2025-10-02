using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultipleObjectDeactivator : MonoBehaviour
{
    public GameObject Character;
    public CharacterEStatus characterEStatus;
    private Animator animator;
    // 8つのオブジェクトグループ
    [SerializeField] private GameObject[] Uwagigroup1;
    [SerializeField] private GameObject[] Eprongroup2;
    [SerializeField] private GameObject[] Zubongroup3;
    [SerializeField] private GameObject[] Bragroup4;
    [SerializeField] private GameObject[] Pantgroup5;
    [SerializeField] private GameObject[] Socksgroup6;
    [SerializeField] private GameObject[] Shoesgroup7;
    [SerializeField] private GameObject[] OtherClothe;

    // 各グループに対応するボタン
    [SerializeField] private Button[] deactivateButtons;
    [SerializeField] private Button ChangeButton;

    // 全グループをまとめたリスト
    private List<GameObject[]> allGroups;


    private void Start()
    {
        animator = Character.GetComponent<Animator>();
        // 8つのグループをリストに追加
        allGroups = new List<GameObject[]>
        {
            Uwagigroup1, Eprongroup2, Zubongroup3, Bragroup4, Pantgroup5, Socksgroup6, Shoesgroup7
        };

        // 各ボタンにリスナーを追加
        for (int i = 0; i < deactivateButtons.Length; i++)
        {
            int index = i; // ローカル変数を使ってインデックスをキャプチャ
            deactivateButtons[i].onClick.AddListener(() => DeactivateGroup(index));
        }
        ChangeButton.onClick.AddListener(DeactivateAllGroups);
    }

    private void DeactivateGroup(int groupIndex)
    {
        if (groupIndex < allGroups.Count)
        {
            // 指定されたオブジェクト群をオンオフ切り替え
            foreach (GameObject obj in allGroups[groupIndex])
            {
                obj.SetActive(!obj.activeSelf);
            }
            Hazukashido();
        }
    }

    public void DeactivateAllGroups()
    {
        // 全グループのオブジェクトをオンオフ切り替え
        foreach (var group in allGroups)
        {
            foreach (GameObject obj in group)
            {
                obj.SetActive(false);
            }
        }
        foreach (GameObject obj in OtherClothe)
        {
            obj.SetActive(true);
        }
    }

    public void Hazukashido()
    {
        int Hazukashisa = 0;

        if (!Bragroup4[0].activeSelf)
        {
            Hazukashisa += Random.Range(7, 13);
        }
        if (!Pantgroup5[0].activeSelf)
        {
            Hazukashisa += Random.Range(7, 13);
        }

        if (!Uwagigroup1[0].activeSelf)
        {
            Hazukashisa += Random.Range(26, 34);
        }
        if (!Zubongroup3[0].activeSelf)
        {
            Hazukashisa += Random.Range(26, 34);
        }

        if (!Eprongroup2[0].activeSelf && Hazukashisa > 25)
        {
            Hazukashisa += Random.Range(8, 12);
        }
        else if (!Eprongroup2[0].activeSelf && Hazukashisa > 29)
        {
            Hazukashisa += Random.Range(8, 12);
        }

        // if (Socksgroup6[0].activeSelf)
        // {
        //     Hazukashisa += 0;
        // }
        // if (Shoesgroup7[0].activeSelf)
        // {
        //     Hazukashisa += 0;
        // }
        if (Hazukashisa > 100) Hazukashisa = 100;
        animator.SetFloat("Shy", Hazukashisa);
        characterEStatus.SetHazukashido(Hazukashisa);
    }
}
