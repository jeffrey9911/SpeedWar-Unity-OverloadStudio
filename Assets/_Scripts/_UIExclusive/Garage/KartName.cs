using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartName : MonoBehaviour
{
    public GameObject textUp;
    public GameObject textMid;
    public GameObject textBot;

    public float mOffsetUp;
    public float mOffsetMid;
    public float mOffsetBot;

    public float mSpeedUp = -0.3f;
    public float mSpeedMid = 0.18f;
    public float mSpeedBot = -0.07f;

    private void FixedUpdate()
    {
        mOffsetBot = calcOffset(mOffsetBot, mSpeedBot);
        mOffsetMid = calcOffset(mOffsetMid, mSpeedMid);
        mOffsetUp = calcOffset(mOffsetUp, mSpeedUp);

        textBot.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(-mOffsetBot, 0);
        textMid.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(-mOffsetMid, 0);
        textUp.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(-mOffsetUp, 0);
    }

    private float calcOffset(float offset, float speed)
    {
        float returning = offset - speed * Time.deltaTime;

        if (returning >= 1) returning -= 1;

        if (returning <= -1) returning += 1;

        return returning;
    }

    public void SwapMaterial(Material material)
    {
        textUp.GetComponent<Renderer>().material = material;
        textMid.GetComponent<Renderer>().material = material;
        textBot.GetComponent<Renderer>().material = material;
    }

}
