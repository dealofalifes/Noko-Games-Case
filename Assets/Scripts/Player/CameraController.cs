using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _Target;
    [SerializeField] private Vector3 _Offset = new Vector3(0, 5, -5);
    [SerializeField] private float _SmoothSpeed = 5f;
    [SerializeField] private float _Distance = 0.5f;

    void LateUpdate()
    {
        if (!_Target) return;

        Vector3 desiredPosition = _Target.position + _Offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, _SmoothSpeed * Time.deltaTime);
        transform.LookAt(_Target.position + Vector3.up * _Distance);
    }
}
