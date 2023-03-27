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

public class NetworkManager : MonoBehaviour
{
    // Connect control
    public static bool isOnNetwork = true;

    // Thread control
    private static Mutex mutex = new Mutex();
    public static List<Thread> threads = new List<Thread>();

    // Sockets
    private static IPEndPoint remoteEP;
    private static Socket clientTCPSocket;
    private static Socket clientUDPSocket;

    // Memory
    public static byte[] tcpReceiveBuffer = new byte[1024];
    public static byte[] tcpSendBuffer = new byte[1024];

    public static byte[] udpReceiveBuffer = new byte[1024];
    public static byte[] udpSendBuffer = new byte[1024];

    // Player ID
    public short displayedPlayerID;
    public static short localPlayerID;

    // UDP Send
    public float sendInterval = 0.3f;
    float udpTimer = 0.0f;

    // Latency
    public float latencyDetectInterval = 1.0f;
    float latencyCheckTimer = 0.0f;
    public static float checkedLatency = 0.0f;
    private static bool isStartedCalculateLatency = false;

    // FLAGS
    public static bool isLogin = false;
    public static bool isUDPSetup = false;
    public static bool isReceiveUDP = false;


    private static bool isUDPReceiving = false;
    private static bool isLocalPlayerSetup = false;

    private static string localPlayerName = "DefualtName";
    private static string localPlayerKartID = "008";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        if (SceneDataManager.instance)
        {
            if (SceneDataManager.instance.getData(SceneData.SelectedMode) == "Online")
            {
                IPAddress ip;
                //ip = IPAddress.Parse("192.168.2.43");
                ip = Dns.GetHostAddresses("jeffrey9911.ddns.net")[0];
                
                remoteEP = new IPEndPoint(ip, 12581);

                clientTCPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientUDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                localPlayerName = SceneDataManager.instance.getData(SceneData.SelectedName);
                localPlayerKartID = SceneDataManager.instance.getData(SceneData.SelectedKart);

                Thread thread = new Thread(() => clientTCPConnect(clientTCPSocket, remoteEP));
                //Task.Run(() => { clientTCPConnect(clientTCPSocket, remoteEP); }, cts.Token);        // TCP Connect Thread
            }
        }
        else
        {
            if (isOnNetwork)
            {
                IPAddress ip;
                //ip = IPAddress.Parse("192.168.2.43");
                ip = Dns.GetHostAddresses("jeffrey9911.ddns.net")[0];
                
                remoteEP = new IPEndPoint(ip, 12581);

                clientTCPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientUDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                Thread thread = new Thread(() => clientTCPConnect(clientTCPSocket, remoteEP));
                //Task.Run(() => { clientTCPConnect(clientTCPSocket, remoteEP); }, cts.Token);        // TCP Connect Thread
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if(isOnNetwork)
        {
            udpTimer += Time.deltaTime;
            latencyCheckTimer += Time.deltaTime;

            if (isLocalPlayerSetup && udpTimer >= sendInterval)
            {
                clientUDPSend("Transform");

                udpTimer -= sendInterval;
            }

            if (latencyCheckTimer >= latencyDetectInterval)
            {

                clientTCPSend("Latency");
                latencyCheckTimer -= latencyDetectInterval;
            }

            if (isStartedCalculateLatency)
            {
                checkedLatency += Time.deltaTime;
            }
            displayedPlayerID = localPlayerID;
        }
        
    }

    static void clientTCPConnect(Socket clientSocket, IPEndPoint serverEP)                  // TCP Connect
    {
        clientSocket.Connect(serverEP);

        Debug.Log("Connected!");

        short[] header = { 0 };

        //string initString = GameplayManager.instance.playerManager.localPlayerName + "#" + GameplayManager.instance.playerManager.localPlayerKartID;
        if(SceneDataManager.instance)
        {

        }
        string initString = localPlayerName + "#" + localPlayerKartID;
        Debug.Log("To Send: " + header[0].ToString() + initString);
        byte[] initByte = Encoding.ASCII.GetBytes(initString);
        byte[] initMsg = new byte[header.Length * 2 + initString.Length];

        Buffer.BlockCopy(header, 0, initMsg, 0, header.Length * 2);

        Buffer.BlockCopy(initByte, 0, initMsg, header.Length * 2, initByte.Length);

        clientSocket.Send(initMsg);

        Debug.Log("FIRST SEND");

        Task.Run(() => { clientTCPReceive(); }, cts.Token);                     // TCP Receive Thread
        Task.Run(() => { clientUDPReceive(); }, cts.Token);             // UDP Receive Thread

    }

    static void clientTCPReceive()                                              // TCP Receive
    {
        //Debug.Log("Start Receive");
        try
        {
            byte[] buffer = new byte[1024];
            int recv = clientTCPSocket.Receive(buffer);

            short[] shortBuffer = new short[1];
            Buffer.BlockCopy(buffer, 0, shortBuffer, 0, 2);
            
            //Debug.Log("Received!");

            switch (shortBuffer[0])
            {
                // First Login
                case 0:
                    byte[] idInfo = new byte[recv - 2];
                    Buffer.BlockCopy(buffer, 2, idInfo, 0, idInfo.Length);
                    Buffer.BlockCopy(idInfo, 0, shortBuffer, 0, idInfo.Length);
                    if (shortBuffer[0] >= 1000) localPlayerID = shortBuffer[0];
                    Debug.Log("PlayerID Set TO:" + localPlayerID);
                    isLocalPlayerSetup = true;
                    
                    break;

                case 1:
                    byte[] newPlayerInfo = new byte[recv - 2];
                    Buffer.BlockCopy(buffer, 2, newPlayerInfo, 0, recv - 2);
                    UnityMainThreadDispatcher.Instance().Enqueue(() => PlayerManager.CheckPlayerDList(ref newPlayerInfo));
                    break;

                case 9:
                    UnityMainThreadDispatcher.Instance().Enqueue(() => CalculateLatency(1));
                    break;

                default:
                    Debug.Log("Default");
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
            throw;
        }

        clientTCPReceive();
    }

    void clientTCPSend(string sendType)
    {
        switch (sendType)
        {
            case "Latency":
                if(!isStartedCalculateLatency)
                {
                    byte[] buffer = new byte[2];
                    short[] shortBuffer = { 9 };
                    Buffer.BlockCopy(shortBuffer, 0, buffer, 0, buffer.Length);

                    clientTCPSocket.Send(buffer);

                    CalculateLatency(0);
                }
                
                
                break;


            default:
                break;
        }

        
    }

    void clientUDPSend(string sendType)
    {
        switch (sendType)
        {
            case "Transform":
                if (isLocalPlayerSetup)
                {
                    byte[] buffer = new byte[36];
                    short[] shortBuffer = { 0, localPlayerID };
                    Buffer.BlockCopy(shortBuffer, 0, buffer, 0, 4);
                    float[] playerTrans = { GameplayManager.instance.playerManager.localPlayer.transform.position.x,
                                        GameplayManager.instance.playerManager.localPlayer.transform.position.y,
                                        GameplayManager.instance.playerManager.localPlayer.transform.position.z,
                                        GameplayManager.instance.playerManager.localPlayer.transform.rotation.eulerAngles.x,
                                        GameplayManager.instance.playerManager.localPlayer.transform.rotation.eulerAngles.y,
                                        GameplayManager.instance.playerManager.localPlayer.transform.rotation.eulerAngles.z,
                                        GameplayManager.instance.playerManager.localPlayer.GetComponent<KartController>().GetMoveAction().x,
                                        GameplayManager.instance.playerManager.localPlayer.GetComponent<KartController>().GetMoveAction().y};
                    Buffer.BlockCopy(playerTrans, 0, buffer, 4, 32);
                    clientUDPSocket.SendTo(buffer, remoteEP);

                    // First UDP sent
                    if (!isUDPReceiving) isUDPReceiving = true;
                }
                break;

            default:
                break;
        }
    }

    static void clientUDPReceive()                                      // UDP Receive
    {
        while (!isUDPReceiving)
        {
            // Waiting for first UDP Sent
        }

        if (isUDPReceiving)
        {
            byte[] receiveBuffer = new byte[1024];
            int recv = clientUDPSocket.Receive(receiveBuffer);
            short[] receiveHeader = new short[1];
            Buffer.BlockCopy(receiveBuffer, 0, receiveHeader, 0, 2);

            switch (receiveHeader[0])
            {
                // Player Transform
                case 0:
                    byte[] buffer = new byte[recv - 2];
                    Buffer.BlockCopy(receiveBuffer, 2, buffer, 0, buffer.Length);

                    for (int i = 0; i < recv / 34; i++)
                    {
                        byte[] transBuffer = new byte[34];
                        Buffer.BlockCopy(buffer, i * 34, transBuffer, 0, 34);

                        try
                        {
                            UnityMainThreadDispatcher.Instance().Enqueue(() => PlayerManager.UpdateOnNetPlayer(ref transBuffer));
                        }
                        catch (Exception ex)
                        {
                            Debug.Log(ex.ToString());
                            throw;
                        }
                    }
                    break;

                default:
                    break;
            }

            

        }


        clientUDPReceive();
    }

    static void CalculateLatency(int operate)
    {
        switch (operate)
        {
            case 0:
                checkedLatency = 0.0f;
                isStartedCalculateLatency = true;
                break;

            case 1:
                GameplayManager.instance.gameplayUIManager.UpdateLatency(checkedLatency);
                isStartedCalculateLatency = false;
                break;

            default:
                break;
        }
    }


    private void OnApplicationQuit()
    {
        cts.Cancel();
        clientTCPSocket.Close();
        clientUDPSocket.Close();
        Destroy(UnityMainThreadDispatcher.Instance());
    }
}
