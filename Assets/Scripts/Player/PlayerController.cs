using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _Acceleration = 1f;
    [SerializeField] private float _MaxSpeed = 1f;
    [SerializeField] private float _Friction = 20f;
    [SerializeField] private float _RotationSpeed = 10f;

    [SerializeField] private Animator _Animator;
    [SerializeField] private Transform _Mesh;

    private readonly int IsRunningHash = Animator.StringToHash("isRunning");

    [Header("DEBUG")]
    [SerializeField] private Vector2 _MoveDirection;
    [SerializeField] private Vector2 _Velocity;
    [SerializeField] private bool _IsTouching;
    [SerializeField] private bool _IsRunning;

    void Start()
    {
        _Animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        MovePlayer();
        RotatePlayer();
    }

    void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _IsTouching = true;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                _IsTouching = false;
            }

            if (_IsTouching && touch.phase == TouchPhase.Moved)
            {
                Vector2 touchDelta = touch.deltaPosition.normalized; // Get finger movement direction
                _MoveDirection = new Vector2(touchDelta.x, touchDelta.y);
            }
        }
        else
        {
            _MoveDirection = Vector2.zero;
        }
    }

    void MovePlayer()
    {
        if (_MoveDirection != Vector2.zero)
        {
            _Velocity += _MoveDirection * _Acceleration * Time.deltaTime;
            _Velocity = Vector2.ClampMagnitude(_Velocity, _MaxSpeed);
            SetRunningState(true);
        }
        else
        {
            _Velocity = Vector2.Lerp(_Velocity, Vector2.zero, _Friction * Time.deltaTime);
            if (_Velocity.magnitude < 0.1f)
            {
                _Velocity = Vector2.zero;
                SetRunningState(false);
            }
        }

        transform.position += new Vector3(_Velocity.x, 0, _Velocity.y) * Time.deltaTime;
    }

    void RotatePlayer()
    {
        Vector3 moveDir = new Vector3(_Velocity.x, 0, _Velocity.y).normalized;
        if (moveDir.magnitude > 0.1f) // Avoid unnecessary rotation when velocity is too low
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _RotationSpeed * Time.deltaTime);
        }
    }

    void SetRunningState(bool state)
    {
        if (_IsRunning != state) // Only update if the state changes
        {
            _IsRunning = state;
            _Animator.SetBool(IsRunningHash, state);
        }
    }
}
