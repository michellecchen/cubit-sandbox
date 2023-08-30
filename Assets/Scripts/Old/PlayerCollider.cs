using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{

    public bool isColliding = false;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Wall")) {
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Wall")) {
            isColliding = false;
        }
    }
}
