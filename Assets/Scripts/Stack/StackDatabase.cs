using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackDatabase : MonoBehaviour
{
    public static StackDatabase Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
