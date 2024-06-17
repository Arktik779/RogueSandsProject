using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform player;
    public Transform[] destinations;
    public GameObject playerGameObject;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerGameObject.SetActive(false);
            int randomIndex = Random.Range(0, destinations.Length);
            Transform randomDestination = destinations[randomIndex];

            player.position = randomDestination.position;

            playerGameObject.SetActive(true);
        }
    }
}
