using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDropArea : MonoBehaviour, IInteractable, IDroppable
{
    [SerializeField] private int _InputIndex;
    [SerializeField] private InputController _InputController;

    public InputController GetInputController()
    {
        return _InputController;
    }

    public bool IsCollectable()
    {
        return false;
    }

    public bool IsDroppable()
    {
        return true;
    }

    public bool IsDestructible()
    {
        return false;
    }

    public ICollectable GetCollectable()
    {
        return null;
    }

    public IDroppable GetDroppable()
    {
        return this;
    }

    public IDestructible GetDestructible()
    {
        return null;
    }

    public int GetInputIndex()
    {
        return _InputIndex;
    }

}
