using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStat))]
[RequireComponent(typeof(StackController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController _Controller;
    [SerializeField] private CharacterStat _Stat;
    [SerializeField] private StackController _StackController;
    [SerializeField] private Animator _Animator;

    private readonly int IsRunningHash = Animator.StringToHash("isRunning");

    [Header("DEBUG")]
    [SerializeField] private bool _IsTouching;
    [SerializeField] private bool _IsRunning;
    [SerializeField] private bool _IsInteracting;

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

        // Use CharacterController for movement
        Vector3 moveVector = new Vector3(velocity.x, 0, velocity.y) * Time.deltaTime;
        _Controller.Move(moveVector);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable _interactable))
        {
            if (_interactable.IsCollectable())
            {
                StorageController interactedStorage = _interactable.GetCollectable().GetStorage();
                if (interactedStorage != null && !_IsInteracting)
                {
                    _IsInteracting = true;
                    _StackController.CollectItemsStart(interactedStorage);
                }
            }
            else if (_interactable.IsDroppable())
            {
                IDroppable droppable = _interactable.GetDroppable();
                int index = droppable.GetInputIndex();

                InputController interactedInput = _interactable.GetDroppable().GetInputController();

                InputStat stat = interactedInput.GetStatByIndex(index);
                if (interactedInput != null && !_IsInteracting)
                {
                    _IsInteracting = true;
                    _StackController.ItemDropStart(interactedInput, stat);
                }
            }
            else if (_interactable.IsDestructible())
            {
                TrashController interactedTrash = _interactable.GetDestructible().GetTrashController();

                TrashStat stat = interactedTrash.GetStat();
                if (interactedTrash != null && !_IsInteracting)
                {
                    _IsInteracting = true;
                    _StackController.ItemDropStart(interactedTrash, stat);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable _interactable))
        {
            if (_IsInteracting)
            {
                _IsInteracting = false;
                _StackController.CollectItemsStop();
            }
        }
    }
}
