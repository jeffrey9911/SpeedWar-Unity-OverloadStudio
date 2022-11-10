using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class lvl1btnSelect : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TMP_Text _buttonText;

    private void Start()
    {
        
    }
    public void OnSelect(BaseEventData eventData)
    {
        //Debug.Log(eventData.selectedObject.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch(this.name)
        {
            case "BTN_Single":
                _buttonText.text = "Play Solo";
                    break;

            case "BTN_Multiplayer":
                _buttonText.text = "Play Online";
                break;

            case "BTN_Settings":
                _buttonText.text = "Settings";
                break;

            case "BTN_Quit":
                _buttonText.text = "Quit Game";
                break;

                default:
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        switch (this.name)
        {
            case "BTN_Single":
                _buttonText.text = "Let's Race!";
                break;

            case "BTN_Multiplayer":
                _buttonText.text = "Let's Race!";
                break;

            case "BTN_Settings":
                _buttonText.text = "Let's Race!";
                break;

            case "BTN_Quit":
                _buttonText.text = "Let's Race!";
                break;

            default:
                break;
        }
    }
}
