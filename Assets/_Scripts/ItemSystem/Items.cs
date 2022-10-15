using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Items : MonoBehaviour
{
    public float _tCounter;

    public abstract string Name { get; }

    public abstract GameObject createItem(GameObject _itemPrefab, Vector3 instPos, Quaternion instQuat);

    public abstract void itemEffect();

    private void effectTime(float effectTime)
    {
        _tCounter = effectTime;
    }

    private void FixedUpdate()
    {
        if (_tCounter > 0)
        {
            itemEffect();
            _tCounter -= Time.deltaTime;
        }   
    }

}

public class Booster : Items
{
    public override string Name => "Booster";

    public override GameObject createItem(GameObject _itemPrefab, Vector3 instPos, Quaternion instQuat)
    {
        GameObject item = Instantiate(_itemPrefab, instPos, instQuat);

        return item;
    }

    public override void itemEffect()
    {
        PunManager.instance._spawnedPlayer.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 100f);
    }
}
