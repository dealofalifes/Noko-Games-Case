using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashStat : MonoBehaviour
{
    [Header("Destroy Point")]
    [SerializeField] private Transform _TrashPoint;

    [Header("Stack IDs to Destroy")]
    [SerializeField] private List<int> _RequiredStackIDs = new();

    [Header("Destroy Speed")]
    [SerializeField] private float _DropSpeed = 0.33f;

    public int[] GetRequiredStackIDs()
    {
        return _RequiredStackIDs.ToArray();
    }
    public float GetDropSpeed()
    {
        return _DropSpeed;
    }

    public Vector3 GetTrashPoint()
    {
        return _TrashPoint.position;
    }

    public Quaternion GetTrashRotation()
    {
        return _TrashPoint.rotation;
    }
}
