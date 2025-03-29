using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharController : MonoBehaviour
{
    [Header("Focus Stacks")]
    [Tooltip("If you set IDs, the character will collect only that IDs")]
    [SerializeField] private List<int> _FocusStackIDs = new();

    public abstract bool CanInteract(MachineController _machineController);
    public int[] GetFocusIDs()
    {
        return _FocusStackIDs.ToArray();
    }
}
