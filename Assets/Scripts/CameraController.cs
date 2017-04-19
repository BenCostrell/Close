using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float expectedMaxDistance;
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
        Vector2 playerPos = Services.GameManager.players[0].transform.position;
        float distance = Vector2.Distance(playerPos, transform.position);
        Vector2 targetPos = Vector2.Lerp(transform.position, playerPos, Easing.QuadEaseOut(1 - (distance / expectedMaxDistance)));
        transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);
    }
}
