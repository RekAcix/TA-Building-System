using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetailsScript : MonoBehaviour
{
    private Vector3 lockedPosition;

    private float rotationSpeed = 90f;

    public void Initialize()
    {
        lockedPosition = transform.position;
        this.gameObject.layer = LayerMask.NameToLayer("Default");
    }

    public void Rotate(float multiplier)
    {
        transform.RotateAround(transform.position, Vector3.up, Time.deltaTime * rotationSpeed * multiplier);
    }

    public void Flip(float value)
    {
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        Quaternion newRotation = transform.rotation;
        eulerAngles.x += value;
        eulerAngles.y = 0;
        eulerAngles.z = 0;
        newRotation.eulerAngles = eulerAngles;
        transform.rotation = newRotation;
    }
}
