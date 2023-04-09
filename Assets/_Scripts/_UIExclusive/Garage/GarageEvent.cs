using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

public class GarageEvent : MonoBehaviour
{
    private int index = 0;

    private KartAsset _kart;
    private KartAssetManager KAM;

    public Button _lv2Btn;
    public Button _lv3Btn;
    public Button _joinBtn;

    private void Start()
    {
        KAM = SceneDataManager.instance.kartAssetManager;
        _kart = KAM.getKartList[index];
        SwitchKart();

        if(NetworkManager.isJoiningRoom)
        {
            _joinBtn.gameObject.SetActive(true);
            _lv2Btn.gameObject.SetActive(false);
            _lv3Btn.gameObject.SetActive(false);
        }
    }

    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void joinRoomOnClick()
    {
        NetworkManager.JoinOnRequest();
    }

    public void startOnClickLV2()
    {
        if(NetworkManager.isOnNetwork)
        {
            short lvid = 2;
            NetworkManager.CreateNewGame(ref lvid);
        }
        else
        {
            SceneManager.LoadScene(2);
        }
        
    }

    public void startOnClick()
    {
        if (NetworkManager.isOnNetwork)
        {
            short lvid = 3;
            NetworkManager.CreateNewGame(ref lvid);
        }
        else
        {
            SceneManager.LoadScene(3);
        }
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
    }
}
