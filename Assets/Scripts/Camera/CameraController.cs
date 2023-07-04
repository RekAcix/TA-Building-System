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

    [SerializeField]
    private float zoomSpeed = 0.5f;

    [SerializeField]
    private float mouseRotSpeed = 2f;
    private float pitch = 30f;
    private float yaw = 0f;

    [SerializeField]
    private float minHeight = -5f;

    [SerializeField]
    private Vector2 rotationLimits;

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

        // Rotation Of camera via panning
        HandlePitchYaw();

        // Movement
        HandleMovement();

        // Handle Zoom
        HandleZoom();


        pitch = transform.eulerAngles.x;
        yaw = transform.eulerAngles.y;
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

        // Check to not clip camera underground and far above into the sky
        if (position.y <= minHeight)
        {
            position.y = minHeight;
        } else if (position.y >= maxZoom)
        {
            position.y = maxZoom;
        }
        transform.position = position;

    }

    private void HandlePitchYaw()
    {
        if (Input.GetMouseButton(2))
        {
            yaw += mouseRotSpeed * Input.GetAxis("Mouse X");
            pitch -= mouseRotSpeed * Input.GetAxis("Mouse Y");

            pitch = Mathf.Clamp(pitch, rotationLimits.x, rotationLimits.y);

            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }
    }

    private void HandleZoom()
    {
        float zoomVel = -zoomSpeed * Input.mouseScrollDelta.y;
        if (zoomVel != 0)
        {
            Vector3 position = transform.position;
            position.y += zoomVel;

            if (position.y >= minZoom && position.y <= maxZoom)
            {
                transform.position = position;
            }     
        }
    }
}
