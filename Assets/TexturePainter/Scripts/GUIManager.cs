using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GUIManager : MonoBehaviour
{
    [SerializeField] private Slider sizeSlider;
    [SerializeField] private Slider intensitySlider;
    [SerializeField] private TexturePainter painter;

    public void UpdateSizeSlider()
    {
        if (painter != null && sizeSlider != null)
        {
            painter.SetBrushSize(sizeSlider.value);
        }
        else
        {
            Debug.LogWarning("Painter or SizeSlider is not assigned.");
        }
    }

    public void UpdateIntensitySlider()
    {
        if (painter != null && sizeSlider != null)
        {
            painter.SetBrushIntensity(intensitySlider.value);
        }
        else
        {
            Debug.LogWarning("Painter or IntensitySlider is not assigned.");
        }
    }

    public void OnSaveButtonClick()
    {
        painter.SaveTexture(); 
    } 
    
    public void OnLoadButtonClick()
    {
        painter.LoadTextureFromFile(); 
    }

}
