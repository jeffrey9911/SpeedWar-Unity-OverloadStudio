using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartInputController : MonoBehaviour
{
    public static KartInputController inst_controller;

    public KartAction inputActions;

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Awake()
    {
        if (inst_controller == null)
            inst_controller = this;

        inputActions = new KartAction();
    }
}
