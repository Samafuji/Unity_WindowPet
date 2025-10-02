using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneScreenSetup : MonoBehaviour
{
    public Camera phoneCamera; // スマホの画面に映し出すカメラ
    public MeshRenderer phoneScreenRenderer; // スマホの画面部分のMeshRenderer
    public RenderTexture renderTexture; // 作成したRender Texture

    void Start()
    {
        // カメラのターゲットテクスチャをRender Textureに設定
        phoneCamera.targetTexture = renderTexture;

        // スマホの画面部分のマテリアルのメインテクスチャにRender Textureを設定
        phoneScreenRenderer.material.mainTexture = renderTexture;

        // カメラのCulling Maskから"HiddenLayer"を除外
        phoneCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("HiddenLayer"));

    }
}