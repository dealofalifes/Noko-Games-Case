using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashController : MachineController
{
    [SerializeField] private TrashStat _Stat;

    public override int[] GetRequiredID(int _indexIfMoreThanOne = 0)
    {
        return _Stat.GetRequiredStackIDs();
    }

    public TrashStat GetStat()
    {
        return _Stat;
    }

    public override bool HasSpace(int _indexIfMoreThanOne = 0)
    {
        return true;
    }

    public override bool HasProduct(int _indexIfMoreThanOne = 0)
    {
        return false;
    }
}
