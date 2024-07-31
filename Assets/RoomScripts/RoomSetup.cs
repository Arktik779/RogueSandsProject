using UnityEngine;
using System.Collections.Generic;
using EK;

public class RoomSetup : MonoBehaviour
{
    public RoomManager roomManager; // Assign in the Inspector
    public List<string> portalTags; // Set these in the Inspector

    private void Start()
    {
        if (roomManager == null)
        {
            roomManager = FindObjectOfType<RoomManager>();
        }

        if (roomManager != null)
        {
            List<GameObject> portals = new List<GameObject>();
            foreach (string tag in portalTags)
            {
                GameObject[] taggedPortals = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject portal in taggedPortals)
                {
                    if (!portals.Contains(portal))
                    {
                        portals.Add(portal);
                    }
                }
            }
            roomManager.AssignPortals(portals);
        }
    }
}