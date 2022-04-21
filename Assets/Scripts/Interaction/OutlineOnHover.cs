using UnityEngine;

namespace Interaction {
public class OutlineOnHover : MonoBehaviour
{
    public Outline outline;

    private bool isHovered = false;

    protected virtual void Update()
    {
        outline.enabled = isHovered;
        isHovered = false;
    }

    protected virtual void OnHover()
    {
        isHovered = true;
    }
}
}
