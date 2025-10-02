using UnityEngine;
using UnityEngine.UI;
// plane distance 3.79 --> 11.08
public class PanelTransparencyController : MonoBehaviour
{
    public Image panelImage; // Assign the Image component of the Panel
    public Slider transparencySlider; // Assign the Slider component used to adjust transparency

    void Start()
    {
        if (transparencySlider != null && panelImage != null)
        {
            transparencySlider.value = panelImage.color.a;
            transparencySlider.onValueChanged.AddListener(UpdatePanelTransparency);
        }
    }

    // Called when the slider value changes
    private void UpdatePanelTransparency(float value)
    {
        if (panelImage != null)
        {
            Color color = panelImage.color;
            color.a = value;
            panelImage.color = color;
        }
    }
}
