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
    [Tooltip("Image component for the crosshair sprite")]
    public Image CrosshairImage;

    [Tooltip("Sprite to display for the cross crosshair type")]
    public Sprite CrossSprite;

    [Tooltip("Sprite to display for the dot crosshair type")]
    public Sprite DotSprite;

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
            CrosshairImage.sprite = value switch {
                CrosshairType.Cross => CrossSprite,
                CrosshairType.Dot => DotSprite,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }
    }

    public Color Colour
    {
        get => CrosshairImage.color;
        set => CrosshairImage.color = value;
    }

    public float Size
    {
        get => CrosshairImage.rectTransform.sizeDelta.x;
        set => CrosshairImage.rectTransform.sizeDelta = new Vector2(value, value);
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
