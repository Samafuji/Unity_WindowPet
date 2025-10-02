using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    public Material morningSkybox;
    public Material afternoonSkybox;
    public Material eveningSkybox;
    public Material nightSkybox;

    private Material currentSkybox;
    private float blendFactor;

    void Start()
    {
        Shader skyboxShader = Shader.Find("Skybox/Blended");
        if (skyboxShader == null)
        {
            Debug.LogError("Custom/BlendedSkybox shader not found. Ensure it is included in the project.");
            return;
        }

        currentSkybox = new Material(skyboxShader);
        RenderSettings.skybox = currentSkybox;
    }

    void Update()
    {
        if (currentSkybox == null)
        {
            return;
        }

        float timeOfDay = (Time.time / 10f) % 24f;
        Material fromSkybox;
        Material toSkybox;

        if (timeOfDay >= 6f && timeOfDay < 12f)
        {
            fromSkybox = morningSkybox;
            toSkybox = afternoonSkybox;
            blendFactor = (timeOfDay - 6f) / 6f;
        }
        else if (timeOfDay >= 12f && timeOfDay < 18f)
        {
            fromSkybox = afternoonSkybox;
            toSkybox = eveningSkybox;
            blendFactor = (timeOfDay - 12f) / 6f;
        }
        else if (timeOfDay >= 18f && timeOfDay < 21f)
        {
            fromSkybox = eveningSkybox;
            toSkybox = nightSkybox;
            blendFactor = (timeOfDay - 18f) / 3f;
        }
        else
        {
            fromSkybox = nightSkybox;
            toSkybox = morningSkybox;
            blendFactor = (timeOfDay < 6f) ? (timeOfDay / 6f) : ((timeOfDay - 21f) / 3f);
        }

        // Ensure fromSkybox and toSkybox are assigned before calling Lerp
        if (fromSkybox != null && toSkybox != null)
        {
            currentSkybox.SetTexture("_Tex1", fromSkybox.GetTexture("_Tex"));
            currentSkybox.SetTexture("_Tex2", toSkybox.GetTexture("_Tex"));
            currentSkybox.SetFloat("_Blend", blendFactor);
            DynamicGI.UpdateEnvironment();
        }
    }
}
