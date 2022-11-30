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

            int index = 0;
            while (true)
            {
                if (index > KAM.kartList.Count - 1)
                { break; }
                GameObject PNL = Instantiate(_scrContentPnlPrefab);
                PNL.transform.SetParent(_ScrollContent.transform);
                
                for(int i = 0; i < 4; i++)
                {

                    if (index > KAM.kartList.Count - 1)
                    {
                        break;
                    }
                    

                    GameObject BTN = Instantiate(_kartBtn);
                    BTN.transform.SetParent(PNL.transform);
                    KartButton KBTN = BTN.GetComponent<KartButton>();
                    KBTN.KartButtonSetup(KAM.kartList[index]);

                    index++;
                }
                
                
            }
        }
    }
    

    

    public void backOnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    
}
