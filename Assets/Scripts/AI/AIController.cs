using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : CharController
{
    [Header("Required Components")]
    [SerializeField] private Animator _Animator;
    [SerializeField] private CharacterStat _Stat;
    [SerializeField] private NavMeshAgent _Agent;
    [SerializeField] private StackController _StackController;

    [Header("Target List")]
    [Tooltip("You can set this in runtime ( Use => SetWork(List<MachineController> _) ) or can set a preloaded job circle")]
    [SerializeField] private AITarget _AITarget;

    [SerializeField] private float _JobStateTimeOutInSeconds = 8;
    private readonly int IsRunningHash = Animator.StringToHash("isRunning");

    [Header("DEBUG")]
    [SerializeField] private int _TargetIndex;
    [SerializeField] private float _JobStateTimer;
    [SerializeField] private AIState _AIState;
    [SerializeField] private AIJobState _JobState;

    [SerializeField] private bool _IsRunning;

    private void Start()
    {
        _TargetIndex = 0;

        _Agent.speed = _Stat.GetMaxSpeed();
        _Agent.acceleration = _Stat.GetAcceleration();

        if (_AITarget.TargetDatas != null && _AITarget.TargetDatas.Count > 0)
            SetWork(_AITarget, true);
    }

    public void SetWork(AITarget _aiMachines, bool _instantMove)
    {
        if(_aiMachines.TargetDatas == null || _aiMachines.TargetDatas.Count == 0)
        {
            Debug.LogError("Can not assing job without machines!");
            return;
        }

        _TargetIndex = 0;

        _AITarget = _aiMachines;
        if (!_instantMove)
        {
            _TargetIndex = 0;
            SetNewState(AIState.Move);
            _Agent.SetDestination(_AITarget.GetPosition(_TargetIndex));
        }
        else
        {
            var target = _AITarget.GetTargetWithStateByIndex(0);
            _JobState = target._jobState;

            _Agent.enabled = false; //Need to discard Agent Component for teleportation.
            transform.position = target._pos.position;
            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x, 0.5f, pos.z);

            //_TargetIndex = _AITarget.TargetDatas.Count == 1 ? 0 : 1;
            _TargetIndex = 0;

            SetNewState(AIState.Interact);
        }
    }

    public void SetNewState(AIState _newState)
    {
        AIState _previousState = _AIState; //Just in case if need later.

        if (_newState == AIState.Idle)
        {
            _Stat.SetVelocity(Vector3.zero);
            _Stat.SetMoveDirection(Vector3.zero);

            SetRunningState(false);
            _Agent.enabled = false;
        }
        else if (_newState == AIState.Move)
        {
            var target = _AITarget.GetTargetWithStateByIndex(_TargetIndex);

            _Agent.enabled = true;
            _Agent.SetDestination(target._pos.position);

            SetRunningState(true);
            _JobState = target._jobState;
        }
        else if (_newState == AIState.Interact)
        {
            _Stat.SetVelocity(Vector3.zero);
            _Stat.SetMoveDirection(Vector3.zero);

            SetRunningState(false);
            _Agent.enabled = false;

            _JobStateTimer = Time.time + _JobStateTimeOutInSeconds;
        }

        _AIState = _newState;
    }

    public void UpdateBehaviour()
    {
        
        switch (_AIState)
        {
            case AIState.Idle:
                Idle();
                break;
            case AIState.Move:
                Move();
                break;
            case AIState.Interact:
                if (_JobStateTimer < Time.time)
                {
                    Debug.Log("Time out, NPC can not completed interact but will continue the circle.", transform);
                    SetNextTarget();
                    SetNewState(AIState.Move);
                    return;
                }
                Interact();
                break;
            default:
                break;
        }
    }

    private void Idle()
    {
        Debug.Log("AI does not work. Who? Click me!", transform);
    }

    private void Move()
    {
        if(_AITarget.TargetDatas == null || _AITarget.TargetDatas.Count == 0)
        {
            SetNewState(AIState.Idle);
            return;
        }

        Vector3 velocity = _Agent.velocity * 3f;

        _Stat.SetVelocity(velocity);
        _Stat.SetMoveDirection(velocity.normalized);

        Transform aiWaitPoint = _AITarget.GetAIWaitPoints(_TargetIndex);

        float distance = Vector3.Distance(transform.position, aiWaitPoint.position);
        if (distance <= 0.1f)
        {
            Debug.Log("AI arrived to collect point!");
            SetNewState(AIState.Interact);
        }
    }

    private void SetNextTarget()
    {
        Transform aiWaitPoints = _AITarget.GetAIWaitPoints(_TargetIndex);
        int machineCount = _AITarget.TargetDatas.Count;

        _TargetIndex++;
        if (_TargetIndex >= machineCount)
            _TargetIndex = 0;
    }

    private void Interact()
    {
        if (_JobState == AIJobState.Collect)
        {
            if (!_StackController.CanCarryMore() || !_AITarget.HasProduct(_TargetIndex))
            {
                SetNextTarget();
                SetNewState(AIState.Move);
            }
        }
        else if (_JobState == AIJobState.Drop)
        {
            bool has = false;
            int[] _ids = _AITarget.GetRequiredID(_TargetIndex);
            foreach (var id in _ids)
            {
                if (_StackController.GetItem(id) != null)
                {
                    if (_AITarget.HasSpace(_TargetIndex))
                        has = true;

                    break;
                }
            }

            if(!has)
            {
                SetNextTarget();
                SetNewState(AIState.Move);
            }
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

    public override bool CanInteract(MachineController _machineController)
    {
        foreach (var item in _AITarget.TargetDatas)
            if (item.TargetMachine == _machineController)
                return true;

        return false;
    }
}

[System.Serializable]
public struct AITarget
{
    public List<AITargetData> TargetDatas;

    public Vector3 GetPosition(int _targetIndex)
    {
        return TargetDatas[_targetIndex].TargetMachine.transform.position;
    }

    public (Transform _pos, AIJobState _jobState) GetTargetWithStateByIndex(int _targetIndex)
    {
        return TargetDatas[_targetIndex].GetTargetWithStateByIndex();
    }

    public Transform GetAIWaitPoints(int _targetIndex)
    {
        return TargetDatas[_targetIndex].GetAIWaitPoints();
    }

    public bool HasSpace(int _targetIndex)
    {
        return TargetDatas[_targetIndex].HasSpace();
    }

    public bool HasProduct(int _targetIndex)
    {
        return TargetDatas[_targetIndex].HasProduct();
    }

    public int[] GetRequiredID(int _targetIndex)
    {
        return TargetDatas[_targetIndex].GetRequiredID();
    }
}

[System.Serializable]
public struct AITargetData
{
    [Header("Target Machine")]
    public MachineController TargetMachine;

    [Header("Index of Target")]
    public int Index;

    public (Transform _pos, AIJobState _jobState) GetTargetWithStateByIndex()
    {
        return TargetMachine.GetTargetWithStateByIndex(Index);
    }

    public Transform GetAIWaitPoints()
    {
        return TargetMachine.GetAIWaitPoints()[Index];
    }

    public bool HasSpace()
    {
        return TargetMachine.HasSpace(Index);
    }

    public int[] GetRequiredID()
    {
        return TargetMachine.GetRequiredID(Index);
    }

    public bool HasProduct()
    {
        return TargetMachine.HasProduct(Index);
    }
}
