using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using PlayerIO.GameLibrary;
using System.Drawing;

namespace MushroomsUnity3DExample
{
    public class Player : BasePlayer
    {
        public float posx = 0;
        public float posz = 0;
    }

    [RoomType("LobbyRoom")]
    public class Lobby : Game<Player>
    {
        public override void UserJoined(Player player)
        {
            Console.WriteLine("Player " + player.ConnectUserId + " joined lobby");
        }
        public override void GotMessage(Player player, Message message)
        {
            Console.WriteLine("Receive " + message.GetType() + " Message from client " + player.ConnectUserId);
            switch (message.Type)
            {
                case "test":
                    player.Send("test", "TG !");
                    break;
            }
        }
    }

    [RoomType("GameRoom")]
    public class GameCode : Game<Player>
    {
        // This method is called when an instance of your the game is created
        public override void GameStarted()
        {
            // anything you write to the Console will show up in the 
            // output window of the development server
            Console.WriteLine("Game is started: " + RoomId);

            // reset game every 2 minutes
            AddTimer(resetgame, 120000);


        }

        private void resetgame()
        {
            //// scoring system
            //Player winner = new Player();
            //int maxscore = -1;
            //foreach (Player pl in Players)
            //{
            //    if (pl.toadspicked > maxscore)
            //    {
            //        winner = pl;
            //        maxscore = pl.toadspicked;
            //    }
            //}

            //// broadcast who won the round
            //if (winner.toadspicked > 0)
            //{
            //    Broadcast("Chat", "Server", winner.ConnectUserId + " picked " + winner.toadspicked + " Toadstools and won this round.");
            //}
            //else
            //{
            //    Broadcast("Chat", "Server", "No one won this round.");
            //}

            //// reset everyone's score
            //foreach (Player pl in Players)
            //{
            //    pl.toadspicked = 0;
            //}
            //Broadcast("ToadCount", 0);
        }

        // This method is called when the last player leaves the room, and it's closed down.
        public override void GameClosed()
        {
            Console.WriteLine("RoomId: " + RoomId);
        }

        // This method is called whenever a player joins the game
        public override void UserJoined(Player player)
        {
            foreach (Player pl in Players)
            {
                if (pl.ConnectUserId != player.ConnectUserId)
                {
                    pl.Send("PlayerJoined", player.ConnectUserId, 0, 0);
                    player.Send("PlayerJoined", pl.ConnectUserId, pl.posx, pl.posz);
                }
            }
        }

        // This method is called when a player leaves the game
        public override void UserLeft(Player player)
        {
            Broadcast("PlayerLeft", player.ConnectUserId);
        }

        // This method is called when a player sends a message into the server code
        public override void GotMessage(Player player, Message message)
        {
            Console.WriteLine("Receive " + message.GetType() + " Message from client " + player.ConnectUserId);
            switch (message.Type)
            {
                // called when a player clicks on the ground
                case "Move":
                    player.posx = message.GetFloat(0);
                    player.posz = message.GetFloat(1);
                    Broadcast("Move", player.ConnectUserId, player.posx, player.posz);
                    break;
                case "MoveHarvest":
                    // called when a player clicks on a harvesting node
                    // sends back a harvesting command to the player, a move command to everyone else
                    player.posx = message.GetFloat(0);
                    player.posz = message.GetFloat(1);
                    foreach (Player pl in Players)
                    {
                        if (pl.ConnectUserId != player.ConnectUserId)
                        {
                            pl.Send("Move", player.ConnectUserId, player.posx, player.posz);
                        }
                    }
                    player.Send("Harvest", player.ConnectUserId, player.posx, player.posz);
                    break;
                case "Chat":
                    foreach (Player pl in Players)
                    {
                        if (pl.ConnectUserId != player.ConnectUserId)
                        {
                            pl.Send("Chat", player.ConnectUserId, message.GetString(0));
                        }
                    }
                    break;
            }
        }
    }
}