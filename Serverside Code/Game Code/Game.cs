using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using PlayerIO.GameLibrary;
using System.Drawing;

namespace MushroomsUnity3DExample
{
    /*public class AuthenticatedPlayer : BasePlayer
    {
        public string username;
        public string password;
    }*/

    public class ConnectedPlayer : BasePlayer
    {
        public string name = "not initialized";
        public List<ConnectedPlayer> group = new List<ConnectedPlayer>();
        public bool isReady = false;
        public string pendingMap = null;
    }

    public class GamePlayer : BasePlayer
    {
        public string name = "not initialized";
        public int lvl = 1;
        public float posX = 0.0f;
        public float posY = 0.0f;
        public float posZ = 0.0f;

        public float velocityX = 0.0f;
        public float velocityY = 0.0f;
        public float velocityZ = 0.0f;
    }

    /*[RoomType("Authentication")]
    public class Authentication : Game<AuthenticatedPlayer>
    {
        private DBManager DBManager = new DBManager();

        public override void GotMessage(AuthenticatedPlayer player, Message message)
        {
            base.GotMessage(player, message);
            switch (message.Type)
            {
                case "Authenticate":
                    string username = message.GetString(0);
                    PlayerIO.BigDB.Load("Users", )
                    DatabaseObject obj = new DatabaseObject();
                    obj.Set("id", new Random().Next());
                    PlayerIO.BigDB.CreateObject("Users", null, obj, delegate(DatabaseObject dbo)
                    {
                        Console.WriteLine("create user");
                    }, delegate(PlayerIOError error)
                    {
                        Console.WriteLine(error.Message);
                    });
                    break;
            }
        }
    }*/

    [RoomType("Lobby")]
    public class Lobby : Game<ConnectedPlayer>
    {
        private Dictionary<string, List<ConnectedPlayer>> maps = new Dictionary<string, List<ConnectedPlayer>>();
        private Dictionary<string, List<ConnectedPlayer>> pending = new Dictionary<string, List<ConnectedPlayer>>();

        public override void UserJoined(ConnectedPlayer player)
        {
            string name = "unknown";
            player.JoinData.TryGetValue("name", out name);
            Console.WriteLine("Player " + name + " joined lobby");
            player.name = name;
            foreach (ConnectedPlayer p in Players)
            {
                if (p.ConnectUserId != player.ConnectUserId)
                {
                    p.Send("PlayerJoinedLobby", player.name);
                    player.Send("PlayerJoinedLobby", p.name);
                }
            }
            player.Send("PlayerJoinedLobby", player.name);
        }

        public override void UserLeft(ConnectedPlayer player)
        {
            Console.WriteLine("Player " + player.name + " left lobby");
            Broadcast("PlayerLeftLobby", player.name);
        }

        public override void GotMessage(ConnectedPlayer player, Message message)
        {
            Console.WriteLine("Receive " + message.GetType() + " Message from client " + player.ConnectUserId);
            switch (message.Type)
            {
                case "Chat":
                    Broadcast("ChatLobby", player.name, message.GetString(0));
                    break;
                case "SelectMap":
                    {
                        List<ConnectedPlayer> players;
                        if (this.maps.TryGetValue(message.GetString(0), out players))
                        {
                            if (players.Contains(player))
                            {
                                players.Remove(player);
                            }
                            else
                            {
                                players.Add(player);
                            }
                        }
                        else
                        {
                            players = new List<ConnectedPlayer>();
                            players.Add(player);
                            this.maps.Add(message.GetString(0), players);
                        }
                        Broadcast("SelectMap", message.GetString(0), player.name);
                    }
                    break;
                case "PlayMap":
                    {
                        foreach (KeyValuePair<string, List<ConnectedPlayer>> entry in this.maps)
                        {
                            List<ConnectedPlayer> players = entry.Value;
                            if (players.Contains(player))
                            {
                                List<ConnectedPlayer> group;
                                if (!this.pending.TryGetValue(entry.Key, out group))
                                {
                                    group = new List<ConnectedPlayer>();
                                    this.pending.Add(entry.Key, group);
                                }
                                foreach (ConnectedPlayer p in players)
                                {
                                    group.Add(p);
                                    if (p != player)
                                        p.Send("InvitationToJoinMap", player.name, entry.Key);
                                }
                                return;
                            }
                        }
                        this.createGame(player, message.GetString(0));
                    }
                    break;
                case "AcceptInvitation":
                    {
                        player.isReady = true;
                        List<ConnectedPlayer> players = null;
                        string mapName = null;
                        foreach (KeyValuePair<string, List<ConnectedPlayer>> entry in this.pending)
                        {
                            mapName = entry.Key;
                            players = entry.Value;
                            if (players.Contains(player))
                            {
                                foreach (ConnectedPlayer p in players)
                                {
                                    if (!p.isReady)
                                    {
                                        return;
                                    }
                                }
                                break;
                            }
                        }
                        foreach (ConnectedPlayer p in players)
                        {
                            p.group.AddRange(players);
                        }
                        this.createGame(player, player.pendingMap);
                    }
                    break;
                case "DeclineInvitation":
                    foreach (KeyValuePair<string, List<ConnectedPlayer>> entry in this.pending)
                    {
                        List<ConnectedPlayer> players = entry.Value;
                        if (players.Contains(player))
                        {
                            players.Remove(player);
                            Broadcast("SelectMap", entry.Key, player.name);
                        }
                    }
                    break;
                /*case "test":
                    player.Send("test", "TG !");
                    DatabaseObject obj = new DatabaseObject();
                    obj.Set("id", new Random().Next());
                    PlayerIO.BigDB.CreateObject("Users", null, obj, delegate(DatabaseObject dbo)
                    {
                        Console.WriteLine("create user");
                    }, delegate(PlayerIOError error)
                    {
                        Console.WriteLine(error.Message);
                    });
                    break;*/
            }
        }

