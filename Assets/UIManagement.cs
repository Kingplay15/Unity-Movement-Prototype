using UnityEngine;
using UnityEngine.UI;

public class UIManagement : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI currentMode;

    public void UpdateMode(int mode)
    {
        switch (mode)
        { 
            case 1:
                currentMode.text = "Arcade";
                break;
            case 2:
                currentMode.text = "Platformer";
                break;
            case 3:
                currentMode.text = "Physics";
                break;
        }
    }
}
