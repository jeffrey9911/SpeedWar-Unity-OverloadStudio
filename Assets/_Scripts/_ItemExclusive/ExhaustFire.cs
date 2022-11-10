using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhaustFire : MonoBehaviour
{
    [SerializeField] private float activeTime;
    private float timeCounter = 0;

    private void FixedUpdate()
    {
        if(timeCounter > 0)
            timeCounter -= Time.deltaTime;

        this.gameObject.SetActive(timeCounter > 0);
    }

    public void SetTimedActive()
    {
        timeCounter = activeTime;
    }
}
