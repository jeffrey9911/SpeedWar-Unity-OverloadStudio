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
    private static CancellationTokenSource cts = new CancellationTokenSource();

    public static bool isOnNetwork = true;

    // Sockets
    private static IPEndPoint remoteEP;
    private static Socket clientTCPSocket;
    private static Socket clientUDPSocket;

    // Player ID
    public short displayedPlayerID;
    public static short localPlayerID;


    // UDP Send
    public float sendInterval = 1.0f;
    float udpTimer = 0.0f;

    // Latency
    public float latencyDetectInterval = 1.0f;
    float latencyCheckTimer = 0.0f;
    static float checkedLatency = 0.0f;
    private static bool isStartedCalculateLatency = false;


    private static bool isUDPReceiving = false;
    private static bool isLocalPlayerSetup = false;


    // Start is called before the first frame update
    void Start()
    {
        if(isOnNetwork)
        {
            IPAddress ip;
            //ip = IPAddress.Parse("192.168.2.43");
            ip = Dns.GetHostAddresses("jeffrey9911.ddns.net")[0];
            Debug.Log(ip.Address.ToString());
            remoteEP = new IPEndPoint(ip, 12581);

            clientTCPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientUDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


            Task.Run(() => { clientTCPConnect(clientTCPSocket, remoteEP); }, cts.Token);        // TCP Connect Thread
        }
    }


    // Update is called once per frame
    void Update()
    {
        udpTimer += Time.deltaTime;
        latencyCheckTimer += Time.deltaTime;

        if(isLocalPlayerSetup && udpTimer >= sendInterval)
        {
            clientUDPSend("Transform");

            udpTimer -= sendInterval;
        }

        if(latencyCheckTimer >= latencyDetectInterval)
        {

            clientTCPSend("Latency");
            latencyCheckTimer -= latencyDetectInterval;
        }

        if(isStartedCalculateLatency)
        {
            checkedLatency += Time.deltaTime;
        }

        
        
    }

    static void clientTCPConnect(Socket clientSocket, IPEndPoint serverEP)                  // TCP Connect
    {
        clientSocket.Connect(serverEP);

        Debug.Log("Connected!");

        short[] header = { 0 };
        byte[] playerName = Encoding.ASCII.GetBytes("TestPlayerName");

        byte[] initMsg = new byte[header.Length * 2 + playerName.Length];

        Buffer.BlockCopy(header, 0, initMsg, 0, header.Length * 2);

        Buffer.BlockCopy(playerName, 0, initMsg, header.Length * 2, playerName.Length);

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
                    byte[] byContent = new byte[recv - 2];
                    Buffer.BlockCopy(buffer, 2, byContent, 0, byContent.Length);
                    Buffer.BlockCopy(byContent, 0, shortBuffer, 0, byContent.Length);
                    if (shortBuffer[0] >= 1000) localPlayerID = shortBuffer[0];
                    Debug.Log("PlayerID Set TO:" + localPlayerID);
                    isLocalPlayerSetup = true;
                    
                    break;

                case 1:
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
                    short[] shortBuffer = { 1 };
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
                    byte[] buffer = new byte[28];
                    short[] shortBuffer = { 0, localPlayerID };
                    Buffer.BlockCopy(shortBuffer, 0, buffer, 0, 4);
                    float[] playerTrans = { GameplayManager.instance.playerManager.localPlayer.transform.position.x,
                                        GameplayManager.instance.playerManager.localPlayer.transform.position.y,
                                        GameplayManager.instance.playerManager.localPlayer.transform.position.z,
                                        GameplayManager.instance.playerManager.localPlayer.transform.rotation.eulerAngles.x,
                                        GameplayManager.instance.playerManager.localPlayer.transform.rotation.eulerAngles.y,
                                        GameplayManager.instance.playerManager.localPlayer.transform.rotation.eulerAngles.z};
                    Buffer.BlockCopy(playerTrans, 0, buffer, 4, 24);
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

                    for (int i = 0; i < recv / 26; i++)
                    {
                        byte[] transBuffer = new byte[26];
                        Buffer.BlockCopy(buffer, i * 26, transBuffer, 0, 26);

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
    }
}
