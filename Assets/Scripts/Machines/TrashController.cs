using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashController : MonoBehaviour
{
    [SerializeField] private TrashStat _Stat;

    public TrashStat GetStat()
    {
        return _Stat;
    }
}
