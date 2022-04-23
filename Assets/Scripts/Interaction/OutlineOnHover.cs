using UnityEngine;

namespace Interaction {
public class OutlineOnHover : MonoBehaviour
{
    [SerializeField]
    protected Outline Outline;

    private bool isHovered = false;

    protected virtual void Update()
    {
        Outline.enabled = isHovered;
        isHovered = false;
    }

    protected virtual void OnHover()
    {
        isHovered = true;
    }
}
}
