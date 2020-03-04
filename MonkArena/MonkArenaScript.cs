﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using static MonkArena.Network;

namespace MonkArena {
    public class MonkArenaScript : MonoBehaviour {
        public void Update() {
            if (Input.GetKeyUp(KeyCode.S)) {
                Network.SetupServer();
                Network.Server.MessageReceivedEvent += Server_MessageReceivedEvent;
            }

            if (Input.GetKeyUp(KeyCode.C)) {
                Network.SetupClient("127.0.0.1");
                Network.Client.MessageReceivedEvent += Client_MessageReceivedEvent;
            }

            if (Network.Connected || Network.IsServer) {
                if (Input.GetKeyDown(KeyCode.Space))
                    Network.SendString("test");
            }
        }

        private void Server_MessageReceivedEvent(Received data) {
            Network.Server.StartReceive();
            RWConsole.LogInfo($"{data.Sender}: {data.Message}");

            Message m = new Message(data.Message);
            Network.Server.Reply(new Message("received", "", m.Token), data.Sender);

            if (!ConnectedClients.ContainsKey(data.Sender)) ConnectedClients[data.Sender] = new PlayerInfo();

            Message receivedMessage = new Message(data.Message);

            switch (receivedMessage.Type) {
                case "player_animation":
                    if (int.TryParse(receivedMessage.Contents, out int result))
                        ConnectedClients[data.Sender].Animation = (Player.AnimationIndex)result;
                    else RWConsole.LogError($"Bad animation string: {receivedMessage.Contents}");
                    break;
                case "player_position":
                    string[] pos = receivedMessage.Contents.Split(',');
                    if (float.TryParse(pos[0], out float x) && float.TryParse(pos[1], out float y))
                        ConnectedClients[data.Sender].Creature.bodyChunks[0].pos = new UnityEngine.Vector2(x, y);
                    else RWConsole.LogError($"Bad position string: {receivedMessage.Contents}");
                    break;

                case "received":
                    UnreceivedMessages.Remove(receivedMessage.Contents);
                    break;
                case "handshake":
                    SendMessageTo(new Message("handshake_approved", "", ""), data.Sender);
                    break;

                default:
                    RWConsole.LogError($"Unable to handle message of type: {receivedMessage.Type} with contents: {receivedMessage.Contents}");
                    break;
            }

        }

        private void Client_MessageReceivedEvent(Received data) {
            if (!Network.IsServer) RWConsole.LogInfo($"{data.Sender}: {data.Message}");

            RWConsole.LogInfo($"{Network.UnreceivedMessages.Count} messages not received by server.");
            Message m = new Message(data.Message);
            if (m.Type == "received")
                Network.UnreceivedMessages.Remove(m.Contents);
            Network.Client.StartReceive();
        }
    }
}
