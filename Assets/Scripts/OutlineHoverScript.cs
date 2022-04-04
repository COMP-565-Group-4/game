// This script is designed to be used with the QuickOutline asset.
// Make sure there's an Outline script on the same object!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineHoverScript : MonoBehaviour
{
    private bool _beingLookedAt;
    private Outline _outline;

    // Start is called before the first frame update
    void Start()
    {
        _beingLookedAt = false;
        _outline = GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_beingLookedAt)
        {
            _beingLookedAt = false;
            _outline.enabled = true;
        }
        else
        {
            _outline.enabled = false;
        }
    }

    void Hover()
    {
        _beingLookedAt = true;
    }
}
