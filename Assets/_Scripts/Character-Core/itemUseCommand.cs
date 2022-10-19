using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemUseCommand : itemCommand
{
    string itemName;

    public itemUseCommand(string itemName)
    {
        this.itemName = itemName;
    }

    public void useItem()
    {
        itemEffector.useEffect(itemName);
    }

    public void switchItem()
    {

    }

    public string getItemName()
    {
        return itemName;
    }
}
