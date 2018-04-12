using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShoot : MonoBehaviour {

    // internal stats
    public float damage = 10f;
    public float range = 100f;
    public float health = 100f;
    public float ammo = 25f;
    public int Score = 0;
    public Text myScore;

    // spawn point
    Vector3 spawnPoint;

    // shooting setup
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public GameObject NPC;

    public float fireRate = 2;
    public float cooldown = 0;

    // Use this for initialization
    void Start()
    {
        spawnPoint = randSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        myScore.text = "Player Score:" + Score.ToString();
        Debug.DrawRay(fpsCam.transform.position, fpsCam.transform.forward);
        if (Input.GetKeyDown("space") && Time.time >= cooldown)
        {
            cooldown = Time.time + 1f / fireRate;
            shoot();
        }
    }

    // player class

    // Damage and death
    public void takeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Debug.Log("Dead");
            transform.position = spawnPoint; // die
            updateScore();
            health = 100f;
        }

    }

    void updateScore()
    {
        NPC.GetComponent<ControlledScript>().Score += 1;
    }

    // shoot method
    void shoot() {

        muzzleFlash.Play();
        ammo -= 1;
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            ControlledScript NPC = hit.transform.GetComponent<ControlledScript>();
            if (NPC != null)
            {
                Debug.Log("Damage");
                NPC.takeDamage(damage);
            }

            GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 2f);
        }

    }

    Vector3 randSpawn()
    {
        System.Random ran = new System.Random();
        int i = ran.Next(0, 2);
        if (i == 0)
        {
            return GameObject.Find("spawnPoint1").transform.position;
        }
        else if(i == 1)
        {
            return GameObject.Find("spawnPoint2").transform.position;
        }
        else
        {
            return GameObject.Find("spawnPoint3").transform.position;
        }
    }

    }
