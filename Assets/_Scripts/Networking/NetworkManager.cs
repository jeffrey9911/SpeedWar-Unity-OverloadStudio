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
    private static byte[] data = new byte[1024];
    private static IPEndPoint remoteEP;
    private static Socket clientSocket;

    public static int playerID;



    // Start is called before the first frame update
    void Start()
    {
        IPAddress ip = IPAddress.Parse("192.168.2.43");
        remoteEP = new IPEndPoint(ip, 12581);
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        clientSocket.Connect(remoteEP);

        data = Encoding.ASCII.GetBytes("TestPlayerName");
        clientSocket.Send(data);

        Task.Run(() => { clientReceive(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static void clientReceive()
    {
        byte[] buffer = new byte[1024];
        int recv = clientSocket.Receive(buffer);

        string str = Encoding.ASCII.GetString(buffer);

        Debug.Log("GETT! : " + str);
        playerID = int.Parse(str);
    }
}
