using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MachineController
{
    [SerializeField] private List<InputStat> _Stats = new();
    public InputStat GetStatByIndex(int _index)
    {
        return _Stats[_index];
    }

    public InputStat[] GetAllInputStats()
    {
        return _Stats.ToArray();
    }

    public override int[] GetRequiredID(int _indexIfMoreThanOne)
    {
        int[] id = new int[] { _Stats[_indexIfMoreThanOne].GetRequiredStackID() };
        return id;
    }

    public override bool HasSpace(int _indexIfMoreThanOne = 0)
    {
        return _Stats[_indexIfMoreThanOne].HasSpace();
    }

    public override bool HasProduct(int _indexIfMoreThanOne = 0)
    {
        return true;
    }
}
