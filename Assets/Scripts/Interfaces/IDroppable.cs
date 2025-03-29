using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDroppable
{
    public InputController GetInputController();

    public int GetInputIndex();
}
