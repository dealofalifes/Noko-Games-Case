using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpawnerStat))]
[RequireComponent(typeof(SpawnController))]
public class StorageController : MonoBehaviour
{
    [SerializeField] private Transform _StoragePoint;
    [SerializeField] private SpawnerStat _Stat;
    [SerializeField] private SpawnController _SpawnController;

    [SerializeField] private float _ItemSpacing = 0.3f;
    [SerializeField] private float _RowSpacing = 0.3f;

    [Header("DEBUG")]
    [SerializeField] private List<GameObject> _StoredProducts = new List<GameObject>();

    public bool HasSpace()
    {
        return _StoredProducts.Count < _Stat.GetTotalOutputCapacity();
    }

    public void AddProduct(GameObject product)
    {
        if (_StoredProducts.Count >= _Stat.GetTotalOutputCapacity())
        {
            _SpawnController.ToggleSpawning(false);
            return;
        }

        _StoredProducts.Add(product);
        UpdateStackPositions();
    }

    public void RemoveProduct()
    {
        if (_StoredProducts.Count == 0) return;

        GameObject removedProduct = _StoredProducts[0];
        _StoredProducts.RemoveAt(0);
        _SpawnController.ReturnProductToPool(removedProduct);

        if (HasSpace())
        {
            _SpawnController.ToggleSpawning(true);
        }

        UpdateStackPositions();
    }

    private void UpdateStackPositions()
    {
        for (int i = 0; i < _StoredProducts.Count; i++)
        {
            int rowSize = _Stat.GetOutputRowCapacity();
            int row = i / rowSize;
            int column = i % rowSize;

            float xOffset = column * _ItemSpacing;
            float yOffset = row * _RowSpacing;

            _StoredProducts[i].transform.SetParent(_StoragePoint);
            _StoredProducts[i].transform.localEulerAngles = new Vector3(0, -90, 0);
            _StoredProducts[i].transform.localPosition = new Vector3(xOffset, yOffset, 0);
        }
    }

    public GameObject TakeProduct()
    {
        if (_StoredProducts.Count == 0) return null;

        GameObject item = _StoredProducts[0];
        _StoredProducts.RemoveAt(0);
        UpdateStackPositions();

        return item;
    }
}
