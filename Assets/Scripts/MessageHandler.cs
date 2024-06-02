using System;

using TMPro;

using UnityEngine;

public class MessageHandler : MonoBehaviour
{
    public TMP_Text serverError;

    public enum CommandType
    {
        None,               //empty
        CreateUserRequest,  //client requests server to create an account and login to it so that they may play
        UpdateUserRequest,  //client requests server to update data of the account
        UpdateUserAccept,   //client requests server to update data of the account
        LoginUserRequest,   //client requests server to login this user and return their id and username
        LoginUserAccept,    //server returns the id and username of the client that asked
        SetType,            //server sets the type of object for each client. so either circle or cross
        PlaceTypeRequest,   //client requests if the clicked position is available for placing a type
        PlaceTypeAccept,    //server returns if the position is available for placing
        DoTurn,             //client requests the server to do the turn since it was accepted
        ShowTurn,           //server tells the clients to show the turn that was done
        Tie,                //server tells the clients they tied
        WinLose,            //server tells the clients whether they won or lost
        InitGameRequest,    //client requests an initial game to be started
        NewGameRequest,     //client requests a new game to be started with the same players
        NewGameAccept,      //server returns the new game state when both players requested a new game
        HighscoreRequest,   //client requests the highscore list to be shown to them
        HighscoreAccept,    //server returns the highscore list to the player that requested it
        Error,              //server tells the client there was an error with their database request, and what the error was
    };

    public struct Message
    {
        public int connId;
        public CommandType cmd;
        public string pos;
        public string type;
        public bool free;
        public string message;

        public int userId;
        public string email;
        public string usrname;
        public string bdate;
        public string pass;
        public string sessId;
    }

    private ClientTurn ct;
    private ServerTurn st;

    private void Awake()
    {
        ct = FindObjectOfType<ClientTurn>();
        st = FindObjectOfType<ServerTurn>();
    }

    public Message C_ConnectToServer()
    {
        Message msg;

        if (string.IsNullOrWhiteSpace(DatabaseConnection.user.username))
        {
            msg = new Message()
            {
                cmd = CommandType.LoginUserRequest,
                email = DatabaseConnection.user.email,
                pass = DatabaseConnection.user.password,
            };
        }
        else
        {
            msg = new Message()
            {
                cmd = CommandType.CreateUserRequest,
                email = DatabaseConnection.user.email,
                usrname = DatabaseConnection.user.username,
                bdate = DatabaseConnection.user.birthdate,
                pass = DatabaseConnection.user.password,
            };
        }

        return msg;
    }

    public void C_ShowError(string _error)
    {
        serverError.text = _error;
        Invoke("C_HideError", 1.5f);
    }

    private void C_HideError()
    {
        serverError.text = "";
    }

    public void C_SetClickType(string _type)
    {
        ClickType c = (ClickType)Enum.Parse(typeof(ClickType), _type);
        ct.SetType(c);
    }

    public Message C_PlaceType(string _pos)
    {
        return new Message()
        {
            connId = DatabaseConnection.userData.connid,
            cmd = CommandType.DoTurn,
            userId = DatabaseConnection.userData.id,
            pos = _pos,
            type = ct.type.ToString(),
        };
    }

    public Message C_SendInitGameRequest(Message _incoming)
    {
        DatabaseConnection.userData = new UserResult() { id = _incoming.userId, username = _incoming.usrname };
        DatabaseConnection.user.username = DatabaseConnection.userData.username;
        DatabaseConnection.userData.connid = _incoming.connId;

        Message msg = new Message()
        {
            connId = DatabaseConnection.userData.connid,
            cmd = CommandType.InitGameRequest,
            userId = DatabaseConnection.userData.id,
        };

        return msg;
    }

    public Message C_RequestPlaceType(string _position)
    {
        Message msg = new Message()
        {
            connId = DatabaseConnection.userData.connid,
            cmd = CommandType.PlaceTypeAccept,
            userId = DatabaseConnection.userData.id,
            pos = _position,
            free = st.TryTurn(_position),
        };

        return msg;
    }

    public void C_UpdateUser(Message _incoming)
    {
        if (!string.IsNullOrWhiteSpace(_incoming.email))
        {
            DatabaseConnection.user.email = _incoming.email;
        }

        if (!string.IsNullOrWhiteSpace(_incoming.usrname))
        {
            DatabaseConnection.user.username = _incoming.usrname;
        }

        if (!string.IsNullOrWhiteSpace(_incoming.pass))
        {
            DatabaseConnection.user.password = _incoming.pass;
        }

        DatabaseConnection.userData.username = DatabaseConnection.user.username;
    }

    public void C_ShowTurn(Message _incoming)
    {
        Vector2Int pos = new Vector2Int(Convert.ToInt32(_incoming.pos.Split(',')[0]), Convert.ToInt32(_incoming.pos.Split(',')[1]));
        ClickType type = (ClickType)Enum.Parse(typeof(ClickType), _incoming.type);

        ct.ShowTurn(pos, type);
    }

    public void C_WinLose(Message _incoming)
    {
        C_ShowTurn(_incoming);

        ClickType type = (ClickType)Enum.Parse(typeof(ClickType), _incoming.type);
        ct.EndGame(type, Convert.ToInt32(_incoming.message));
    }

    public void C_Tie(Message _incoming)
    {
        C_ShowTurn(_incoming);

        ct.TieGame();
    }

    public void C_NewGame()
    {
        ct.Restart();
    }

    public Message[] Sv_ClientJoin()
    {
        Message[] msgs = new Message[2];

        msgs[0] = SetCircle();
        msgs[1] = SetCross();

        return msgs;
    }

    public Message Sv_NewGame()
    {
        st.Restart();

        return new Message()
        {
            cmd = CommandType.NewGameAccept,
        };
    }

    private Message SetCircle()
    {
        Message msg = new Message()
        {
            cmd = CommandType.SetType,
            type = "Circle",
        };

        return msg;
    }

    private Message SetCross()
    {
        Message msg = new Message()
        {
            cmd = CommandType.SetType,
            type = "Cross",
        };

        return msg;
    }

    public Message Sv_DoTurn(Message _incoming)
    {
        Vector2Int pos = new Vector2Int(Convert.ToInt32(_incoming.pos.Split(',')[0]), Convert.ToInt32(_incoming.pos.Split(',')[1]));
        ClickType type = (ClickType)Enum.Parse(typeof(ClickType), _incoming.type);

        int finish = st.DoTurn(pos, type);
        Message msg;

        switch (finish)
        {
            default:
            case 0:
                msg = new Message()
                {
                    cmd = CommandType.ShowTurn,
                    type = _incoming.type,
                    pos = _incoming.pos,
                };
                break;
            case 1:
                msg = new Message()
                {
                    cmd = CommandType.WinLose,
                    type = type.ToString(),
                    pos = _incoming.pos,
                };
                break;
            case 2:
                msg = new Message()
                {
                    cmd = CommandType.Tie,
                    type = type.ToString(),
                    pos = _incoming.pos,
                };
                break;
        }

        return msg;
    }
}
