using TMPro;

using UnityEngine;

namespace UI {
public class HUDManager : MonoBehaviour
{
    [Header("Text")]
    [Tooltip("TMP component for the timer text")]
    public TextMeshProUGUI TimeText;

    [Tooltip("TMP component for the round text")]
    public TextMeshProUGUI RoundText;

    [Tooltip("TMP component for the money text")]
    public TextMeshProUGUI MoneyText;

    [Header("Values")]
    [SerializeField]
    [Tooltip("Current round")]
    private uint round;

    [SerializeField]
    [Tooltip("Total number of rounds")]
    private uint totalRounds;

    [SerializeField]
    [Tooltip("Amount of money the player possesses")]
    private uint money;

    [Header("Time")]
    [SerializeField]
    [Tooltip("Minutes remaining for the current round")]
    private uint minutes;

    [SerializeField]
    [Tooltip("Seconds remaining for the current round")]
    private uint seconds;

    public uint Round
    {
        get => round;
        set {
            round = value;
            RoundText.text = $"{value}/{TotalRounds}";
        }
    }

    public uint TotalRounds
    {
        get => totalRounds;
        set {
            totalRounds = value;
            RoundText.text = $"{Round}/{value}";
        }
    }

    public uint Money
    {
        get => money;
        set {
            money = value;
            MoneyText.text = value.ToString();
        }
    }

    public uint Minutes
    {
        get => minutes;
        set {
            minutes = value;
            TimeText.text = $"{value:00}:{Seconds:00}";
        }
    }

    public uint Seconds
    {
        get => seconds;
        set {
            seconds = value;
            TimeText.text = $"{Minutes:00}:{value:00}";
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Triggers setters of properties when a backing field is updated via the inspector.
    /// </summary>
    protected void OnValidate()
    {
        Round = round;
        TotalRounds = totalRounds;
        Money = money;
        Minutes = minutes;
        Seconds = seconds;
    }
#endif
}
}
