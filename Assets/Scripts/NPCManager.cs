using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour {

    public int numNpcs;
    public float minAcceptableDistance;
    public int maxNumTries;
    public Vector2 topRightBounds;
    public Vector2 bottomLeftBounds;

    private List<NPC> npcs;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GenerateNPCS()
    {
        npcs = new List<NPC>();
        for (int i = 0; i < numNpcs; i++)
        {
            NPC npc = GenerateNPC();
            if (npc.transform.position == Vector3.forward)
            {
                Destroy(npc.gameObject);
                Debug.Log("only created " + i + "npcs");
                break;
            }
            else npcs.Add(npc);
        }
    }

    NPC GenerateNPC()
    {
        Vector3 location = GenerateValidLocation();
        return CreateNPC(location);
    }

    NPC CreateNPC(Vector3 location)
    {
        GameObject npcObj = Instantiate(Services.Prefabs.NPC, location, Quaternion.identity) as GameObject;
        NPC npc = npcObj.GetComponent<NPC>();
        return npc;
    }

    Vector3 GenerateValidLocation()
    {
        Vector3 location = Vector3.forward;
        bool isValid = false;
        for (int i = 0; i < maxNumTries; i++)
        {
            location = GenerateRandomLocation();
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

    Vector3 GenerateRandomLocation()
    {
        float xCoord = Random.Range(bottomLeftBounds.x, topRightBounds.x);
        float yCoord = Random.Range(bottomLeftBounds.y, topRightBounds.y);
        return new Vector3(xCoord, yCoord, 0);
    }


}
