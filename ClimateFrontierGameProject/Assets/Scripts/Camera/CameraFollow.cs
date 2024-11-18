using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 _offset;
    private Transform target;
    private float smoothTime = 0.3f;
    private Vector3 _currentVelocity = Vector3.zero;

    private void Start()
    {
        // Optionally, try to find the player at Start
        FindPlayer();
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            // Try to find the player if target is null
            FindPlayer();
            if (target == null)
                return; // Exit if still null
        }

        Vector3 targetPosition = target.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        _offset = transform.position - target.position;
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SetTarget(player.transform);
        }
    }
}
