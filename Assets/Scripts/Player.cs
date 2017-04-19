using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [HideInInspector]
    public int playerNum;

    public float accel;
    public float maxSpeed;

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
        Move();
	}

    void Move()
    {
        float xInput = Input.GetAxis("Horizontal_P" + playerNum);
        float yInput = Input.GetAxis("Vertical_P" + playerNum);

        rb.AddForce(new Vector2(xInput, yInput) * accel);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}
