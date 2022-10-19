using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    private ItemsFactory _itemFactory;



    private void SpawnItem()
    {

    }

    private void Start()
    {
        _itemFactory = GameObject.Find("GAMEMANAGER").GetComponent<ItemsFactory>();

    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.T))
        {
            Vector3 spawnPos = PunManager.instance._spawnedPlayer.transform.position;
            spawnPos.x += Random.Range(-5.0f, 5.0f);
            spawnPos.z += Random.Range(-5.0f, 5.0f);
            _itemFactory.GetItems("Booster").createItem(_itemFactory.booster, spawnPos, _itemFactory.booster.transform.rotation);

            spawnPos.x += Random.Range(-5.0f, 5.0f);
            spawnPos.z += Random.Range(-5.0f, 5.0f);
            _itemFactory.GetItems("InstItem").createItem(_itemFactory.instantEffectItem, spawnPos, _itemFactory.instantEffectItem.transform.rotation);
        }

    }

    

    
}
