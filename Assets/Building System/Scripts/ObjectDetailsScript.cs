using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetailsScript : MonoBehaviour
{
    private Vector3 lockedPosition;

    private float rotationSpeed = 90f;
    private Color savedColor;
    private Vector3 offset;

    public void Initialize()
    {
        lockedPosition = transform.position;
        this.gameObject.layer = LayerMask.NameToLayer("Default");
        savedColor = GetComponent<MeshRenderer>().material.color;
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

    public void CopyDetails(ObjectDetailsScript otherDetails)
    {
        lockedPosition = otherDetails.GetLockedPosition();
        savedColor = otherDetails.GetSavedColor();
        offset = otherDetails.GetOffset();
    }

    public void UpdateLockedPosition()
    {
        lockedPosition = transform.position;
    }

    public void CalculateOffset(Vector3 mousePosition)
    {
        offset = lockedPosition - mousePosition;
    }

    public void CalculateCurrentOffset(Vector3 mousePosition)
    {
        offset = transform.position - mousePosition;
    }

    public Vector3 GetOffset()
    {
        return offset;
    }

    public Color GetSavedColor()
    {
        return savedColor;
    }

    public Vector3 GetLockedPosition()
    {
        return lockedPosition;
    }
}
