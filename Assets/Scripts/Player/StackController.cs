using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackController : MonoBehaviour
{
    [SerializeField] private Transform _StackPoint;

    [SerializeField] private int _MaxCapacity = 30;
    [SerializeField] private int _TiltCurveEffectStart = 3;

    [SerializeField] private float _StackSpacing = 0.05f;
    [SerializeField] private float _MaxTiltAmount = 0.2f;

    [SerializeField] private AnimationCurve _TiltCurve;

    [SerializeField] private List<Transform> _StackedItems = new List<Transform>();
    [SerializeField] private CharacterStat _Stat;

    [SerializeField] private Queue<GameObject> _StackPool = new Queue<GameObject>();

    private void Start()
    {
        foreach (Transform stack in _StackPoint)
        {
            stack.gameObject.SetActive(false);
            _StackPool.Enqueue(stack.gameObject);
        }
    }
    private void Update()
    {
        if (_StackedItems.Count == 0)
            return;

        AdjustStackTilt();
    }

    public void AddStack()
    {
        if (_StackedItems.Count >= _MaxCapacity || _StackPool.Count == 0)
            return;

        GameObject newWood = _StackPool.Dequeue();
        newWood.transform.SetParent(_StackPoint);
        newWood.SetActive(true);

        _StackedItems.Add(newWood.transform);
    }

    public void RemoveStack()
    {
        if (_StackedItems.Count == 0)
            return;

        GameObject woodToRemove = _StackedItems[_StackedItems.Count - 1].gameObject;
        _StackedItems.RemoveAt(_StackedItems.Count - 1);

        // Return to pool
        woodToRemove.SetActive(false);
        _StackPool.Enqueue(woodToRemove);
    }

    private void AdjustStackTilt()
    {
        if (_StackedItems.Count == 0)
            return;

        float velocityMagnitude = _Stat.GetVelocity().magnitude;
        float tiltFactor = Mathf.Lerp(0, _MaxTiltAmount, velocityMagnitude / _Stat.GetMaxSpeed());

        for (int i = 0; i < _StackedItems.Count; i++)
        {
            float curveFactor = _TiltCurve.Evaluate((float)i / _StackedItems.Count);
            float zOffset = -tiltFactor * curveFactor;
            float yOffset = i * _StackSpacing;

            _StackedItems[i].transform.localPosition = new Vector3(0, yOffset, _StackedItems.Count > _TiltCurveEffectStart ? zOffset : 0);
        }
    }


#if UNITY_EDITOR
    public void UpdateStacks()
    {
        AdjustStackTilt();
        UpdateStackPositions();
    }

    private void UpdateStackPositions()
    {
        _StackedItems = new();
        foreach (Transform item in _StackPoint)
            _StackedItems.Add(item);
        
        for (int i = 0; i < _StackedItems.Count; i++)
        {
            _StackedItems[i].transform.localPosition = new Vector3(0, i * _StackSpacing, 0);
        }

        _StackedItems = new();
    }

    public void AddEditorStack()
    {
        AddStack();
    }

    public void RemoveEditorStack()
    {
        RemoveStack();
    }

#endif
}
