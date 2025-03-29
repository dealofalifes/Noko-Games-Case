using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerCollectArea : MonoBehaviour, IInteractable, ICollectable
{
    [SerializeField] private StorageController _Storage;

    public ICollectable GetCollectable()
    {
        return this;
    }

    public IDroppable GetDroppable()
    {
        return null;
    }

    public StorageController GetStorage()
    {
        return _Storage;
    }

    public bool IsCollectable()
    {
        return true;
    }

    public bool IsDroppable()
    {
        return false;
    }
}
