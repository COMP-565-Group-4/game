using UnityEngine;

public class OutlineOnHover : MonoBehaviour
{
    public Outline outline;
    private bool isHovered = false;

    void Update()
    {
        outline.enabled = isHovered;
        isHovered = false;
    }

    void OnHover()
    {
        isHovered = true;
    }
}
