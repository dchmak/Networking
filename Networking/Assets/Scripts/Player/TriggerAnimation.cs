/*
* Created by Daniel Mak
*/

using UnityEngine;

public class TriggerAnimation : MonoBehaviour {

    private Animator animator;

    public void Trigger(string name) {
        animator.SetTrigger(name);
    }

    private void Start() {
        animator = GetComponent<Animator>();
    }
}