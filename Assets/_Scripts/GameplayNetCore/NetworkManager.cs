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

    private void Awake()
    {
        
        if (!instance) instance = this;
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
                        Debug.Log("TCP0");
                        if (!isFirstInitialized)
                        {
                            string allPlayer = Encoding.ASCII.GetString(GetContent(recvBuffer, 2));
                            UnityMainThreadDispatcher.Instance().Enqueue(() => InitialPlayerList(ref allPlayer));
                        }
                        else
                        {
                            Debug.Log("Initialize Ignored");
                        }


                        break;

                    case 1:
                        Debug.Log("TCP1"); // new player
                        short idToAdd = GetHeader(recvBuffer, 2);
                        string nameToAdd = Encoding.ASCII.GetString(GetContent(recvBuffer, 4));
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AddPlayer(ref idToAdd, ref nameToAdd));
                        break;

                    case -1:
                        Debug.Log("TCP-1"); // remove player
                        short idToRemove = GetHeader(recvBuffer, 2);
                        UnityMainThreadDispatcher.Instance().Enqueue(() => RemovePlayer(ref idToRemove));
                        break;

                    case 2:
                        Debug.Log("TCP2"); // New message
                        string msgReceived = Encoding.ASCII.GetString(GetContent(recvBuffer, 2));
                        UnityMainThreadDispatcher.Instance().Enqueue(() => ReceivedMessage(ref msgReceived));

                        break;
                    case 9:
                        Debug.Log("TCP9");
                        short pid = GetHeader(recvBuffer, 2);
                        Debug.Log("9: " + pid);

                        string newPlayerName = Encoding.ASCII.GetString(GetContent(recvBuffer, 4));
                        Debug.Log("9: " + newPlayerName);

                        //UnityMainThreadDispatcher.Instance().Enqueue(() => NetPlayerManager.AddPlayer(ref pid, ref newPlayerName));

                        break;

                    case 999:
                        Debug.Log("TCP 999");
                        short quitID = GetHeader(recvBuffer, 2);

                        //UnityMainThreadDispatcher.Instance().Enqueue(() => NetPlayerManager.DeletePlayer(ref quitID));

                        break;
                    default:
                        break;
                }
            }
        }
        catch (Exception ex)
        {

        }
        finally
        {
            clientTCPSocket.Close();
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

        LobbyManager.RefreshPlayerList();
    }

    public static void RemovePlayer(ref short pID)
    {
        if(playerDList.ContainsKey(pID))
        {
            switch (playerDList[pID].playerLevelID)
            {
                case 0: // lobby
                    playerDList.Remove(pID);
                    LobbyManager.RefreshPlayerList();
                    break;

                case 1:
                    break;

                case 2:
                    break;

                case 3:
                    break;
                default:
                    break;
            }
        }
    }

    static void ClientUDPReceive()
    {
        try
        {
            while(isUDPReceiving)
            {
                byte[] recvBuffer = new byte[1024];
                int recv = clientUDPSocket.Receive(recvBuffer);

                switch (GetHeader(recvBuffer, 0))
                {
                    case 0:
                        break;

                    case 1:
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

    public static void SendMessage(ref string msgToSend)
    {
        clientTCPSocket.Send(AddHeader(AddHeader(Encoding.ASCII.GetBytes(msgToSend), localPlayer.playerID), 2));
    }
    public static void ReceivedMessage(ref string msgReceived)
    {
        LobbyManager.AddMessage(ref msgReceived);
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

    static byte[] GetContent(byte[] buffer, int offset)
    {
        byte[] returnBy = new byte[buffer.Length - offset];
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
