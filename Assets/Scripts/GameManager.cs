using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public Player[] players;
    public Vector3[] playerSpawns;

	void Awake()
    {
        InitializeServices();
    }
    
    // Use this for initialization
	void Start () {
        InitializePlayers();
        Services.NPCManager.GenerateNPCS();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Reset"))
        {
            Reset();
        }
	}

    void InitializeServices()
    {
        Services.GameManager = this;
        Services.EventManager = new EventManager();
        Services.TaskManager = new TaskManager();
        Services.Prefabs = Resources.Load<PrefabDB>("Prefabs/Prefabs");
        Services.NPCManager = GameObject.FindGameObjectWithTag("NPCManager").GetComponent<NPCManager>();
    }

    void InitializePlayers()
    {
        players = new Player[1]
        {
            InitializePlayer(1)
        };
    }

    Player InitializePlayer(int playerNum)
    {
        GameObject playerObj = Instantiate(Services.Prefabs.Player, playerSpawns[playerNum - 1], Quaternion.identity);
        Player player = playerObj.GetComponent<Player>();
        player.playerNum = playerNum;
        return player;
    }

    void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
