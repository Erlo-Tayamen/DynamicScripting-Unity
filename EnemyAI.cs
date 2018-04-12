using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {

    // rigid body
    private Rigidbody rb;
    public Vector3 moveDir;

    // raycasting
    RaycastHit hit;
    Ray ray;
    LayerMask layer;

    // targeting
    public Transform target;
    public GameObject player;

    // speed and distance
    private float speed = 3f;
    private float rotSpeed = 5f;
    private float minDist = 5f;
    private float maxDist = 10f;

    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    // internal stats
    public float health = 5f;
    public float damage = 1f;
    public float ammo = 24;


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(transform.position, transform.forward, out hit, 100f);
        Debug.DrawRay(transform.position, transform.forward);
        followAttack();

    }

    // Follow and Attack
    public void followAttack()
    {
   
        // look at player
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), rotSpeed * Time.deltaTime);
        // move towards player
        transform.position += transform.forward * speed * Time.deltaTime;
        
        
    }


    // Damage and death
    public void takeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Destroy(gameObject); // die
        }
    }

    //shooting
    void shoot()
    {
        muzzleFlash.Play();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100f))
        {
            Debug.Log(hit.transform.name);

            PlayerShoot player = hit.transform.GetComponent<PlayerShoot>();
            if (player != null)
            {
                player.takeDamage(damage);
            }

            GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 2f);
        }

    }



    // Direction choosing
    Vector3 chooseDirection()
    {
        System.Random ran = new System.Random();
        int i = ran.Next(0, 3);
        Vector3 tmp = new Vector3();

        if (i == 0)
        {
            tmp = transform.forward;
        }
        else if (i == 1)
        {
            tmp = -transform.forward;
        }
        else if (i == 2)
        {
            tmp = transform.right;
        }
        else if (i == 3)
        {
            tmp = -transform.right;
        } 

        return tmp;

    }

    
}
