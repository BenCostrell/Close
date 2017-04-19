using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float speed;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        FollowPlayer();
	}

    void FollowPlayer()
    {
        Vector3 playerPos = Services.GameManager.players[0].transform.position;
        transform.position = new Vector3(playerPos.x, playerPos.y, transform.position.z);
    }
}
