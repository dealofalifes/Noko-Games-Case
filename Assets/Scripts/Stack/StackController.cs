using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StackController : MonoBehaviour
{
    [SerializeField] private Transform _StackPoint;

    [SerializeField] private int _TiltCurveEffectStart = 3;

    [SerializeField] private float _StackSpacing = 0.05f;
    [SerializeField] private float _MaxTiltAmount = 0.2f;

    [SerializeField] private AnimationCurve _TiltCurve;

    [SerializeField] private List<StackItem> _StackedItems = new List<StackItem>();
    [SerializeField] private CharacterStat _Stat;

    //Pool System
    private Queue<StackItem> _StackPool = new Queue<StackItem>();

    [Header("DEBUG")]
    [SerializeField] private bool _IsInteracting = false;

    private Coroutine _CollectionRoutine;
    private Coroutine _DropRoutine;

    private void Start()
    {
        foreach (Transform stack in _StackPoint)
        {
            StackItem currentStack = stack.GetComponent<StackItem>();
            currentStack.StackObject.gameObject.SetActive(false);
            _StackPool.Enqueue(currentStack);
        }
    }
    private void Update()
    {
        if (_StackedItems.Count == 0)
            return;

        AdjustStackTilt();
    }

    public void AddStack(int _stackID)
    {
        if (_StackedItems.Count >= _Stat.GetCapacity())
            return;

        if (_StackPool.Count == 0)
        {
            GameObject addStack = Instantiate(_StackedItems[0].gameObject, _StackedItems[0].StackObject.parent);
            StackItem currentStack = addStack.GetComponent<StackItem>();
            currentStack.SetID(_stackID);
            _StackPool.Enqueue(currentStack);
        }

        StackItem newStack = _StackPool.Dequeue();
        newStack.SetID(_stackID);
        newStack.transform.SetParent(_StackPoint);
        newStack.StackObject.gameObject.SetActive(true);

        _StackedItems.Add(newStack);
    }

    public bool RemoveStack(int _stackID)
    {
        if (_StackedItems.Count == 0)
            return false;

        int length = _StackedItems.Count;
        for (int i = length - 1; i >= 0; i--)
        {
            StackItem currentStack = _StackedItems[i].GetComponent<StackItem>();
            if (currentStack.StackID == _stackID)
            {
                _StackedItems.RemoveAt(i);

                // Return to pool
                currentStack.gameObject.SetActive(false);
                _StackPool.Enqueue(currentStack);
                return true;
            }
        }

        return false;
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
            _StackedItems[i].transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    public void CollectItemsStart(StorageController _interactedStorage)
    {
        CollectItemsStop();
        _CollectionRoutine = StartCoroutine(CollectItems(_interactedStorage));
    }

    public void CollectItemsStop()
    {
        if (_CollectionRoutine != null)
            StopCoroutine(_CollectionRoutine);

        _IsInteracting = false;
    }

    private IEnumerator CollectItems(StorageController _interactedStorage)
    {
        _IsInteracting = true;
        float collectDelay = _Stat.GetCollectDelay();
        while (_IsInteracting)
        {
            if (_interactedStorage == null)
                break;

            while (!CanCarryMore()) //Has enough space?
            {
                yield return new WaitForSeconds(collectDelay);
                continue; //Retry until exit the trigger area.
            }

            GameObject item = _interactedStorage.TakeProduct();
            while (item == null) //Has output to collect?
            {
                yield return new WaitForSeconds(collectDelay);
                item = _interactedStorage.TakeProduct();
                continue; //Retry until exit the trigger area.
            }

            StartCoroutine(MoveToCarryPosition(item));

            yield return new WaitForSeconds(collectDelay);
        }
        _IsInteracting = false;
    }

    private IEnumerator MoveToCarryPosition(GameObject item)
    {
        Vector3 startPos = item.transform.position;
        Quaternion startRot = item.transform.rotation;

        float elapsedTime = 0;
        float duration = 0.5f;

        float moveSpeed = _Stat.GetCollectMoveSpeed();
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime * moveSpeed;

            item.transform.position = Vector3.Lerp(startPos, 
                _StackedItems.Count > 0 ? (_StackedItems[_StackedItems.Count - 1].StackObject.position + Vector3.up * 0.2f) : _StackPoint.position, 
                elapsedTime / duration);

            item.transform.rotation = Quaternion.Lerp(startRot ,_StackedItems.Count > 0
                        ? Quaternion.LookRotation(_StackedItems[_StackedItems.Count - 1].StackObject.forward)
                        : _StackPoint.rotation, 
                        elapsedTime / duration);

            yield return null;
        }

        item.SetActive(false);
        item.transform.localPosition = Vector3.zero;
        AddStack(item.GetComponent<StackItem>().StackID);
    }

    public bool CanCarryMore()
    {
        return _StackedItems.Count < _Stat.GetCapacity();
    }

    public StackItem GetItem(int _stackID)
    {
        int length = _StackedItems.Count;
        for (int i = length - 1; i >= 0; i--) //Start from the end to get item from top first.
            if (_StackedItems[i].StackID == _stackID)
                return _StackedItems[i];

        return null;
    }

    public StackItem GetItem(int[] _stackIDs)
    {
        int length = _StackedItems.Count;
        for (int i = length - 1; i >= 0; i--) //Start from the end to get the top item first.
        {
            if(_stackIDs.Length == 0) //If nothing, destroy anything.
                return _StackedItems[i];

            if (_stackIDs.Contains(_StackedItems[i].StackID)) //Check if the item ID exists in the array
                return _StackedItems[i];
        }
        return null;
    }

    public void ItemDropStart(InputController _currentInput, InputStat _stat)
    {
        if (_DropRoutine != null)
            StopCoroutine(_DropRoutine);

        _DropRoutine = StartCoroutine(HandleItemDrop(_currentInput, _stat));
    }

    public void ItemDropStart(TrashController _currentInput, TrashStat _stat)
    {
        if (_DropRoutine != null)
            StopCoroutine(_DropRoutine);

        _DropRoutine = StartCoroutine(HandleItemDrop(_currentInput, _stat));
    }

    private IEnumerator HandleItemDrop(InputController _currentInput, InputStat _stat)
    {
        _IsInteracting = true;
        int maxCapacity = _stat.GetMaxCapacity();
        int requiredStackID = _stat.GetRequiredStackID();
        float dropItemDelay = _stat.GetDropSpeed();
        while (_IsInteracting && _stat.GetCurrentItemAmount() < maxCapacity)
        {
            StackItem currentItem = GetItem(requiredStackID);
            if (currentItem == null || _currentInput == null)
                break;

            GameObject item = _stat.AddInput(requiredStackID);
            item.transform.position = currentItem.transform.position;
            item.transform.rotation = currentItem.transform.rotation;

            yield return StartCoroutine(MoveItemToArea(item, _stat, dropItemDelay));

            yield return new WaitForSeconds(0.1f);
        }
        _IsInteracting = false;
    }

    private IEnumerator HandleItemDrop(TrashController _currentTrash, TrashStat _stat)
    {
        _IsInteracting = true;

        int[] requiredStackID = _stat.GetRequiredStackIDs();
        float dropItemDelay = _stat.GetDropSpeed();

        while (_IsInteracting)
        {
            StackItem currentItem = GetItem(requiredStackID);
            if (currentItem == null || _currentTrash == null)
                break;

            yield return StartCoroutine(MoveItemToArea(currentItem.gameObject, _stat, dropItemDelay));

            yield return new WaitForSeconds(0.1f);
        }
        _IsInteracting = false;
    }

    private IEnumerator MoveItemToArea(GameObject _item, InputStat _stat, float _speed)
    {
        RemoveStack(_item.GetComponent<StackItem>().StackID);

        Vector3 startPos = _item.transform.position;
        Quaternion startRot = _item.transform.rotation;

        float elapsedTime = 0;
        float duration = 0.25f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime * _speed;

            _item.transform.position = Vector3.Lerp(startPos, _stat.GetLastStackPosition(), elapsedTime / duration);
            _item.transform.rotation = Quaternion.Lerp(startRot, _stat.GetLastStackRotation(), elapsedTime / duration);

            yield return null;
        }

        _item.transform.position = _stat.GetLastStackPosition();
        _item.transform.rotation = _stat.GetLastStackRotation();
    }

    private IEnumerator MoveItemToArea(GameObject _item, TrashStat _stat, float _speed)
    {
        RemoveStack(_item.GetComponent<StackItem>().StackID);

        Transform originalParent = _item.transform.parent;
        _item.transform.SetParent(null);
        _item.gameObject.SetActive(true);

        Vector3 startPos = _item.transform.position;
        Quaternion startRot = _item.transform.rotation;

        float elapsedTime = 0;
        float duration = 0.25f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime * _speed;

            _item.transform.position = Vector3.Lerp(startPos, _stat.GetTrashPoint(), elapsedTime / duration);
            _item.transform.rotation = Quaternion.Lerp(startRot, _stat.GetTrashRotation(), elapsedTime / duration);

            yield return null;
        }

        _item.transform.SetParent(originalParent);
        _item.gameObject.SetActive(false);

        _item.transform.position = _stat.GetTrashPoint();
        _item.transform.rotation = _stat.GetTrashRotation();
    }

    private Vector3 GetNextStackPosition(InputStat _stat)
    {
        int currentCapacity = _stat.GetCurrentItemAmount();
        int maxCapacity = _stat.GetMaxCapacity();
        int row = currentCapacity / maxCapacity; 
        int column = currentCapacity % maxCapacity; 

        float xOffset = column * _stat.GetColumnOffset(); 
        float yOffset = row * _stat.GetRowOffset();   

        return new Vector3(xOffset, yOffset, 0f); 
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
            _StackedItems.Add(item.GetComponent<StackItem>());
        
        for (int i = 0; i < _StackedItems.Count; i++)
        {
            _StackedItems[i].transform.localPosition = new Vector3(0, i * _StackSpacing, 0);
        }

        _StackedItems = new();
    }

    public void AddEditorStack(int _stackID)
    {
        AddStack(_stackID);
    }

    public void RemoveEditorStack(int _stackID)
    {
        RemoveStack(_stackID);
    }

#endif
}
