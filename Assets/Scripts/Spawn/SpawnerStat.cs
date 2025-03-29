using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerStat : MonoBehaviour
{
    [SerializeField] private GameObject _OutputPrefab;

    [SerializeField] private int _ProduceStackID = 1;
    [SerializeField] private int _TotalOutputCapacity = 30;
    [SerializeField] private int _RowOutputCapacity = 6;
    [SerializeField] private float _ProduceTime = 1.5f;

    [Header("Input Stat")]
    [Tooltip("If you want this machine checks Inputs before produce, Place InputStats Here. If not, just let it be empty!")]
    [SerializeField] private List<RequiredInput> _RequiredInputs = new();

    public GameObject GetProductPrefab()
    {
        return _OutputPrefab;
    }

    public int GetProduceStackID()
    {
        return _ProduceStackID;
    }

    public int GetTotalOutputCapacity()
    {
        return _TotalOutputCapacity;
    }

    public int GetOutputRowCapacity()
    {
        return _RowOutputCapacity;
    }

    public float GetProduceTime()
    {
        return _ProduceTime;
    }

    public bool HasEnoughInput()
    {
        foreach (var item in _RequiredInputs)
        {
            if (item.Stat.GetCurrentItemAmount() < item.RequiredAmount)
            {
                Debug.Log("This machine needs input ID: "+ item.Stat.GetReuiredStackID() + "/Amount: " + item.RequiredAmount + " to produce. Click on me to see!" + "", transform);
                return false;
            }
        }

        return true;
    }

    public void UseInputs()
    {
        foreach (var item in _RequiredInputs)
            for (int i = 0; i < item.RequiredAmount; i++)
                item.Stat.RemoveInput(item.Stat.GetReuiredStackID()); 
    }
}

[System.Serializable]
public struct RequiredInput
{
    public InputStat Stat;
    public int RequiredAmount; //How many of this stacks are required to produce 1 item
}
