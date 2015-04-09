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

    //Here it begins 
    public void startConnection()
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
    }

    void successfullConnect(Client client)
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

        //Create or join the room	
        string roomId = "RoomId";
        if (string.IsNullOrEmpty(roomId))
        {
            roomId = userId;
        }

        client.Multiplayer.CreateJoinRoom(
            roomId,	                            //Room is the Alliance of the player 
            "LobbyRoom",							//The room type started on the server
            false,								//Should the room be visible in the lobby?
            null,
            null,
            delegate(Connection conn)
            {
                Debug.Log("Joined Room : " + roomId);
                // We successfully joined a room so set up the message handler
                connection = conn;
                connection.OnMessage += handlemessage;
                connection.OnDisconnect += disconnected;
                joinedRoom = true;
            },
        delegate(PlayerIOError error)
        {
            Debug.LogError("Error Joining Room: " + error.ToString());
        }
        );

        this.client = client;
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
        if (Input.GetKeyDown(KeyCode.A))
            connection.Send("test", "Yo !");
    }

    void FixedUpdate()
    {
        // process message queue
        foreach (PlayerIOClient.Message m in messages)
        {
            Debug.Log(Time.time + " - Message received from server " + m.ToString());
            switch (m.Type)
            {
                //Basic connection/deconnection

                //Lobby Messages
                case "PlayerJoined":
                    Debug.Log("PlayerJoined : " + m.GetString(0));
                    break;
                case "PlayerLeft":
                    Debug.Log("PlayerLeft : " + m.GetString(0));
                    break;
                case "gameStarted":

                    break;
                case "Chat":
                    Debug.Log(m.GetString(0) + ":" + m.GetString(1));
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

    void joinGameRoom(string roomId)
    {
        client.Multiplayer.CreateJoinRoom(
            roomId,				//Room is the Alliance of the player 
            "GameRoom",							//The room type started on the server
            false,									//Should the room be visible in the lobby?
            null,
            null,
            delegate(Connection conn)
            {
                Debug.Log("Joined Room : " + roomId);
                // We successfully joined a room so set up the message handler
                connection = conn;
                connection.OnMessage += handlemessage;
                connection.OnDisconnect += disconnected;
                joinedRoom = true;

            },
        delegate(PlayerIOError error)
        {
            Debug.LogError("Error Joining Room: " + error.ToString());
        }
        );
    }

    // Use this for initialization
    void Start()
    {
        startConnection();
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
}
