using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStat : MonoBehaviour
{
    [SerializeField] private Transform _StackPoint;

    [SerializeField] private int _RequiredStackID = 1;
    [SerializeField] private int _MaxCapacity = 30;
    [SerializeField] private int _MaxRows = 6;

    [SerializeField] private float _DropSpeed = 0.33f;

    [SerializeField] private float _RowOffset = 0.075f;
    [SerializeField] private float _ColumnOffset = 0.06f;

    //Pool System
    private Queue<StackItem> _StackPool = new Queue<StackItem>();

    [Header("DEBUG")]
    [SerializeField] private List<GameObject> _StoredInputs = new List<GameObject>();

    private void Start()
    {
        foreach (Transform stack in _StackPoint)
        {
            StackItem currentStack = stack.GetComponent<StackItem>();
            currentStack.StackObject.gameObject.SetActive(false);
            _StackPool.Enqueue(currentStack);
        }
    }

    public int GetCurrentItemAmount()
    {
        return _StoredInputs.Count;
    }

    public int GetMaxCapacity()
    {
        return _MaxCapacity;
    }

    public int GetRequiredStackID()
    {
        return _RequiredStackID;
    }

    public int GetMaxRows()
    {
        return _MaxRows;
    }

    public float GetDropSpeed()
    {
        return _DropSpeed;
    }

    public float GetRowOffset()
    {
        return _RowOffset;
    }

    public float GetColumnOffset()
    {
        return _ColumnOffset;
    }

    public GameObject AddInput(int _stackID)
    {
        if (_StackPool.Count == 0)
        {
            GameObject addStack = Instantiate(_StoredInputs[0].gameObject, _StoredInputs[0].transform.parent);
            StackItem currentStack = addStack.GetComponent<StackItem>();
            currentStack.SetID(_stackID);
            _StackPool.Enqueue(currentStack);
        }

        StackItem newStack = _StackPool.Dequeue();
        newStack.SetID(_stackID);
        newStack.transform.SetParent(_StackPoint);
        newStack.StackObject.gameObject.SetActive(true);

        _StoredInputs.Add(newStack.gameObject);
        UpdateStackPositions();

        return newStack.gameObject;
    }

    public bool RemoveInput(int _stackID)
    {
        if (_StoredInputs.Count == 0)
            return false;

        int length = _StoredInputs.Count;
        for (int i = length - 1; i >= 0; i--)
        {
            StackItem currentStack = _StoredInputs[i].GetComponent<StackItem>();
            if (currentStack.StackID == _stackID)
            {
                _StoredInputs.RemoveAt(i);

                // Return to pool
                currentStack.gameObject.SetActive(false);
                _StackPool.Enqueue(currentStack);
                return true;
            }
        }

        UpdateStackPositions();
        return false;
    }

    private void UpdateStackPositions()
    {
        for (int i = 0; i < _StoredInputs.Count; i++)
        {
            Vector2 offset = GetStackOffsetByIndex(i);

            _StoredInputs[i].transform.SetParent(_StackPoint);
            _StoredInputs[i].transform.localEulerAngles = new Vector3(0, -90, 0);
            _StoredInputs[i].transform.localPosition = new Vector3(offset.x, offset.y, 0);
        }
    }

    public Vector3 GetLastStackPosition()
    {
        if (_StoredInputs.Count > 0)
        {
            Vector2 offset = GetStackOffsetByIndex(_StoredInputs.Count - 1);
            return _StackPoint.TransformPoint(new Vector3(offset.x, offset.y, 0));
        }
        else
        {
            return _StackPoint.position;
        }
    }

    public Quaternion GetLastStackRotation()
    {
        Quaternion localRotation = Quaternion.Euler(0, -90, 0);
        return _StackPoint.rotation * localRotation;
    }

    public Vector2 GetStackOffsetByIndex(int _index)
    {
        int rowSize = _MaxRows;
        int row = _index / rowSize;
        int column = _index % rowSize;

        float xOffset = column * _ColumnOffset;
        float yOffset = row * _RowOffset;

        return new Vector2(xOffset, yOffset);
    }
}
