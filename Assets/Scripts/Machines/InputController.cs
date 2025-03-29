using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
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
}
