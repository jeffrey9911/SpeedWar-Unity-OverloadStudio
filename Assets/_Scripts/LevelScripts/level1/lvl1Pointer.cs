using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lvl1Pointer : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 ptPos = Camera.main.WorldToScreenPoint(this.transform.position);
        ptPos.x /= 100.0f;
        ptPos.y /= 100.0f;
        ptPos.z = 0.0f;

        Vector2 ptDirection = Input.mousePosition - ptPos;

        

        ptDirection = ptDirection.normalized;

        float angle = Mathf.Atan2(ptDirection.y, ptDirection.x) * Mathf.Rad2Deg - 47.0f;

        //ebug.Log(Camera.main.WorldToScreenPoint(this.transform.position));


        Quaternion ptRot = Quaternion.AngleAxis(angle, new Vector3(0.0f, 0.0f, 1.0f));

        this.transform.rotation = ptRot;

    }
}
