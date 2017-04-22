using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {

    private Tree<NPC> behaviorTree;
    private Rigidbody2D rb;
    public float accel;
    public float maxSpeed;
    private List<NPC> nearbyNPCs;
    public float noiseFactor;
    public int id;
    public float minDistanceAtRuntime;
    private ParticleSystem ps;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        nearbyNPCs = new List<NPC>();
        ps = GetComponentInChildren<ParticleSystem>();
    }
    
    // Use this for initialization
	void Start () {
        InitializeBehaviorTree();
	}
	
	// Update is called once per frame
	void Update () {
        behaviorTree.Update(this);	
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        GameObject obj = col.gameObject;
        if (obj.tag == "NPC")
        {
            NPC npc = obj.GetComponent<NPC>();
            if (!nearbyNPCs.Contains(npc))
            {
                nearbyNPCs.Add(npc);
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        GameObject obj = col.gameObject;
        if (obj.tag == "NPC")
        {
            NPC npc = obj.GetComponent<NPC>();
            if (nearbyNPCs.Contains(npc))
            {
                nearbyNPCs.Remove(npc);
            }
        }
    }

    NPC GetClosestNPC(bool minDist)
    {
        NPC closestNPC = null;
        float shortestDistance = Mathf.Infinity;

        foreach (NPC npc in nearbyNPCs)
        {
            float distance = Vector3.Distance(npc.transform.position, transform.position);

            if (distance < shortestDistance)
            {
                if ((minDist && distance > minDistanceAtRuntime) || !minDist)
                {
                    shortestDistance = distance;
                    closestNPC = npc;
                }
            }
        }

        return closestNPC;
    }

    void ApproachLocation(Vector3 location)
    {
        Vector3 direction = location - transform.position;

        rb.AddForce(direction.normalized * accel);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    void AvoidLocation(Vector3 location)
    {
        Vector3 direction = transform.position - location;

        rb.AddForce(direction.normalized * accel);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    public void SetHappyParticleStatus(bool active)
    {
        if (active && !ps.isPlaying)
        {
            ps.Play();
        }
        else if (!active && ps.isPlaying)
        {
            ps.Stop();
        }
    }

    void InitializeBehaviorTree()
    {
        behaviorTree = new Tree<NPC>
        (
            new Selector<NPC>(
            new Sequence<NPC>(
                new AreNPCsNearby(),
                new Selector<NPC>(
                    new Sequence<NPC>(
                        new AnyNPCsTooClose(),
                        new SpaceOutFromClosest(),
                        new BeHappy()
                    ),
                    new Sequence<NPC>(
                        new AreApproachableNPCsNearby(),
                        new MoveToClosestNPC()
                    )
                )
            ),
            new Sequence<NPC>(
                new BeSad(),
                new Wander()
            )
            )
        );
    }

    /// ACTIONS


    private class MoveToClosestNPC : Node<NPC>
    {
        public override bool Update(NPC context)
        {
            if (context.GetClosestNPC(true) != null)
            {
                context.ApproachLocation(context.GetClosestNPC(true).transform.position);
                return true;
            }
            return false;
        }
    }

    private class SpaceOutFromClosest : Node<NPC>
    {
        public override bool Update(NPC context)
        {
            context.AvoidLocation(context.GetClosestNPC(false).transform.position);
            return true;
        }
    }

    private class Wander : Node<NPC>
    {
        public override bool Update(NPC context)
        {
            float xDir = Mathf.PerlinNoise(100 * context.id + Time.time, 100 * context.id + Time.time);
            float yDir = Mathf.PerlinNoise(1000 * context.id + Time.time, 100 * context.id + Time.time);
            context.rb.AddForce(context.accel * new Vector2(xDir, yDir).normalized);

            return true;
        }
    }

    private class BeHappy : Node<NPC>
    {
        public override bool Update(NPC context)
        {
            context.SetHappyParticleStatus(true);
            return true;
        }
    }

    private class BeSad : Node<NPC>
    {
        public override bool Update(NPC context)
        {
            context.SetHappyParticleStatus(false);
            return true;
        }
    }


    /// CONDITIONS


    private class AnyNPCsTooClose : Node<NPC>
    {
        public override bool Update(NPC context)
        {
            foreach (NPC npc in context.nearbyNPCs)
            {
                if ((npc.transform.position - context.transform.position).magnitude <= context.minDistanceAtRuntime)
                {
                    return true;
                }
            }
            return false;
        }
    }

    private class AreApproachableNPCsNearby : Node<NPC>
    {
        public override bool Update(NPC context)
        {
            foreach(NPC npc in context.nearbyNPCs)
            {
                if ((npc.transform.position - context.transform.position).magnitude > context.minDistanceAtRuntime)
                {
                    return true;
                }
            }
            return false;
        }
    }

    private class AreNPCsNearby : Node<NPC>
    {
        public override bool Update(NPC context)
        {
            if (context.nearbyNPCs.Count > 0)
            {
                return true;
            }    
            else
            {
                return false;
            }
        }
    }
}
