using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStat : MonoBehaviour
{
    [SerializeField] private float _Acceleration = 20f;
    [SerializeField] private float _MaxSpeed = 1f;
    [SerializeField] private float _Friction = 20f;
    [SerializeField] private float _RotationSpeed = 10f;

    [SerializeField] private Vector2 _MoveDirection;
    [SerializeField] private Vector2 _Velocity;
    public float GetAcceleration()
    {
        return _Acceleration;
    }

    public float GetMaxSpeed()
    {
        return _MaxSpeed;
    }

    public float GetFriction()
    {
        return _Friction;
    }

    public float GetRotationSpeed()
    {
        return _RotationSpeed;
    }

    public Vector2 GetMoveDiraction()
    {
        return _MoveDirection;
    }

    public Vector2 GetVelocity()
    {
        return _Velocity;
    }

    public void SetMoveDirection(Vector2 _moveDirection)
    {
        _MoveDirection = _moveDirection;
    }

    public void SetVelocity(Vector2 _velocity)
    {
        _Velocity = _velocity;
    }
}
