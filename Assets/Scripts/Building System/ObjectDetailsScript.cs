using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetailsScript : MonoBehaviour
{
    private Vector3 lockedPosition;

    public void Initialize()
    {
        lockedPosition = transform.position;
        this.gameObject.layer = LayerMask.NameToLayer("Default");
    }
}
