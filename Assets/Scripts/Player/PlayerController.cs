using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterStat _Stat;
    [SerializeField] private Animator _Animator;

    private readonly int IsRunningHash = Animator.StringToHash("isRunning");

    [Header("DEBUG")]
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
                _Stat.SetMoveDirection(new Vector2(touchDelta.x, touchDelta.y));
            }
        }
        else
        {
            _Stat.SetMoveDirection(Vector2.zero);
        }
    }

    void MovePlayer()
    {
        Vector2 moveDirection = _Stat.GetMoveDiraction();
        Vector2 velocity = _Stat.GetVelocity();
        if (moveDirection != Vector2.zero)
        {
            velocity += moveDirection * _Stat.GetAcceleration() * Time.deltaTime;
            velocity = Vector2.ClampMagnitude(velocity, _Stat.GetMaxSpeed());
            SetRunningState(true);
        }
        else
        {
            velocity = Vector2.Lerp(velocity, Vector2.zero, _Stat.GetFriction() * Time.deltaTime);
            if (velocity.magnitude < 0.1f)
            {
                velocity = Vector2.zero;
                SetRunningState(false);
            }
        }

        _Stat.SetVelocity(velocity);
        transform.position += new Vector3(velocity.x, 0, velocity.y) * Time.deltaTime;
    }

    void RotatePlayer()
    {
        Vector2 velocity = _Stat.GetVelocity();
        Vector3 moveDir = new Vector3(velocity.x, 0, velocity.y).normalized;
        if (moveDir.magnitude > 0.1f) // Avoid unnecessary rotation when velocity is too low
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _Stat.GetRotationSpeed() * Time.deltaTime);
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
