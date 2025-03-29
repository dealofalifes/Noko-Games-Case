using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpawnerStat))]
[RequireComponent(typeof(StorageController))]
public class SpawnController : MachineController
{
    [SerializeField] private Animator _Animator;
    [SerializeField] private Transform _SpawnPoint;
    [SerializeField] private SpawnerStat _Stat;
    [SerializeField] private StorageController _Storage;
    [SerializeField] private AudioSource _AudioSource;

    private readonly int IsProducedHash = Animator.StringToHash("Produced");

    //Pool System
    private Queue<GameObject> _ProductPool = new Queue<GameObject>();

    [Header("DEBUG")]
    [SerializeField] private bool _CanSpawn = true;

    void Start()
    {
        GameObject productPrefab = _Stat.GetProductPrefab();
        int storageCapacity = _Stat.GetTotalOutputCapacity();
        for (int i = 0; i < storageCapacity; i++)
        {
            GameObject product = Instantiate(productPrefab, _SpawnPoint);
            product.SetActive(false);
            _ProductPool.Enqueue(product);
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (_CanSpawn && _Storage.HasSpace())
            {
                SpawnProduct();
            }
            yield return new WaitForSeconds(_Stat.GetProduceTime());
        }
    }

    private void SpawnProduct()
    {
        if (_Stat.HasEnoughInput())
        {
            _Animator.SetTrigger(IsProducedHash);
            _Stat.UseInputs();
        }
        else
        {
            return;
        }

        if (_ProductPool.Count == 0)
        {
            foreach (Transform item in _SpawnPoint) //Refresh carried ones.
                if (!item.gameObject.activeSelf && !_ProductPool.Contains(item.gameObject))
                    _ProductPool.Enqueue(item.gameObject);
                
            if(_ProductPool.Count == 0) //If still 0 means really we need to instantiate new now.
            {
                GameObject addStack = Instantiate(_SpawnPoint.GetChild(0).gameObject, _SpawnPoint);
                _ProductPool.Enqueue(addStack);
            }
        }

        GameObject product = _ProductPool.Dequeue();
        product.GetComponent<StackItem>().SetID(_Stat.GetProduceStackID());
        product.transform.position = _SpawnPoint.position;
        product.SetActive(true);

        _Storage.AddProduct(product);

        AudioManager.Instance.PlayProduceItemSound(_AudioSource);
    }

    public void ReturnProductToPool(GameObject product)
    {
        product.SetActive(false);
        _ProductPool.Enqueue(product);
    }

    public void ToggleSpawning(bool state)
    {
        _CanSpawn = state;
    }

    public override int[] GetRequiredID(int _indexIfMoreThanOne = 0)
    {
        return new int[] { };
    }

    public override bool HasSpace(int _indexIfMoreThanOne = 0)
    {
        return true;
    }

    public override bool HasProduct(int _indexIfMoreThanOne = 0)
    {
        return _Storage.GetProductAmount() > 0;
    }
}
