/*
* Created by Daniel Mak
*/

using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CoinFlipCore : PunBehaviour {

    public Image coin;
    public Sprite coinHead;
    public Sprite coinTail;
    public TextMeshProUGUI currentState;
    public TextMeshProUGUI playerCount;

    public override void OnLeftRoom() {
        SceneManager.LoadScene(0);
    }

    public void StartFliping() {
        Animator animator = coin.GetComponent<Animator>();
        if (animator != null) animator.SetTrigger("Flip");

        Flip();
    }
    
    private void Flip() {
        if (Random.Range(0, 2) == 0) {
            coin.sprite = coinHead;
            currentState.text = "Head";
        } else {
            coin.sprite = coinTail;
            currentState.text = "Tail";
        }
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    private void Update() {
        playerCount.text = PhotonNetwork.playerList.Length.ToString();

        if (Input.GetKeyDown(KeyCode.L)) {
            LeaveRoom();
        }
    }
}