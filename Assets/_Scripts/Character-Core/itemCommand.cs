using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface itemCommand
{
    public void useItem();

    public void switchItem();

    public string getItemName();
}
