using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform lookAt;

    private Vector3 desiredPosition;
    [SerializeField] private float offset = 1.5f;
    [SerializeField] private float distance = 3f;

    private void Update()
    {
        // Update position
        desiredPosition = lookAt.position + (-transform.forward * distance) + (transform.up * offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime);

        // Update the rotation
        transform.LookAt(lookAt.position + (Vector3.up * offset));
    }
}
