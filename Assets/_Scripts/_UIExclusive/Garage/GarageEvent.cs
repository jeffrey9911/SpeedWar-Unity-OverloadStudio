using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GarageEvent : MonoBehaviour
{
    private int index = 0;

    private KartAsset _kart;
    private KartAssetManager KAM;

    private void Start()
    {
        KAM = SceneDataManager.instance.kartAssetManager;
        _kart = KAM.getKartList[index];
        SwitchKart();
    }

    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void startOnClickLV2()
    {
        SceneManager.LoadScene(2);
    }

    public void startOnClick()
    {
        SceneManager.LoadScene(3);
    }


    private void SwitchKart()
    {
        Debug.Log(SceneData.SelectedKart);
        SceneDataManager.instance.setData(SceneData.SelectedKart, _kart.KartID);

        KartName _kn = GameObject.Find("KartName").GetComponent<KartName>();
        _kn.SwapMaterial(_kart.KartNameMaterial);

        KartStat _ks = GameObject.Find("KartSpawn").GetComponent<KartStat>();
        _ks.setupKart(_kart);
    }



    public void NextOnClick()
    {
        index = index == KAM.getKartList.Count - 1 ? 0 : index + 1;
        _kart = KAM.getKartList[index];
        SwitchKart();
    }

    public void PreviousOnClick()
    {
        index = index == 0 ? KAM.getKartList.Count - 1 : index - 1;
        _kart = KAM.getKartList[index];
        SwitchKart();
    }

    public void backOnClick()
    {
        SceneManager.LoadScene("MainMenu");
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
