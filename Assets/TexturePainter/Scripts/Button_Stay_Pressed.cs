using UnityEngine;
using UnityEngine.UI;

public class ChangeButtonColor : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    public Color activeColor = Color.red;

    private int activeButtonIndex = 0;

    void Start()
    {

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnClickButton(index));
        }

        // initial button colors
        UpdateButtonColors();
    }

    void OnClickButton(int index)
    {
        activeButtonIndex = index;
        UpdateButtonColors();
    }

    void UpdateButtonColors()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == activeButtonIndex)
                buttons[i].GetComponent<Image>().color = activeColor;
            else
                buttons[i].GetComponent<Image>().color = Color.white;
        }
    }
}