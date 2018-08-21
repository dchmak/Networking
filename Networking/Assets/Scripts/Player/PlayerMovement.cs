/*
* Created by Daniel Mak
*/

using Photon;
using UnityEngine;
using Daniel.Character2D;

[RequireComponent(typeof(MovementController), typeof(Animator))]
public class PlayerMovement : PunBehaviour {

    public MovementController controller;
    
    private float horizontalInput;
    private bool jumpInput;    

    private void Update () {
        if (photonView.isMine == false && PhotonNetwork.connected == true) {
            return;
        }

        horizontalInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetButton("Jump");

        //if (Input.GetButtonDown("Jump")) jumpInput = true;
    }

    private void FixedUpdate() {
        controller.Move(horizontalInput, false, jumpInput);
        //jumpInput = false;
    }
}