using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour {

    public int numNpcs;
    public float minAcceptableDistance;
    public int maxNumTries;
    public float creationRadius;

    private int lastId;

    private List<NPC> npcs;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        UpdateNPCsToBeInRange();
	}

    public void UpdateNPCsToBeInRange()
    {
        Vector3 playerPos = Services.GameManager.players[0].transform.position;
        foreach (NPC npc in npcs)
        {
            if(Vector3.Distance(npc.transform.position, playerPos) > creationRadius)
            {
                npc.transform.position = GenerateValidLocation(playerPos, creationRadius/2);
                npc.SetHappyParticleStatus(false);
            }
        }
    }

    public void GenerateNPCS()
    {
        lastId = 0;
        npcs = new List<NPC>();
        for (int i = 0; i < numNpcs; i++)
        {
            NPC npc = GenerateNPC(Vector3.zero, 0);
            if (npc.transform.position == Vector3.forward)
            {
                Destroy(npc.gameObject);
                Debug.Log("only created " + i + "npcs");
                break;
            }
            else npcs.Add(npc);
        }
    }

    NPC GenerateNPC(Vector3 centerPosition, float minDist)
    {
        Vector3 location = GenerateValidLocation(centerPosition, minDist);
        return CreateNPC(location, minDist);
    }

    NPC CreateNPC(Vector3 location, float minDist)
    {
        GameObject npcObj = Instantiate(Services.Prefabs.NPC, location, Quaternion.identity) as GameObject;
        NPC npc = npcObj.GetComponent<NPC>();
        npc.id = lastId + 1;
        return npc;
    }

    Vector3 GenerateValidLocation(Vector3 centerPosition, float minDist)
    {
        Vector3 location = Vector3.forward;
        bool isValid = false;
        for (int i = 0; i < maxNumTries; i++)
        {
            location = GenerateRandomLocation(centerPosition, minDist);
            isValid = IsLocationValid(location);
            if (isValid) break;
        }
        if (!isValid) return Vector3.forward;
        else return location;
    }

    bool IsLocationValid(Vector3 location)
    {
        bool isValid = true;
        foreach (NPC npc in npcs)
        {
            if (Vector3.Distance(location, npc.transform.position) < minAcceptableDistance)
            {
                isValid = false;
                break;
            }
        }
        foreach(Player player in Services.GameManager.players)
        {
            if (Vector3.Distance(location, player.transform.position) < minAcceptableDistance)
            {
                isValid = false;
                break;
            }
        }
        return isValid;
    }

    Vector3 GenerateRandomLocation(Vector3 centerPosition, float minDist)
    {
        float distanceFromCenter = Random.Range(minDist, creationRadius);
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;

        float xCoord = distanceFromCenter * Mathf.Cos(angle);
        float yCoord = distanceFromCenter * Mathf.Sin(angle);
        return new Vector3(xCoord, yCoord, 0) + centerPosition;
    }

    public NPC NPCThatPlayerIsMostMovingTowards()
    {
        NPC targetNPC = npcs[0];
        float trajectoryTimeMeasure;
        float currentClosestTrajectory = Mathf.Infinity;
        Vector3 posDifferenceVector;
        float dotProduct;
        Vector3 playerMovementVector = Services.GameManager.players[0].rb.velocity;
        Vector3 playerPos = Services.GameManager.players[0].transform.position;


        foreach(NPC npc in npcs)
        {
            posDifferenceVector = npc.transform.position - playerPos;
            dotProduct = Vector3.Dot(playerMovementVector, posDifferenceVector);
            trajectoryTimeMeasure = posDifferenceVector.magnitude / (dotProduct/posDifferenceVector.magnitude);
            if (trajectoryTimeMeasure > 0 && trajectoryTimeMeasure < currentClosestTrajectory)
            {
                currentClosestTrajectory = trajectoryTimeMeasure;
                targetNPC = npc;
            }
        }

        return targetNPC;
    }
}
