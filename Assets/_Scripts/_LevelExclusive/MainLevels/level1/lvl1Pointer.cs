using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class lvl1Pointer : MonoBehaviour
{
    public RectTransform _parentRect;

    void Update()
    {
        RectTransform _thisRect = this.GetComponent<RectTransform>();
        Vector2 _pointerController = this.GetComponent<RectTransform>().anchoredPosition;
        
        
        Vector2 ptDirection = getMousePosition() - _pointerController;

        ptDirection = ptDirection.normalized;

        float angle = 360 - Mathf.Atan2(ptDirection.x, ptDirection.y) * Mathf.Rad2Deg;

        //Quaternion ptRot = Quaternion.AngleAxis(angle, new Vector3(0.0f, 0.0f, 1.0f));

        //this.transform.rotation = ptRot;

        _thisRect.eulerAngles = new Vector3(0.0f, 0.0f, angle);
        

        //this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Vector2.Angle(pointerVecter, mouseVector));
    }

    Vector2 getMousePosition()
    {
        Vector2 outPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, Input.mousePosition, Camera.main, out outPos);
        return outPos;
    }
}
