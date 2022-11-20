using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Event_Select : MonoBehaviour
{
    [SerializeField]
    private GameObject _ScrollContent;
    [SerializeField]
    private GameObject _scrContentPnlPrefab;
    [SerializeField]
    private GameObject _kartBtn;
    private void Start()
    {
        if(SceneConnect.instance != null)
        {
            KartAssetManager KAM = SceneConnect.instance.gameObject.GetComponent<KartAssetManager>();
            for(int i = 0; i < KAM.kartList.Count; i++)
            {
                GameObject PNL = Instantiate(_scrContentPnlPrefab);
                PNL.transform.SetParent(_ScrollContent.transform);
                for(int j = 0; j < 4; j++)
                {
                    GameObject BTN = Instantiate(_kartBtn);
                    BTN.transform.SetParent(PNL.transform);
                    KartButton KBTN = BTN.GetComponent<KartButton>();
                    KBTN.SetKartName(KAM.kartList[i].KartName);
                    KBTN.SetKartImage(KAM.kartList[i].KartImage);
                    if(i < KAM.kartList.Count - 1)
                    {
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
    public void car1OnClick()
    {
        GameplayManager.instance.setData(GameplayManager.instance.selectedCar, 1.001f);

        Debug.Log("KART 1 SELECTED!");
    }

    public void car2OnClick()
    {
        GameplayManager.instance.setData(GameplayManager.instance.selectedCar, 1.002f);

        Debug.Log("KART 2 SELECTED!");
    }

    public void car3OnClick()
    {
        GameplayManager.instance.setData(GameplayManager.instance.selectedCar, 1.01f);

        Debug.Log("KART 3 SELECTED!");
    }

    public void startOnClick()
    {
        if (GameplayManager.instance.getData(GameplayManager.instance.selectedMode) == 1f)
        {
            SceneManager.LoadScene("Level1");
        }
        else if (GameplayManager.instance.getData(GameplayManager.instance.selectedMode) == 2f)
        {
            PhotonNetwork.LoadLevel("Level1");
        }
    }

    public void backOnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
