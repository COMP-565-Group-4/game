using UnityEngine;

namespace UI {
public class UIController : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField]
    private GameObject hud;

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject startMenu;

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
