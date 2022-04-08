using StarterAssets;

using UnityEngine;

namespace UI {
public class UIController : MonoBehaviour
{
    public StarterAssetsInputs inputs;

    [Header("Canvases")]
    public GameObject hud;
    public GameObject pauseMenu;
    public GameObject startMenu;

    private void Update()
    {
        pauseMenu.SetActive(inputs.isPaused);
    }
}
}
