using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Items : MonoBehaviour
{
    public abstract string Name { get; }

    public abstract GameObject createItem(GameObject _itemPrefab, Vector3 instPos, Quaternion instQuat);
}

public class Booster : Items
{
    public override string Name => "Booster";

    public override GameObject createItem(GameObject _itemPrefab, Vector3 instPos, Quaternion instQuat)
    {
        GameObject item = Instantiate(_itemPrefab, instPos, instQuat);

        return item;
    }
}

public class InstItem : Items
{
    public override string Name => "InstItem";

    public override GameObject createItem(GameObject _itemPrefab, Vector3 instPos, Quaternion instQuat)
    {
        GameObject item = Instantiate(_itemPrefab, instPos, instQuat);

        return item;
    }
}