using UnityEngine;
using UnityEngine.UI;

public class BrushSelector : MonoBehaviour
{
    [SerializeField] private GameObject texturePainter; 

    private TexturePainter texturePainterScript;

    void Start()
    {
        texturePainterScript = texturePainter.GetComponent<TexturePainter>();
    }

    public void SelectBrush(string brushName)
    {
        texturePainterScript.SetBrush(brushName);
    }
}
