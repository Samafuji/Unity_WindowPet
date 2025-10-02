// using UnityEngine;

// public class AttachTooltipScript : MonoBehaviour
// {
//     public GameObject tooltipImage; // UIのImageオブジェクトをアサインする

//     private void Start()
//     {
//         // 親オブジェクトのコライダーをチェックしてスクリプトを追加
//         AddTooltipScriptToGameObject(gameObject);

//         // 子オブジェクトに対しても同様にスクリプトを追加
//         foreach (Transform child in transform)
//         {
//             AddTooltipScriptToGameObject(child.gameObject);
//         }
//     }

//     private void AddTooltipScriptToGameObject(GameObject obj)
//     {
//         // オブジェクトにTooltipスクリプトがアタッチされていない場合、追加する
//         ShowTooltip tooltipScript = obj.GetComponent<ShowTooltip>();
//         if (tooltipScript == null)
//         {
//             tooltipScript = obj.AddComponent<ShowTooltip>();
//         }

//         // ツールチップのUIオブジェクトを設定
//         tooltipScript.tooltipImage = tooltipImage;
//     }
// }
