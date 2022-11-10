using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemEffectInvoker : MonoBehaviour
{
    KartAction inputActions;

    public static List<itemCommand> _itemCommands;

    private float effectCD;

    private itemCommand _itemInUse;

    private bool isUsingItem;

    private void Start()
    {
        _itemCommands = new List<itemCommand>();

        inputActions = KartInputController.inst_controller.inputActions;

        inputActions.Player.UseItem.performed += context => UseItem();
        
        

        effectCD = -1.0f;

        isUsingItem = false;
    }

    public static void AddItem(itemCommand _command)
    {
        _itemCommands.Add(_command);
    }

    private void UseItem()
    {
        if(_itemCommands.Count > 0)
        {
            if (_itemCommands[0].getItemName() == "Booster")
            {
                effectCD = 3.0f;
            }
            else
            {
                effectCD = -1.0f;
            }

            isUsingItem = true;    
            

            _itemInUse = _itemCommands[0];
            _itemCommands.RemoveAt(0);
        }
    }

    private void FixedUpdate()
    {
        if(isUsingItem)
        {
            if (effectCD > 0)
            {
                _itemInUse.useItem();
                effectCD -= Time.deltaTime;
                isUsingItem = effectCD > 0;
                if(_itemInUse.getItemName() == "Booster")
                    KartController.instance.isBoosted = isUsingItem;

                Debug.Log("PERFORM EFFECT: " + _itemInUse.getItemName() + " for: [" + effectCD + "] seconds!");
            }
            else if (effectCD == -1.0f)
            {
                _itemInUse.useItem();
                isUsingItem = false;
                Debug.Log("PERFORM EFFECT: " + _itemInUse.getItemName() + " instantly");
            }
        }
        
    }
}
