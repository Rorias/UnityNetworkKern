using Unity.Networking.Transport;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    public static string serverIP = "192.168.2.225";

    private MessageHandler msgHandler;

    private NetworkDriver m_Driver;
    private NetworkConnection m_Connection;

    private void Start()
    {
        m_Driver = NetworkDriver.Create();

        m_Connection = default(NetworkConnection);

        var endpoint = NetworkEndpoint.Parse(serverIP, 7777, NetworkFamily.Ipv4);
        endpoint.Port = 7777;
        m_Connection = m_Driver.Connect(endpoint);

        msgHandler = FindObjectOfType<MessageHandler>();
    }

    private void OnDestroy()
    {
        m_Driver.Dispose();
    }

    private void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated) { return; }

        HandleConnection();
    }

    private void HandleConnection()
    {
        Unity.Collections.DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("Client: We are now connected to the server.");

                SendToServer(msgHandler.C_ConnectToServer());
            }
            else if (cmd == NetworkEvent.Type.Data)//Received message types
            {
                MessageHandler.Message message = JsonUtility.FromJson<MessageHandler.Message>(stream.ReadFixedString4096().ToString());
                Debug.Log($"Client: Got {message.cmd} from the server.");

                switch (message.cmd)
                {
                    default:
                    case MessageHandler.CommandType.None:
                        break;
                    case MessageHandler.CommandType.LoginUserAccept:
                        SendToServer(msgHandler.C_SendInitGameRequest(message));
                        break;
                    case MessageHandler.CommandType.UpdateUserAccept:
                        msgHandler.C_UpdateUser(message);
                        break;
                    case MessageHandler.CommandType.SetType:
                        Debug.Log(DatabaseConnection.user.username + " is " + message.type);
                        msgHandler.C_SetClickType(message.type);
                        break;
                    case MessageHandler.CommandType.PlaceTypeAccept:
                        if (message.free)
                        {
                            SendToServer(msgHandler.C_PlaceType(message.pos));
                        }
                        break;
                    case MessageHandler.CommandType.ShowTurn:
                        msgHandler.C_ShowTurn(message);
                        break;
                    case MessageHandler.CommandType.WinLose:
                        msgHandler.C_WinLose(message);
                        break;
                    case MessageHandler.CommandType.Tie:
                        msgHandler.C_Tie(message);
                        break;
                    case MessageHandler.CommandType.NewGameAccept:
                        msgHandler.C_NewGame();
                        break;
                    case MessageHandler.CommandType.HighscoreAccept:
                        GetComponent<ClientHighscores>().LoadHighscores(message.message);
                        break;
                    case MessageHandler.CommandType.Error:
                        msgHandler.C_ShowError(message.message);
                        break;
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client: Client got disconnected from server.");
                m_Connection = default;
            }
        }
    }

    public void SendToServer(MessageHandler.Message _msg)
    {
        if (_msg.cmd != MessageHandler.CommandType.None)
        {
            string json = JsonUtility.ToJson(_msg);

            m_Driver.BeginSend(m_Connection, out var writer);
            writer.WriteFixedString512(json);
            m_Driver.EndSend(writer);
        }
    }

    public void Disconnect()
    {
        m_Connection.Disconnect(m_Driver);
        m_Connection = default;

        SceneManager.LoadScene(0);
    }
}
