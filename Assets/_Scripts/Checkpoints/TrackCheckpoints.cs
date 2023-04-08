using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    public static TrackCheckpoints instance;

    private List<Vector3> cpPosList = new List<Vector3>();
    private Queue<CheckpointSingle> cpQueue = new Queue<CheckpointSingle>();

    public int laps = 1;
    private int currentLap = 0;
    private int numOfCP = 0;

    private float totalDist = 0;
    private float currentDist = 0;

    private float _gTimer = 0.0f;

    [SerializeField] private TMP_Text _lapText;

    [SerializeField] private Canvas _GUI;
    [SerializeField] private Canvas _Winlose;
    [SerializeField] private TMP_Text _winloseText;

    private void Awake()
    {
        if(!instance)
            instance = this;

        
        Transform checkpointsTransform = GameObject.Find("Checkpoints").transform;
        

        for(int i = 0; i < laps; i++)
        {
            foreach (Transform checkpointSingleTransform in checkpointsTransform)
            {
                //Debug.Log(checkpointSingleTransform);
                CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();

                cpQueue.Enqueue(checkpointSingle);
                cpPosList.Add(checkpointSingleTransform.position);

                checkpointSingleTransform.GetComponent<MeshRenderer>().enabled = false;
                numOfCP++;
            }
        }

        numOfCP /= laps;
    }

    private void Start()
    {
        cpQueue.Peek().GetComponent<MeshRenderer>().enabled = true;
        totalDist = calcDistance();
    }

    private void Update()
    {
        UpdateCompletion();
        UpdateLaps();
        UpdateGTimer();
    }

    public void PlayerThroughCheckpoint(CheckpointSingle checkpointSingle)
    {

        if(checkpointSingle == cpQueue.Peek())
        {
            Debug.Log("Correct");
            cpQueue.Dequeue().GetComponent<MeshRenderer>().enabled = false;
            cpPosList.RemoveAt(0);
            if(cpQueue.Count == 0)
            {
                GameOver();
                Debug.Log("YOU WIN!");
                return;
            }
            cpQueue.Peek().GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            Debug.Log("Wrong Way");
        }

        if(cpQueue.Count == 1)
        {
            cpQueue.Peek().GetComponent<Renderer>().material.color = new Vector4(1, 0, 0, 0.5f);
        }
    }

    private float calcDistance()
    {
        float returnDist = 0.0f;
        for(int i = 0; i < cpPosList.Count - 1; i++)
        {
            returnDist += Vector3.Distance(cpPosList[i], cpPosList[i + 1]);
        }
        return returnDist;
    }

    private void UpdateCompletion()
    {
        if (cpQueue.Count > 0)
        {
            currentDist = Vector3.Distance(NetworkManager.localPlayer.playerTransform.position, cpPosList[0]) + calcDistance();
            float perc = (totalDist - currentDist) / totalDist * 100;
            if(perc < 0)
            {
                perc = 0;
            }
        }
    }

    private void UpdateLaps()
    {
        if(cpQueue.Count > 0)
        {
            currentLap = Mathf.CeilToInt(laps - (cpQueue.Count / numOfCP));
            _lapText.text = currentLap + "/" + laps;
        }
    }

    private void UpdateGTimer()
    {
        if (cpQueue.Count > 0)
        {
            _gTimer += Time.deltaTime;
            GameplayUIManager.instance.UpdateGameTime(_gTimer);
        }
    }

    private void GameOver()
    {
        
        _GUI.gameObject.SetActive(false);
        _Winlose.gameObject.SetActive(true);
        int finalScore = scoreManager.instance.getScore() * 10 / (int)_gTimer;
        if(finalScore >= 400)
        {
            _winloseText.text = "Your points is \n Score * 10 / Timer = " + finalScore + " POINTS!\n" + finalScore + " >= 400! You Win!";
        }
        else
        {
            _winloseText.text = "Your points is \n Score * 10 / Timer = " + finalScore + " POINTS!\n" + finalScore + " < 400! You Lose!";
        }
        
        Time.timeScale = 0.0f;
    }
    
}
