using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

using System.Threading.Tasks;
using Unity.VisualScripting;
using System.Threading;

using UnityEngine.SceneManagement;
using TMPro;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    // Networking
    public static bool isOnNetwork = false;

    private static IPEndPoint serverTCPEP;
    public static IPEndPoint serverUDPEP;

    public static Socket clientTCPSocket;
    public static Socket clientUDPSocket;


    // Player ID
    public static NetPlayer localPlayer;

    public static Dictionary<short, NetPlayer> playerDList = new Dictionary<short, NetPlayer>();

    // Threads control
    public static List<Thread> threads = new List<Thread>();

    // Flags
    static bool isTCPReceiving = false;
    static bool isUDPReceiving = false;
    static bool isFirstInitialized = false; // Player login player list initialize
    public static bool isUpdatingLocalTransform = false;
    public static bool isJoiningRoom = false;
    public static bool isLocalGameFinished = false;
    

    public static short joiningPlayerID = 0;

    // Transform Updating
    static float updateTimer = 0;
    public static float updateTimeInterval = 0.2f;

    // Latency Updating
    static bool isNeedCalcLatency = false;
    static bool isLatencyCalcStarted = false;
    static float latencyCounter = 0;
    static float latencyTimer = 0;
    static float latencyCalcInterval = 1f;

    public static float latency = 0;

    private void Awake()
    {
        
        if (!instance) instance = this;
    }

    private void Update()
    {
        if(isUpdatingLocalTransform && !isLocalGameFinished)
        {
            UpdateLocalTransform();
        }


        if(isNeedCalcLatency && isUpdatingLocalTransform)
        {
            CalculateLatency();
        }
        else
        {
            latencyTimer += Time.deltaTime;
            if(latencyTimer >= latencyCalcInterval)
            {
                isNeedCalcLatency = true;
                latencyTimer = 0;
            }
            
        }
    }

    public void ConnectOnClick(string setName, string ipAdress)
    {
        isOnNetwork = true;
        IPAddress ip = IPAddress.Parse(ipAdress);

        serverTCPEP = new IPEndPoint(ip, 12581);
        serverUDPEP = new IPEndPoint(ip, 12582);

        clientTCPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientUDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // => TCP Receive
        Thread clientLogin = new Thread(() => ClientLogin(setName));
        threads.Add(clientLogin);
        threads[threads.Count - 1].Start();
    }

    static void ClientLogin(string setName)
    {
        try
        {
            clientTCPSocket.Connect(serverTCPEP);
            Debug.Log("TCP Connect");

            clientUDPSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
            Debug.Log("UDP Bind");

            byte[] loginMsg = AddHeader(Encoding.ASCII.GetBytes(setName), 0);
            Debug.Log("Pname length: " + loginMsg.Length);

            clientTCPSocket.Send(loginMsg);
            Debug.Log("Login");
            isTCPReceiving = true;

            ClientTCPReceive();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            throw;
        }
    }

    static void ClientTCPReceive()
    {
        try
        {
            while (isTCPReceiving)
            {
                byte[] recvBuffer = new byte[2048];
                int recv = clientTCPSocket.Receive(recvBuffer);

                switch (GetHeader(recvBuffer, 0))
                {
                    case 0:
                        //Debug.Log("TCP0");
                        if (!isFirstInitialized)
                        {
                            string allPlayer = Encoding.ASCII.GetString(GetContent(recvBuffer, 2, recv - 2));
                            UnityMainThreadDispatcher.Instance().Enqueue(() => InitialPlayerList(ref allPlayer));
                        }
                        else
                        {
                            Debug.Log("Initialize Ignored");
                        }


                        break;

                    case 1:
                        //Debug.Log("TCP1"); // new player
                        short idToAdd = GetHeader(recvBuffer, 2);
                        string nameToAdd = Encoding.ASCII.GetString(GetContent(recvBuffer, 4, recv - 4));
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AddPlayer(ref idToAdd, ref nameToAdd));
                        break;

                    case -1:
                        //Debug.Log("TCP-1"); // remove player
                        short idToRemove = GetHeader(recvBuffer, 2);
                        UnityMainThreadDispatcher.Instance().Enqueue(() => RemovePlayer(ref idToRemove));
                        break;

                    case 2:
                        //Debug.Log("TCP2"); // New message
                        string msgReceived = Encoding.ASCII.GetString(GetContent(recvBuffer, 2, recv - 2));
                        UnityMainThreadDispatcher.Instance().Enqueue(() => ReceivedMessage(ref msgReceived));
                        break;

                    case 3:
                        //Debug.Log("TCP3"); // Update status
                        short[] shorts = new short[4];
                        Buffer.BlockCopy(recvBuffer, 2, shorts, 0, 4);
                        UnityMainThreadDispatcher.Instance().Enqueue(() => UpdateNetPlayerLevelID(ref shorts));
                        break;

                    case 4:
                        //Debug.Log("TCP4"); // Room creation request accepted / New Room Created
                        if(recv <= 4)
                        {
                            localPlayer.playerRoomID = GetHeader(recvBuffer, 2);
                            Debug.Log("Creating room with id: " + localPlayer.playerRoomID + " Level: " + localPlayer.playerLevelID);
                            UnityMainThreadDispatcher.Instance().Enqueue(() => SceneManager.LoadScene(localPlayer.playerLevelID));
                        }
                        else
                        {
                            short newRpid = GetHeader(recvBuffer, 2);
                            short newLvid = GetHeader(recvBuffer, 4);
                            Debug.Log("msg length: " + recv);
                            string newKid = Encoding.ASCII.GetString(GetContent(recvBuffer, 6, recv - 6));
                            UnityMainThreadDispatcher.Instance().Enqueue(() => UpdateNetPlayerRoom(ref newRpid, ref newLvid, ref newKid));
                        }


                        
                        break;

                    case 6:
                        latency = latencyCounter;
                        isLatencyCalcStarted = false;
                        isNeedCalcLatency = false;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => LatencyToUI());
                        break;

                    

                    case 9:
                        Debug.Log("TCP9");
                        short idToScore = GetHeader(recvBuffer, 2);
                        short score = GetHeader(recvBuffer, 4);
                        UnityMainThreadDispatcher.Instance().Enqueue(() => UpdateNetPlayerScore(ref idToScore, ref score));

                        break;

                    default:
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            throw;
        }
        finally
        {
            clientTCPSocket.Close();
        }

        

    }

    static void ClientUDPReceive()
    {
        try
        {
            while (isUDPReceiving)
            {
                byte[] recvBuffer = new byte[1024];
                int recv = clientUDPSocket.Receive(recvBuffer);

                switch (GetHeader(recvBuffer, 0))
                {
                    case 0:
                        break;

                    case 1:
                        //Debug.Log("UDP1");
                        short netPID = GetHeader(recvBuffer, 2);
                        short netPRmid = GetHeader(recvBuffer, 4);
                        float[] fPos = new float[3];
                        Buffer.BlockCopy(recvBuffer, 6, fPos, 0, 12);

                        UnityMainThreadDispatcher.Instance().Enqueue(() => UpdatePlayerPosition(ref netPID, ref netPRmid, ref fPos));
                        
                        break;

                    case 2:
                        //Debug.Log("UDP2");
                        short netRID = GetHeader(recvBuffer, 2);
                        short netRRmid = GetHeader(recvBuffer, 4);

                        float[] fRot = new float[3];

                        Buffer.BlockCopy(recvBuffer, 6, fRot, 0, 12);

                        UnityMainThreadDispatcher.Instance().Enqueue(() => UpdatePlayerRotation(ref netRID, ref netRRmid, ref fRot));

                        break;

                    case 3:
                        //Debug.Log("UDP3");
                        short netIID = GetHeader(recvBuffer, 2);
                        short netIRmid = GetHeader(recvBuffer, 4);

                        float[] fInput = new float[2];

                        Buffer.BlockCopy(recvBuffer, 6, fInput, 0, 8);

                        UnityMainThreadDispatcher.Instance().Enqueue(() => UpdatePlayerInput(ref netIID, ref netIRmid, ref fInput));
                        break;

                    default:
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            throw;
        }
        finally
        {
            clientUDPSocket.Close();
        }
    }

    public static void UpdatePlayerPosition(ref short posID, ref short rmID, ref float[] fPos)
    {
        if (playerDList.ContainsKey(posID))
        {
            if (rmID == localPlayer.playerRoomID)
            {

                if(rmID != playerDList[posID].playerRoomID)
                {
                    playerDList[posID] = playerDList[posID].SetPlayerRoomID(rmID);
                }

                //KartController kc;
                if (playerDList[posID].playerTransform != null/*.TryGetComponent<KartController>(out kc)*/)
                {
                    //playerDList[posID].playerTransform.position = new Vector3(fPos[0], fPos[1], fPos[2]);
                    playerDList[posID].playerTransform.GetComponent<MotionController>().ServerUpdatePos(new Vector3(fPos[0], fPos[1], fPos[2]));
                    
                }
                else
                {
                    Debug.Log(playerDList[posID].playerKartID + " IDID");
                    KartAsset kartKAM = SceneDataManager.instance.kartAssetManager.getKart(playerDList[posID].playerKartID);
                    Debug.Log("Instantiate: " + kartKAM.KartName);
                    playerDList[posID] = playerDList[posID].UpdatePlayerKartObject(Instantiate(kartKAM.AssetPrefab).transform);
                    playerDList[posID].playerTransform.GetComponent<KartController>().KartSetup(kartKAM._acceleration, kartKAM._maxSpeed, kartKAM._drift, kartKAM._control, kartKAM._weight);
                    playerDList[posID].playerTransform.GetComponent<KartController>().spawnMode = 2;

                    //playerDList[posID].playerTransform.position = new Vector3(fPos[0], fPos[1], fPos[2]);
                    playerDList[posID].playerTransform.GetComponent<MotionController>().ServerUpdatePos(new Vector3(fPos[0], fPos[1], fPos[2]));
                }
            }
        }
    }

    public static void UpdatePlayerRotation(ref short rotID, ref short rmID, ref float[] fRot)
    {
        if (playerDList.ContainsKey(rotID))
        {
            if (rmID == localPlayer.playerRoomID)
            {
                if (playerDList[rotID].playerTransform != null)
                {
                    //playerDList[rotID].playerTransform.rotation = Quaternion.Euler(new Vector3(fRot[0], fRot[1], fRot[2]));
                    playerDList[rotID].playerTransform.GetComponent<MotionController>().ServerUpdateRot(Quaternion.Euler(new Vector3(fRot[0], fRot[1], fRot[2])));
                }
            }
        }
    }

    public static void UpdatePlayerInput(ref short inpID, ref short rmID, ref float[] fInput)
    {
        if (playerDList.ContainsKey(inpID))
        {
            if (rmID == localPlayer.playerRoomID)
            {
                if (playerDList[inpID].playerTransform != null)
                {
                    //playerDList[inpID].playerTransform.rotation = Quaternion.Euler(new Vector3(fRot[0], fRot[1], fRot[2]));
                    playerDList[inpID].playerTransform.GetComponent<MotionController>().ServerUpdateInput(new Vector2(fInput[0], fInput[1]));
                }
            }
        }
    }

    public static void InitialPlayerList(ref string allPlayer)
    {
        if (allPlayer.Length > 4 && !isFirstInitialized)
        {
            string[] players = allPlayer.Split("#");

            localPlayer = new NetPlayer(short.Parse(players[0].Substring(0, 4)), players[0].Substring(4, players[0].Length - 4));
            localPlayer.playerLevelID = 0;
            Debug.Log("Local ID :" + localPlayer.playerID + " Local Name: " + localPlayer.playerName);

            for (int i = 1; i < players.Length; i++)
            {
                short netPlayerID = short.Parse(players[i].Substring(0, 4));
                string netPlayerName = players[i].Substring(4, players[i].Length - 4);

                playerDList.Add(netPlayerID, new NetPlayer(netPlayerID, netPlayerName));

                Debug.Log("Net ID: " + netPlayerID + " Net Name: " + netPlayerName + " Created!");
            }

            LobbyManager.RefreshPlayerList();

            short[] shorts = { 0, localPlayer.playerID };
            byte[] loginMsg = new byte[4];
            Buffer.BlockCopy(shorts, 0, loginMsg, 0, 4);

            clientUDPSocket.SendTo(loginMsg, serverUDPEP);
            isFirstInitialized = true;
            isUDPReceiving = true;

            Thread udpReceive = new Thread(ClientUDPReceive);
            threads.Add(udpReceive);
            threads[threads.Count - 1].Start();
        }
    }

    public static void AddPlayer(ref short pID, ref string pName)
    {
        playerDList.Add(pID, new NetPlayer(pID, pName));

        switch (localPlayer.playerLevelID)
        {
            case 0:
                LobbyManager.RefreshPlayerList();
                break;


            default:
                break;
        }
    }

    public static void RemovePlayer(ref short pID)
    {
        if(playerDList.ContainsKey(pID))
        {
            switch (localPlayer.playerLevelID)
            {
                case 0: // lobby
                    playerDList.Remove(pID);
                    LobbyManager.RefreshPlayerList();
                    break;

                case 1:
                    playerDList.Remove(pID);
                    break;

                case 2:
                    Destroy(playerDList[pID].playerTransform.gameObject);
                    playerDList.Remove(pID);
                    break;

                case 3:
                    Destroy(playerDList[pID].playerTransform.gameObject);
                    playerDList.Remove(pID);
                    break;
                default:
                    break;
            }
        }
    }

    

    public static void SendMessage(ref string msgToSend)
    {
        clientTCPSocket.Send(AddHeader(AddHeader(Encoding.ASCII.GetBytes(msgToSend), localPlayer.playerID), 2));
    }
    public static void ReceivedMessage(ref string msgReceived)
    {
        LobbyManager.AddMessage(ref msgReceived);
    }

    public static void UpdateLocalLevelID(ref short id)
    {
        localPlayer.playerLevelID = id;

        short[] updateHeaders = { 3, localPlayer.playerID, localPlayer.playerLevelID };
        byte[] updateMsg = new byte[6];
        Buffer.BlockCopy(updateHeaders, 0, updateMsg, 0, 6);

        clientTCPSocket.Send(updateMsg); // 3: to server update level id
    }

    public static void UpdateNetPlayerLevelID(ref short[] shorts)
    {
        if (playerDList.ContainsKey(shorts[0]))
        {
            playerDList[shorts[0]] = playerDList[shorts[0]].SetPlayerLevelID(shorts[1]);
        }
        switch (localPlayer.playerLevelID)
        {
            case 0:
                LobbyManager.RefreshPlayerList();
                break;


            default:
                break;
        }
    }

    public static void CreateNewGame(ref short levelID)
    {
        localPlayer.playerKartID = SceneDataManager.instance.getData(SceneData.SelectedKart);
        Debug.Log("Local Kart Selected: " + localPlayer.playerKartID);

        localPlayer.playerLevelID = levelID;

        clientTCPSocket.Send(AddHeader(AddHeader(AddHeader(Encoding.ASCII.GetBytes(localPlayer.playerKartID), localPlayer.playerLevelID), localPlayer.playerID), 4)); // 4: to server set kart id
    }

    public static void UpdateNetPlayerRoom(ref short pID, ref short lvID, ref string kID)
    {
        if(playerDList.ContainsKey(pID))
        {
            playerDList[pID] = playerDList[pID].SetPlayerOnRoomCreated(kID, lvID);
            Debug.Log("Room Created: " + playerDList[pID].playerName + " KartID: " + playerDList[pID].playerKartID);
            Debug.Log("Level: " + playerDList[pID].playerLevelID);
        }

        switch (localPlayer.playerLevelID)
        {
            case 0:
                LobbyManager.RefreshPlayerList();
                break;
            default:
                break;
        }
    }

    public static void UpdateLocalTransform()
    {
        if(updateTimer >= updateTimeInterval)
        {
            float[] fPos = { localPlayer.playerTransform.position.x, localPlayer.playerTransform.position.y, localPlayer.playerTransform.position.z };
            float[] fRot = { localPlayer.playerTransform.rotation.eulerAngles.x, localPlayer.playerTransform.rotation.eulerAngles.y, localPlayer.playerTransform.rotation.eulerAngles.z };
            Vector2 moveVec = localPlayer.playerTransform.GetComponent<KartController>().GetMoveAction();
            float[] fInput = { moveVec.x, moveVec.y };

            byte[] byPos = new byte[12];
            byte[] byRot = new byte[12];
            byte[] byInput = new byte[8];

            Buffer.BlockCopy(fPos, 0, byPos, 0, 12);
            Buffer.BlockCopy(fRot, 0, byRot, 0, 12);
            Buffer.BlockCopy(fInput, 0, byInput, 0, 8);

            clientUDPSocket.SendTo(AddHeader(AddHeader(AddHeader(byPos, localPlayer.playerRoomID), localPlayer.playerID), 1), serverUDPEP);
            clientUDPSocket.SendTo(AddHeader(AddHeader(AddHeader(byRot, localPlayer.playerRoomID), localPlayer.playerID), 2), serverUDPEP);
            clientUDPSocket.SendTo(AddHeader(AddHeader(AddHeader(byInput, localPlayer.playerRoomID), localPlayer.playerID), 3), serverUDPEP);

            updateTimer -= updateTimeInterval;
        }
        updateTimer += Time.deltaTime;
    }

    public static void CalculateLatency()
    {
        if(!isLatencyCalcStarted)
        {
            short[] shorts = { 6, localPlayer.playerID };
            byte[] bytes = new byte[4];
            Buffer.BlockCopy(shorts, 0, bytes, 0, 4);
            clientTCPSocket.Send(bytes);
            isLatencyCalcStarted = true;
            latencyCounter = 0;
        }
        else
        {
            latencyCounter += Time.deltaTime;
        }
    }

    public static void LatencyToUI()
    {
        if(localPlayer.playerLevelID > 1)
        {
            if(GameplayUIManager.instance != null)
            {
                GameplayUIManager.instance.UpdateLatency(latency);
            }
        }
    }

    public static void JoinOnRequest()
    {
        if (playerDList.ContainsKey(joiningPlayerID))
        {
            if (playerDList[joiningPlayerID].playerLevelID > 1)
            {
                localPlayer.playerLevelID = playerDList[joiningPlayerID].playerLevelID;
                localPlayer.playerKartID = SceneDataManager.instance.getData(SceneData.SelectedKart);
                byte[] byKid = Encoding.ASCII.GetBytes(localPlayer.playerKartID);

                clientTCPSocket.Send(AddHeader(AddHeader(AddHeader(byKid, joiningPlayerID), localPlayer.playerID), 5));
            }
        }
    }

    public static void FinishScore(ref short score)
    {
        short[] shorts = { 9, localPlayer.playerID, score };
        byte[] bytes = new byte[6];
        Buffer.BlockCopy(shorts, 0, bytes, 0, 6);


        isLocalGameFinished = true;

        clientTCPSocket.Send(bytes);
    }

    public static void UpdateNetPlayerScore(ref short pID, ref short score)
    {
        if(playerDList.ContainsKey(pID))
        {
            if (playerDList[pID].playerRoomID == localPlayer.playerRoomID)
            {
                playerDList[pID] = playerDList[pID].SetPlayerScore(score);
                //Destroy(playerDList[pID].playerTransform.gameObject);
                Debug.Log("Get Score: " + score + " For: " + playerDList[pID].name);
            }
        }
    }

    static short GetHeader(byte[] bytes, int offset)
    {
        short[] sheader = new short[1];
        Buffer.BlockCopy(bytes, offset, sheader, 0, 2);
        return sheader[0];
    }

    static byte[] AddHeader(byte[] bytes, short header)
    {
        byte[] buffer = new byte[bytes.Length + 2];
        short[] sBuffer = { header };
        Buffer.BlockCopy(sBuffer, 0, buffer, 0, 2);
        Buffer.BlockCopy(bytes, 0, buffer, 2, bytes.Length);
        return buffer;
    }

    static byte[] GetContent(byte[] buffer, int offset, int count)
    {
        byte[] returnBy = new byte[count];
        Buffer.BlockCopy(buffer, offset, returnBy, 0, returnBy.Length);
        return returnBy;
    }

    private void OnApplicationQuit()
    {
        short[] quitShort = { -1 };
        byte[] quitMsg = new byte[2];
        Buffer.BlockCopy(quitShort, 0, quitMsg, 0, 2);
        clientTCPSocket.Send(quitMsg);
        isTCPReceiving = false;
        isUDPReceiving = false;
    }
}
