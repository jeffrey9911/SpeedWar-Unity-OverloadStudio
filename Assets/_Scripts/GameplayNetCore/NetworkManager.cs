using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

using System.Threading.Tasks;

public class NetworkManager : MonoBehaviour
{
    private static IPEndPoint remoteEP;
    private static Socket clientTCPSocket;
    private static Socket clientUDPSocket;

    public static short localPlayerID;

    public Dictionary<short, GameObject> onNetPlayerDList = new Dictionary<short, GameObject>();

    public float sendInterval = 1.0f;
    float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //IPAddress ip = IPAddress.Parse("174.93.70.156");

        IPAddress ip;
        //ip = IPAddress.Parse("174.93.70.156");
        ip = Dns.GetHostAddresses("jeffrey9911.ddns.net")[0];
        Debug.Log(ip.Address.ToString());
        remoteEP = new IPEndPoint(ip, 12581);

        clientTCPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientUDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


        Task.Run(() => { clientConnect(clientTCPSocket, remoteEP); });
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (localPlayerID > 100 && timer >= sendInterval)
        {
            byte[] buffer = new byte[16];
            short[] shortBuffer = { 0, localPlayerID };
            Buffer.BlockCopy(shortBuffer, 0, buffer, 0, 4);
            float[] playerPosition = { GameplayManager.instance.playerManager.localPlayer.transform.position.x, GameplayManager.instance.playerManager.localPlayer.transform.position.y, GameplayManager.instance.playerManager.localPlayer.transform.position.z };
            Buffer.BlockCopy(playerPosition, 0, buffer, 4, 12);
            clientUDPSocket.SendTo(buffer, remoteEP);
            Debug.Log("POSITION SENT!");
            timer -= sendInterval;
        }
        else
        {
            //Debug.Log("NOT SENDING");
        }
        
    }

    static void clientConnect(Socket clientSocket, IPEndPoint serverEP)
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

        Task.Run(() => { clientReceive(); });
    }

    static void clientReceive()
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
}
