using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool IsCollectable();
    public bool IsDroppable();
    public bool IsDestructible();

    public ICollectable GetCollectable();
    public IDroppable GetDroppable();
    public IDestructible GetDestructible();
}
