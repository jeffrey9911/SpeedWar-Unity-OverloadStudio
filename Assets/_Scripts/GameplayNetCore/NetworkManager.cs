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

    private static IPEndPoint remoteEP;
    private static Socket clientTCPSocket;
    private static Socket clientUDPSocket;

    public short displayedPlayerID;
    public static short localPlayerID;



    public float sendInterval = 1.0f;
    float timer = 0.0f;

    private static bool isUpdatingNetPlayer = false;
    private static bool isLocalPlayerSetup = false;


    // Start is called before the first frame update
    void Start()
    {
        IPAddress ip;
        //ip = IPAddress.Parse("192.168.2.43");
        ip = Dns.GetHostAddresses("jeffrey9911.ddns.net")[0];
        Debug.Log(ip.Address.ToString());
        remoteEP = new IPEndPoint(ip, 12581);

        clientTCPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientUDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


        Task.Run(() => { clientTCPConnect(clientTCPSocket, remoteEP); }, cts.Token);
        //ask.Run(() => { TestUDPReceive(); }, cts.Token);
    }


    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        clientUDPSend("Transform");
        displayedPlayerID = localPlayerID;
    }

    static void clientTCPConnect(Socket clientSocket, IPEndPoint serverEP)
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

        Task.Run(() => { clientTCPReceive(); }, cts.Token);
        Task.Run(() => { clientUDPReceive(); }, cts.Token);
    }

    static void clientTCPReceive()
    {
        Debug.Log("Start Receive");
        try
        {
            byte[] buffer = new byte[1024];
            int recv = clientTCPSocket.Receive(buffer);

            short[] shortBuffer = new short[1];
            Buffer.BlockCopy(buffer, 0, shortBuffer, 0, 2);
            byte[] byContent = new byte[recv - 2];
            Buffer.BlockCopy(buffer, 2, byContent, 0, byContent.Length);
            Debug.Log("Received!");

            switch (shortBuffer[0])
            {
                // First Login
                case 0:
                    Buffer.BlockCopy(byContent, 0, shortBuffer, 0, byContent.Length);
                    localPlayerID = shortBuffer[0];
                    Debug.Log("PlayerID Set TO:" + localPlayerID);
                    isLocalPlayerSetup = true;
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
        

    }

    void clientUDPSend(string sendType)
    {
        switch (sendType)
        {
            case "Transform":
                if (localPlayerID >= 1000 && timer >= sendInterval)
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

                    timer -= sendInterval;
                    if (!isUpdatingNetPlayer) isUpdatingNetPlayer = true;
                }
                break;

            default:
                break;
        }

        
    }

    static void clientUDPReceive()
    {
        if (isUpdatingNetPlayer)
        {
            byte[] buffer = new byte[260];
            int recv = clientUDPSocket.Receive(buffer);

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

        }
        else
        {
            Debug.Log("IS NOT UPDATING");
        }
        clientUDPReceive();
    }


    /*
    public static void ConPrint(byte[] buffer)
    {
        short[] shortBuffer = new short[1];
        float[] fPos = { 0, 0, 0 };
        float[] fRot = { 0, 0, 0 };

        Buffer.BlockCopy(buffer, 0, shortBuffer, 0, 2);
        Buffer.BlockCopy(buffer, 0 + 2, fPos, 0, fPos.Length * 4);
        Buffer.BlockCopy(buffer, 0 + 2 + 12, fRot, 0, fRot.Length * 4);

        Debug.Log(shortBuffer[0] + ": " + fPos[0] + " " + fPos[1] + " " + fPos[2]);
        Debug.Log(shortBuffer[0] + ": " + fRot[0] + " " + fRot[1] + " " + fRot[2]);
    }*/

    private void OnApplicationQuit()
    {
        cts.Cancel();
        clientTCPSocket.Close();
        clientUDPSocket.Close();
    }
}
