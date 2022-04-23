using System;

using UnityEngine;
using UnityEngine.UI;

namespace UI {
public enum CrosshairType
{
    Cross = 0,
    Dot = 1
}

public class CrosshairManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Image component for the crosshair sprite")]
    private Image crosshairImage;

    [SerializeField]
    [Tooltip("Sprite to display for the cross crosshair type")]
    private Sprite crossSprite;

    [SerializeField]
    [Tooltip("Sprite to display for the dot crosshair type")]
    private Sprite dotSprite;

    [SerializeField]
    [Tooltip("The kind of sprite to display for the crosshair")]
    private CrosshairType type = CrosshairType.Cross;

    [SerializeField]
    [Tooltip("The colour of the crosshair")]
    private Color colour = Color.white;

    public CrosshairType Type
    {
        get => type;
        set {
            type = value;
            crosshairImage.sprite = value switch {
                CrosshairType.Cross => crossSprite,
                CrosshairType.Dot => dotSprite,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }
    }

    public Color Colour
    {
        get => crosshairImage.color;
        set => crosshairImage.color = value;
    }

    public float Size
    {
        get => crosshairImage.rectTransform.sizeDelta.x;
        set => crosshairImage.rectTransform.sizeDelta = new Vector2(value, value);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Triggers setters of properties when a backing field is updated via the inspector.
    /// </summary>
    protected void OnValidate()
    {
        Type = type;
        Colour = colour;
    }
#endif
}
}
