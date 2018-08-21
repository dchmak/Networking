/*
* Created by Daniel Mak
*/

using Photon;
using TMPro;

namespace Daniel.Networkings {
    public class Launcher : PunBehaviour {

        public byte maxPlayer = 4;
        public TextMeshProUGUI log;

        private static readonly string gameVersion = "1";
        private static readonly string roomName = "room";

        private bool isConnecting;

        public override void OnConnectedToMaster() {
            print("OnConnectedToMaster has been called.");
            if (isConnecting) {
                JoinRoom();
            }
        }

        public override void OnDisconnectedFromPhoton() {
            print("OnDisconnectedFromPhoton has been called.");
        }

        public override void OnJoinedRoom() {
            print("OnJoinedRoom has been called.");
            PhotonNetwork.LoadLevel(1);
        }

        public void Connect() {
            isConnecting = true;

            if (PhotonNetwork.connected) {
                JoinRoom();
            } else {
                log.text += "Connecting...\n";
                PhotonNetwork.ConnectUsingSettings(gameVersion);
            }
        }

        private void JoinRoom() {
            log.text += "Joining/Creating room...\n";
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = maxPlayer }, null);
        }

        private void Awake() {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = true;

            log.text = "";
        }


    }
}