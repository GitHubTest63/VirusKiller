using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerIOClient;
using System;

public class NetworkManager : MonoBehaviour
{

    //Singleton
    private static NetworkManager instance;
    public static NetworkManager Instance
    {
        get
        {
            return instance;
        }
    }


    void Awake()
    {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(this);
    }

    //PlayerIO stuff
    private Connection connection;
    private List<PlayerIOClient.Message> messages = new List<PlayerIOClient.Message>(); //  Messsage queue implementation
    private bool joinedRoom = false;
    private PlayerIOClient.Client client;
    public bool isConnected { get { return connection != null ? connection.Connected : false; } }
    public string userId = "";

    public bool developmentServer;
    public bool localhost;
    public string ipDevServ = "192.168.1.3";
    private static string gameId = "virus-iaad396bmk2vohrpvimqa";

    public GameObject playerPrefab;
    public string playerName;

    public void authenticate(string id, string mp)
    {
        PlayerIO.QuickConnect.SimpleConnect(
            gameId,
            id,
            mp,
            null,
            delegate(Client client)
            {
                //authenticated
                Debug.Log("Authenticated");
                successfullAuthentication(client, id);
            },
            delegate(PlayerIOError error)
            {
                this.register(id, mp);
            }
        );
    }

    private void register(string id, string mp)
    {
        PlayerIO.QuickConnect.SimpleRegister(
            gameId,
            id,
            mp,
            null,
            null,
            null,
            null,
            null,
            null,
            delegate(Client client)
            {
                //authenticated
                Debug.Log("Registered");
                successfullAuthentication(client, id);
            },
            delegate(PlayerIORegistrationError error)
            {
                Debug.LogError("Authentication failed : " + error.Message);
                GUIManager.Instance.displayErrorMessage(error.Message);
            }
        );
    }

    private void successfullAuthentication(Client client, string playerName)
    {
        Debug.Log("Successfully connected to Player.IO");
        if (developmentServer)
        {
            client.Multiplayer.DevelopmentServer = new ServerEndpoint(System.String.IsNullOrEmpty(ipDevServ) ? "192.168.1.96" : ipDevServ, 8184);
        }
        if (localhost)
        {
            client.Multiplayer.DevelopmentServer = new ServerEndpoint("127.0.0.1", 8184);
        }

        this.playerName = playerName;

        Dictionary<string, string> userData = new Dictionary<string, string>();
        userData.Add("name", playerName);

        this.client = client;

        this.client.Multiplayer.CreateJoinRoom(
            "RoomId", //TODO: ???
            "Lobby",
            false,
            null,
            userData,
            delegate(Connection conn)
            {
                connection = conn;
                connection.OnMessage += handlemessage;
                connection.OnDisconnect += disconnected;
                joinedRoom = true;
                GameManager.Instance.goToLobbyScene();
            },
            delegate(PlayerIOError error)
            {
                Debug.LogError("Error Joining Room: " + error.ToString());
            }
        );
    }

    void joinGameRoom(string roomId, string mapName)
    {
        Dictionary<string, string> userData = new Dictionary<string, string>();
        userData.Add("name", this.playerName);

        client.Multiplayer.CreateJoinRoom(
            roomId,				//Room is the Alliance of the player 
            "Game",							//The room type started on the server
            false,									//Should the room be visible in the lobby?
            null,
            userData,
            delegate(Connection conn)
            {
                Debug.Log("Joined Room : " + roomId);
                // We successfully joined a room so set up the message handler
                connection = conn;
                connection.OnMessage += handlemessage;
                connection.OnDisconnect += disconnected;
                joinedRoom = true;
                GameManager.Instance.goToMainScene();
            },
        delegate(PlayerIOError error)
        {
            Debug.LogError("Error Joining Room: " + error.ToString());
        }
        );
    }

    public void disconnect()
    {
        if (!connection.Connected) return;
        connection.Disconnect();
    }

