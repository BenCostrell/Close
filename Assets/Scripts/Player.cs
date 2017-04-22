using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [HideInInspector]
    public int playerNum;

    public float accel;
    public float maxSpeed;

    public float dragFactor;

    [HideInInspector]
    public Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Move();
    }

    void Move()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal_P" + playerNum), Input.GetAxis("Vertical_P" + playerNum));

        if (inputVector.magnitude > 0.1)
        {
            rb.AddForce(inputVector.normalized * accel);
            SlowDownApproach();
        }

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    void SlowDownApproach()
    {
        NPC npcImMostMovingTowards = Services.NPCManager.NPCThatPlayerIsMostMovingTowards();

        Vector3 differenceVector = npcImMostMovingTowards.transform.position - transform.position;

        Vector2 dragVector = differenceVector.normalized * dragFactor / Mathf.Pow(differenceVector.magnitude, 0.85f);

        Vector2 previousVelocity = rb.velocity;

        rb.velocity -= dragVector;
    }
}
