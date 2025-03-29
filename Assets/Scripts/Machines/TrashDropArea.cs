using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashDropArea : MonoBehaviour, IInteractable, IDestructible
{
    [SerializeField] private TrashController _TrashController;
    public ICollectable GetCollectable()
    {
        return null;
    }

    public IDestructible GetDestructible()
    {
        return this;
    }

    public IDroppable GetDroppable()
    {
        return null;
    }

    public bool IsCollectable()
    {
        return false;
    }

    public bool IsDroppable()
    {
        return false;
    }

    public bool IsDestructible()
    {
        return true;
    }

    public TrashController GetTrashController()
    {
        return _TrashController;
    }
}