    public void disconnected(object sender, string error)
    {
        Debug.LogWarning("Disconnected !");
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.A))
            connection.Send("test", "Yo !");
        if (Input.GetKeyDown(KeyCode.C))
            connection.Send("Chat", "Tacos", "Yo!");
        if (Input.GetKeyDown(KeyCode.B))
            this.client.BigDB.LoadRange("Users", "byId", null, 0, int.MaxValue, 1000, delegate(DatabaseObject[] results)
            {
                foreach (DatabaseObject result in results)
                {
                    Debug.Log("get db result id : " + result.GetInt("id"));
                }
            },
            delegate(PlayerIOError error)
            {
                Debug.LogError(error.Message);
            });*/
    }

    void FixedUpdate()
    {
        // process message queue
        foreach (PlayerIOClient.Message m in messages)
        {
            //Debug.Log(Time.time + " - Message received from server " + m.ToString());
            switch (m.Type)
            {
                //game messages
                case "PositionMessage":
                    //Debug.Log("Player : " + m.GetString(0) + " at [" + m.GetFloat(1) + ", " + m.GetFloat(2) + ", " + m.GetFloat(3) + "]");
                    //Debug.Log(m.GetString(0));
                    GameObject entity = EntitiesManager.Instance.getEntity(m.GetString(0));
                    if (entity != null)
                    {
                        entity.transform.position.Set(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
                    }
                    break;
                //Lobby Messages
                case "PlayerJoinedLobby":
                    ConnectedPlayersManager.Instance.addConnectedPlayer(m.GetString(0));
                    break;
                case "PlayerLeftLobby":
                    ConnectedPlayersManager.Instance.removeConnectedPlayer(m.GetString(0));
                    break;
                case "SelectMap":
                    MapManager.Instance.addPlayer(m.GetString(0), m.GetString(1));
                    break;
                case "LaunchGame":
                    this.joinGameRoom(m.GetString(0), m.GetString(1));
                    break;
                case "InvitationToJoinMap":
                    GUIManager_Lobby.Instance.showInvitationPanel(m.GetString(0), m.GetString(1));
                    break;
                case "PlayerJoinedGame":
                    Debug.Log(m.GetString(1) + "joined the party at x:" + m.GetFloat(2) + ", y:" + m.GetFloat(3) + ", z:" + m.GetFloat(4));
                    //this.player = GameObject.FindGameObjectWithTag("Player");
                    //this.player.transform.position.Set(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
                    //keep track of players to make updates
                    EntitiesManager.Instance.addEntity(m.GetString(1), EntityFactory.Instance.createPlayer(m.GetString(1)));
                    break;
                case "ChatLobby":
                    ChatManager.Instance.addChatMessage(m.GetString(0), m.GetString(1));
                    break;
                case "test":
                    Debug.Log("Server answers : " + m.GetString(0));
                    break;
            }
        }

        // clear message queue after it's been processed
        messages.Clear();
    }

    void handlemessage(object sender, PlayerIOClient.Message m)
    {
        messages.Add(m);
    }

    /*void OnJoinedRoom()
    {
        // Spawn player
        GameObject player = PhotonNetwork.Instantiate("Prefabs/" + playerPrefab.name, Vector3.up * 5, Quaternion.identity, 0);

        //init miniMap
        GameObject miniMapCamera = GameObject.Find("MiniMapCamera");
        MiniMapFollow follow = miniMapCamera.GetComponent<MiniMapFollow>();
        follow.target = player;

        //attach main camera
        CameraFollow camFollow = Camera.main.GetComponent<CameraFollow>();
        camFollow.setTarget(player.transform);
    }*/

    /*void OnLevelWasLoaded(int level)
    {
        if (Application.loadedLevelName.Equals("Game"))
        {

        }
    }*/

    //METHODS SENT TO SERVER
    public void sendStart()
    {
        Debug.Log("Sending Start to Server");
        connection.Send("start");
    }

    public void sendChat(string text)
    {
        connection.Send("Chat", text);
    }

    public void sendSelectedMap(string mapName)
    {
        connection.Send("SelectMap", mapName);
    }

    public void sendPlayMap(string mapName)
    {
        connection.Send("PlayMap", mapName);
    }

    public void sendAcceptInvitation()
    {
        connection.Send("AcceptInvitation");
    }

    public void sendDeclineInvitation()
    {
        connection.Send("DeclineInvitation");
    }

    /*public void send(string msgType, params object[] values)
    {
        object[] parameters = new object[values.Length + 1];
        parameters[0] = "Tacos";
        for (int i = 1; i < parameters.Length; i++)
        {
            parameters[i] = values[i - 1];
        }
        this.connection.Send(msgType, values);
    }*/
}
