using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] private List<MachineController> _Targets = new();

    [SerializeField] private List<AIController> _NPCs = new();

    [Header("Update AIs Time in Seconds")]
    [SerializeField] private float _UpdateDelayInSeconds = 3;

    [Header("DEBUG")]
    [SerializeField] private float _UpdateTimer;
    private void Update() //Handling many AI with just 1 update with a delayed system saves lots of resources!
    {
        if (_UpdateTimer <= Time.time)
        {
            foreach (var item in _NPCs)
            {
                item.UpdateBehaviour();
            }

            _UpdateTimer = Time.time + _UpdateDelayInSeconds;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _Targets = FindObjectsOfType<MachineController>().ToList();
        _NPCs = FindObjectsOfType<AIController>().ToList();
    }
#endif
}

public enum AIState
{
    Idle,
    Move,
    Interact,
}

public enum AIJobState
{
    Free,
    Collect,
    Drop,
}
