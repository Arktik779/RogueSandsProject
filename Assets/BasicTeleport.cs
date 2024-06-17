using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTeleport : MonoBehaviour
{
    public Transform player, destination;
    public GameObject playerPlayer;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerPlayer.SetActive(false);
            player.position = destination.position;
            playerPlayer.SetActive(true);
        }
    }
}