using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MachineController : MonoBehaviour, IAITarget
{
    [SerializeField] private AISetting _AISetting;

    public abstract int[] GetRequiredID(int _indexIfMoreThanOne = 0);

    public abstract bool HasSpace(int _indexIfMoreThanOne = 0);

    public abstract bool HasProduct(int _indexIfMoreThanOne = 0);

    public List<Transform> GetAIWaitPoints()
    {
        return _AISetting.AIWaitPoints;
    }

    public (Transform _pos, AIJobState _jobState) GetTargetWithStateByIndex(int _index)
    {
        return (_AISetting.AIWaitPoints[_index], _AISetting.AIJobStates[_index]);
    }

    [System.Serializable]
    public struct AISetting
    {
        public List<Transform> AIWaitPoints;
        public List<AIJobState> AIJobStates;
    }
}
