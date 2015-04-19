using UnityEngine;
using System.Collections;
using PlayerIOClient;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{

    //Singleton
    static NetworkManager instance;
    static public NetworkManager Instance { get { return instance; } }
    void Awake()
    {
        if (instance == null) instance = this;
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

    private GameObject player;
    public string playerName;

    //Here it begins 
    public void authenticate(string id, string mp, bool register = false)
    {
        Dictionary<string, string> authData = new Dictionary<string, string>();
        if (register)
        {
            authData.Add("register", "true");
        }
        authData.Add("username", id);
        authData.Add("password", mp); //TODO: encryption MD5?

        PlayerIOClient.PlayerIO.Authenticate(
            "virus-iaad396bmk2vohrpvimqa",
            "public",
            authData,
            null,
            delegate(Client client)
            {
                //authenticated
                Debug.Log("Authenticated");
                successfullAuthentication(client);
            },
            delegate(PlayerIOError error)
            {
                //authentication failed
                //TODO: handlemessage authentication errors
                if (register)
                {
                    Debug.LogError("Authentication failed : " + error.Message);
                    GUIManager.Instance.displayErrorMessage(error.Message);
                }
                else
                {
                    this.authenticate(id, mp, true);
                }
            }
        );
    }

    private void successfullAuthentication(Client client)
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

        Dictionary<string, string> userData = new Dictionary<string, string>();

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

    /*public void startConnection()
    {
        string playerId = SystemInfo.deviceUniqueIdentifier;

        //user is just using this device with no account
        Debug.Log("Annonymous connect : " + playerId);
        userId = playerId;
        PlayerIOClient.PlayerIO.Connect(
            "virus-iaad396bmk2vohrpvimqa",	// Game id 
            "public",							// The id of the connection, as given in the settings section of the admin panel. By default, a connection with id='public' is created on all games.
            playerId,							// The id of the user connecting. 
            null,								// If the connection identified by the connection id only accepts authenticated requests, the auth value generated based on UserId is added here
            null,
            null,
            delegate(Client client)
            {
                successfullConnect(client);
            },
            delegate(PlayerIOError error)
            {
                Debug.Log("Error connecting: " + error.ToString());
            }
        );
    }*/

    void joinGameRoom(string roomId, string mapName)
    {
        Dictionary<string, string> userData = new Dictionary<string, string>();
        userData.Add("name", "Tacos");

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

    // Use this for initialization
    void Start()
    {

    }

    void OnLevelWasLoaded(int level)
    {
        if (Application.loadedLevelName.Equals("Game"))
        {

        }
    }

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

    public void send(string msgType, params object[] values)
    {
        object[] parameters = new object[values.Length + 1];
        parameters[0] = "Tacos";
        for (int i = 1; i < parameters.Length; i++)
        {
            parameters[i] = values[i - 1];
        }
        this.connection.Send(msgType, values);
    }
}
