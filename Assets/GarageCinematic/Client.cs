using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Lec04
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    public GameObject myCube;
    //private static byte[] outBuffer = new byte[1024];
    private static IPEndPoint remoteEP;
    private static Socket clientSoc;



    private float[] floatPosition;
    private static byte[] Position;


    public static void StartClient()
    {
        try
        { //change ip address later to my own
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            remoteEP = new IPEndPoint(ip, 8888);

            clientSoc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);



        }
        catch (Exception e)
        {
            Debug.Log("Exception" + e.ToString());
        }
    }

    private void sendDataPosition()
    {
        floatPosition = new float[] { myCube.transform.position.x, myCube.transform.position.y, myCube.transform.position.z };

        Position = new byte[floatPosition.Length * 4];

        Buffer.BlockCopy(floatPosition, 0, Position, 0, Position.Length);

        clientSoc.SendTo(Position, remoteEP);

        Debug.Log("SendPositionCoodinates " + myCube.transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        myCube = GameObject.Find("Cube");
        StartClient();


    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            // Debug.Log("SendPositionCoodinates " + myCube.transform.position);
            sendDataPosition();
        }


        //outBuffer = Encoding.ASCII.GetBytes(myCube.transform.position.ToString());


        // clientSoc.SendTo(outBuffer, remoteEP);

        //Debug.Log("Translate: " + transform.position);

    }
}
