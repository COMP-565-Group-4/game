// Copyright 2021 by Hextant Studios. https://HextantStudios.com
// This work is licensed under CC BY 4.0. http://creativecommons.org/licenses/by/4.0/

using UnityEngine;

namespace Utils {
// A simple MonoBehaviour-based singleton for use at runtime.
// Note: This implementation does not support the "Recompile and Continue Playing"
// editor preference!
public class Singleton<T> : MonoBehaviour
    where T : Singleton<T>
{
    // The singleton instance.
    public static T Instance { get; private set; }

    // Called when the instance is created.
    protected virtual void Awake()
    {
        // Verify there is not more than one instance and assign _instance.
        Debug.Assert(Instance == null, "More than one singleton instance instantiated!", this);
        Instance = (T) this;
    }

    // Clear the instance field when destroyed.
    protected virtual void OnDestroy() => Instance = null;
}
}
