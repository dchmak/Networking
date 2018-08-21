/*
* Created by Daniel Mak
*/

using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Daniel.Networkings.PVP {
    public class GameManager : PunBehaviour {

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        public override void OnPhotonPlayerConnected(PhotonPlayer other) {
            Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer other) {
            Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects
        }

        public override void OnLeftRoom() {
            SceneManager.LoadScene(0);
        }

        private void Start() {
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
            player.GetComponent<NameDisplay>().SetName(PhotonNetwork.player.NickName);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.L)) PhotonNetwork.LeaveRoom();
        }
    }
}