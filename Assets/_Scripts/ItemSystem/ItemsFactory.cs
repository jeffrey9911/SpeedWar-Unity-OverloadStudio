using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Reflection;
using System;
using Unity.VisualScripting;

public class ItemsFactory : MonoBehaviour
{
    public GameObject booster;

    public GameObject instantEffectItem;

    List<Items> _items;

    private void Start()
    {
        var itemTypes = Assembly.GetAssembly(typeof(Items)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Items)));

        _items = new List<Items>();

        foreach(var type in itemTypes)
        {
            var tempType = Activator.CreateInstance(type) as Items;

            _items.Add(tempType);
        }
    }

    public Items GetItems(string itemName)
    {
        foreach(Items item in _items)
        { 
            if(item.Name == itemName)
            {
                var target = Activator.CreateInstance(item.GetType()) as Items;

                return target;
            }
        }

        return null;
    }
}


