using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackItem : MonoBehaviour
{
    [SerializeField] private Transform _Mesh;

    [Header("DEBUG")]
    [SerializeField] private int _StackID;

    public int StackID
    {
        get => _StackID;
    }

    public Transform StackObject
    {
        get => transform;
    }

    public void SetID(int _id)
    {
        _StackID = _id;

        foreach (Transform item in _Mesh)
            item.gameObject.SetActive(false);

        _Mesh.GetChild(_id - 1).gameObject.SetActive(true);
    }
}
