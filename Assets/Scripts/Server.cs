using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Unity.Collections;
using Unity.Networking.Transport;

using UnityEngine;

public class Server : MonoBehaviour
{

    private NetworkDriver m_Driver;
    [NonSerialized] public NativeList<NetworkConnection> m_Connections;
    public static List<NetworkUser> users = new List<NetworkUser>();
    private MessageHandler msgHandler;
    private int playerCount = 0;

    private void Start()
    {
        m_Driver = NetworkDriver.Create();
        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

        var endpoint = NetworkEndpoint.AnyIpv4.WithPort(7777);
        if (m_Driver.Bind(endpoint) != 0)
        {
            Debug.LogError("Server: Failed to bind to port 7777.");
            return;
        }

        m_Driver.Listen();

        msgHandler = FindObjectOfType<MessageHandler>();
    }

    private void OnDestroy()
    {
        if (m_Driver.IsCreated)
        {
            m_Driver.Dispose();
            m_Connections.Dispose();
        }
    }

    private void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        CleanConnections();
        AcceptNewConnections();
        HandleConnections();
    }

    private void CleanConnections()
    {
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                i--;
            }
        }
    }

    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default)
        {
            m_Connections.Add(c);
            Debug.Log("Server: Accepted a connection.");
        }
    }

    private void HandleConnections()
    {
        for (int i = 0; i < m_Connections.Length; i++)
        {
            DataStreamReader stream;
            NetworkEvent.Type cmd;

            while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)//Received message types
                {
                    MessageHandler.Message message = JsonUtility.FromJson<MessageHandler.Message>(stream.ReadFixedString512().ToString());
                    Debug.Log($"Server: Got {message.cmd} from client {message.connId}-{message.userId}");

                    switch (message.cmd)
                    {
                        default:
                        case MessageHandler.CommandType.None:
                            break;
                        case MessageHandler.CommandType.CreateUserRequest:
                            GetComponent<ServerUserSignup>().SignupAsUser(this, message);
                            break;
                        case MessageHandler.CommandType.LoginUserRequest:
                            GetComponent<ServerUserLogin>().LoginAsUser(this, message);
                            break;
                        case MessageHandler.CommandType.UpdateUserRequest:
                            GetComponent<ServerUserEdit>().UpdateUser(this, message);
                            break;
                        case MessageHandler.CommandType.InitGameRequest:
                            users.Add(new NetworkUser() { conn = m_Connections[i], userId = message.userId, score = 6 });
                            if (users.Count == 2)
                            {
                                MessageHandler.Message[] msgs = msgHandler.Sv_ClientJoin();

                                for (int m = 0; m < msgs.Length; m++)
                                {
                                    users.First(x => x.conn == m_Connections[m]).type = (ClickType)Enum.Parse(typeof(ClickType), msgs[m].type);
                                    SendToClientConn512(m, msgs[m]);
                                }
                            }
                            break;
                        case MessageHandler.CommandType.NewGameRequest:
                            playerCount++;

                            if (playerCount == 2)
                            {
                                playerCount = 0;

                                for (int u = 0; u < users.Count; u++)
                                {
                                    users[u].score = 6;
                                }

                                SendToClients(msgHandler.Sv_NewGame());
                            }
                            break;
                        case MessageHandler.CommandType.PlaceTypeRequest:
                            SendToClientId(message.userId, msgHandler.C_RequestPlaceType(message.pos));
                            break;
                        case MessageHandler.CommandType.DoTurn:
                            MessageHandler.Message returnMsg = msgHandler.Sv_DoTurn(message);
                            if (returnMsg.cmd == MessageHandler.CommandType.WinLose)
                            {
                                NetworkUser win = users.First(x => x.userId == message.userId);
                                NetworkUser lose = users.First(x => x.userId != message.userId);

                                GetComponent<ServerInsertScore>().InsertScore(win.userId, win.score, 1);
                                GetComponent<ServerInsertScore>().InsertScore(lose.userId, 0, 0);
                            }

                            SendToClients(returnMsg);
                            break;
                        case MessageHandler.CommandType.HighscoreRequest:
                            GetComponent<ServerGetHighscores>().GetHighscores(this, message);
                            break;
                    }
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Server: Client disconnected from the server.");
                    users.RemoveAll(x => x.conn == m_Connections[i]);
                    m_Connections[i] = default;
                    break;
                }
            }
        }
    }

    public void SendToClients(MessageHandler.Message _msg)
    {
        string json = JsonUtility.ToJson(_msg);

        for (int i = 0; i < m_Connections.Length; i++)
        {
            m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out var writer);
            writer.WriteFixedString512(json);
            m_Driver.EndSend(writer);
        }
    }

    public void SendToClientId(int _userId, MessageHandler.Message _msg)
    {
        string json = JsonUtility.ToJson(_msg);

        m_Driver.BeginSend(NetworkPipeline.Null, users.First(x => x.userId == _userId).conn, out var writer);
        writer.WriteFixedString512(json);
        m_Driver.EndSend(writer);
    }

    public void SendToClientConn512(int _conn, MessageHandler.Message _msg)
    {
        string json = JsonUtility.ToJson(_msg);

        m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[_conn], out var writer);
        writer.WriteFixedString512(json);
        m_Driver.EndSend(writer);
    }

    public void SendToClientConn4096(int _conn, MessageHandler.Message _msg)
    {
        string json = JsonUtility.ToJson(_msg);

        m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[_conn], out var writer);
        writer.WriteFixedString4096(json);
        m_Driver.EndSend(writer);
    }
}
