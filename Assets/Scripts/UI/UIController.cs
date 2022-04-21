using UnityEngine;

namespace UI {
public class UIController : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject hud;
    public GameObject pauseMenu;
    public GameObject startMenu;

    public void PauseEventHandler()
    {
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeEventHandler()
    {
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
}
}