        private void createGame(ConnectedPlayer player, string mapName)
        {
            string roomId = Guid.NewGuid().ToString("n");
            if (player.group.Count == 0)
            {
                player.Send("LaunchGame", roomId, mapName);
            }
            else
            {
                foreach (ConnectedPlayer p in player.group)
                {
                    p.Send("LaunchGame", roomId, mapName);
                }
            }
        }
    }

    [RoomType("Game")]
    public class GameCode : Game<GamePlayer>
    {
        // This method is called when an instance of your the game is created
        public override void GameStarted()
        {
            Console.WriteLine("Game is started : " + RoomId);

            this.AddTimer(this.syncPositions, 50);
        }

        private void syncPositions()
        {
            Console.WriteLine("sync positions");
            foreach (GamePlayer pl in Players)
            {
                Broadcast("PositionMessage", pl.name, pl.posX, pl.posY, pl.posZ);
            }
        }

        // This method is called when the last player leaves the room, and it's closed down.
        public override void GameClosed()
        {
            Console.WriteLine("Close room : " + RoomId);
        }

        public override void UserJoined(GamePlayer player)
        {
            string name = "unknown";
            if (player.JoinData.TryGetValue("name", out name))
            {
                Console.WriteLine(name + " joined the room");
                player.name = name;
                foreach (GamePlayer p in Players)
                {
                    if (p.ConnectUserId != player.ConnectUserId)
                    {
                        p.Send("PlayerJoinedGame", player.ConnectUserId, player.name, player.posX, player.posY, player.posZ);
                        player.Send("PlayerJoinedGame", p.ConnectUserId, p.name, p.posX, p.posY, p.posZ);
                    }
                }
                //player.Send("PlayerJoinedGame", player.ConnectUserId, player.name, player.posX, player.posY, player.posZ);
            }
            else
            {
                player.Disconnect();
            }
        }

        // This method is called when a player leaves the game
        public override void UserLeft(GamePlayer player)
        {
            Broadcast("PlayerLeft", player.ConnectUserId);
        }

        // This method is called when a player sends a message into the server code
        public override void GotMessage(GamePlayer player, Message message)
        {
            Console.WriteLine("Receive " + message.GetType() + " Message from client " + player.ConnectUserId);
            switch (message.Type)
            {
                case "Chat":
                    foreach (GamePlayer pl in Players)
                    {
                        if (pl.ConnectUserId != player.ConnectUserId)
                        {
                            pl.Send("Chat", player.ConnectUserId, message.GetString(0));
                        }
                    }
                    break;
                case "PositionMessage":
                    player.posX = message.GetFloat(0);
                    player.posY = message.GetFloat(1);
                    player.posZ = message.GetFloat(2);
                    break;
                case "Shoot":
                    Broadcast("Shoot", player.name);
                    break;
            }
        }

    }
}