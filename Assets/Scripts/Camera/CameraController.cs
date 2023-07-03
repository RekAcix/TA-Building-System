using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // This class should be placed on the Camera Container object

    [SerializeField]
    private float rotationSpeed = 1f;

    [SerializeField]
    private float movementSpeed = 0.5f;

    [SerializeField]
    private float minZoom = 5f;

    [SerializeField]
    private float maxZoom = 10f;

    private Camera mainCamera;

    void Awake()
    {
        // Get references to main camera object
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Change controls later on to the newer Unity Input System

        // Rotation
        HandleRotation();

        // Movement
        HandleMovement();

    }

    private void HandleRotation()
    {
        Vector3 rotation = transform.rotation.eulerAngles;

        if (Input.GetKey(KeyCode.Q))
        {
            rotation.y = rotation.y -= rotationSpeed;
        }

        if (Input.GetKey(KeyCode.E))
        {
            rotation.y = rotation.y += rotationSpeed;
        }

        Quaternion newRotationQuaternion = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        transform.rotation = newRotationQuaternion;
    }

    private void HandleMovement()
    {

        Vector3 position = transform.position;

        if (Input.GetKey(KeyCode.W))
        {
            position += transform.TransformDirection(Vector3.forward) * movementSpeed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            position -= transform.TransformDirection(Vector3.forward) * movementSpeed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            position += transform.TransformDirection(Vector3.right) * movementSpeed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            position -= transform.TransformDirection(Vector3.right) * movementSpeed;
        }

        transform.position = position;

    }
}
