using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ControlledScript : MonoBehaviour {

    public class Rule
    {
        public int weight;
        public bool activated;
        public float reward;
        public float maxReward;
        public float minReward;

        public Rule (int _weight, bool _activated)
        {
            weight = _weight;
            activated = _activated;
        }

        public void updateReward(float amount)
        {
            if (reward < maxReward)
            {
                reward += amount;
            }
        }

        public float getReward()
        {
            return reward;
        }

        public int getWeight()
        {
            return weight;
        }

        public bool getActivated()
        {
            return activated;
        }

        public void setWeight(int amount)
        {
            weight += amount;
        }

        public void setActivated(bool _activated)
        {
            activated = _activated;
        }

    }
    
    delegate void useRule();

    /***** Exra Variables ******/
    private int i;
    private int j;
    private int condition = 1;

    /***** Script Generation ******/
    private int ruleCount = 9;
    private int sizeOfScript = 5;
    private int[] weights;
    private int sumWeights;
    private int tryNum;
    private int maxTries;
    private int sum;
    private int selected;
    private bool lineadded;
    private int fraction;


    /***** NPC Attributes ******/

    // navMesh
    public Vector3 moveDir;
    NavMeshAgent agent;

    // targeting
    public Transform target;
    public Transform[] wayPoint;
    public int destPoint = 0;
    public int point = 0;
    public int pos = 0;

    // pickups
    public Transform[] ammoPickup;
    public Transform[] healthPickup;

    public Vector3 spawnPoint;
    public GameObject player;
    public float distance;
    public float fleeDistance;
    public float attackDistance;

    // speed and distance
    private float timer = 5f;

    public float fireRate = 2;
    public float cooldown = 0;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    // internal stats
    public float health = 100f;
    public float damage = 10f;
    public float range = 100f;
    public float ammo = 25;
    public int Score = 0;
    public Text AIScore;

    // Lists
    List<int> currRules = new List<int>();
    List<useRule> script = new List<useRule>();
    public Rule[] rule;

    
    /**** Weight Adjustment ***/
    private int maxRules;
    private int fitness;
    private int adjustment;
    private int remainder;
    private int compensation;
    private int minWeight;
    private int maxWeight;
    private int active;
    private int nonActive;

    // Use this for initialization
    void Start () {
        spawnPoint = randSpawn();
        attackDistance = 30f;
        fleeDistance = 25f;
        agent = GetComponent<NavMeshAgent>();
        maxTries = 2;
        minWeight = 0;
        maxWeight = 200;

        initRules();
        summariseWeights();
        GenerateScript();
        // after death OR if opponent die;

	}

    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward);
        distance = Vector3.Distance(transform.position, target.transform.position);
        
    }

    // run on update
    void Update()
    {
        AIScore.text = "AI Score:" + Score.ToString();

        for (i = 0; i < sizeOfScript; i++) {
            script[currRules[i]]();
            Debug.Log(rule[i].getReward());
        }
        //if (Input.GetKeyDown("space"))
        //{
        //    Debug.Log("Generating a new script...");
        //    GenerateScript();
        //}
    }

 
    // generate Script
    void GenerateScript()
    {
        currRules.Clear();
        
        
        for (i = 0; i < sizeOfScript + 1; i++)
        {
            tryNum = 0;
            lineadded = false;
            while (tryNum < maxTries && !lineadded)
            {
                j = 0;
                sum = 0;
                selected = -1;
                fraction = Random.Range(0, sumWeights);
                while (selected < 0)
                {
                    sum += rule[j].getWeight();
                    if (sum > fraction)
                    {
                        selected = j;
                    }
                    else
                    {
                        j += 1;
                    }
                }
                InsertToScript(selected);
                tryNum += 1;
            }

        }
    }

    // insert rule in to script
    bool InsertToScript(int rule)
    {
        if (!currRules.Contains(rule) && currRules.Count != 5)
        {
            Debug.Log("Rule added:" + rule);
            currRules.Add(rule);
            return true;
        }
        else
        {
            return false;
        }
    }

    // summarise weights
    void summariseWeights()
    {
        for (i = 0; i < ruleCount; i++)
        {
            sumWeights += rule[i].getWeight();
        }
    }

    // weight adjustment
    void weightAdjustment()
        {
            active = 0;
            for (i = 0; i < ruleCount - 1; i++)
            {
                if (rule[i].getActivated())
                {
                    active += 1;
                }
            }
            if (active <= 0 || active >= ruleCount)
            {
                return;
            }
            nonActive = ruleCount - active;
            adjustment = CalculateAdjustment(fitness);
            compensation = (-active * adjustment) / nonActive;
            remainder = 0;

            for (i = 0; i < ruleCount - 1; i++)
            {
                if (rule[i].getActivated())
                {
                    rule[i].setWeight(adjustment);
                }
                else
                {
                    rule[i].setWeight(compensation);
                }

                if (rule[i].getWeight() < minWeight)
                {
                    remainder += rule[i].getWeight() - minWeight;
                    rule[i].setWeight(minWeight);
                }
                else if (rule[i].getWeight() > maxWeight)
                {
                    remainder += rule[i].getWeight() + maxWeight;
                    rule[i].setWeight(maxWeight);
                }
            }
        }

    // calculate adjusment
    int CalculateAdjustment(int fitness)
    {
        int adjustment = 2;
        return adjustment;
    }


    // initialise rules
    void initRules()
    {
        rule = new Rule[ruleCount];
        for (i = 0; i < ruleCount; i++)
        {
            rule[i] = new Rule(1, false);
        }

        script.Add(FollowAttack);
        script.Add(StandingAttack);
        script.Add(SideStepAttack);
        script.Add(FindAmmo);
        script.Add(FindHealth);
        script.Add(Flee);
        script.Add(Idle);
        script.Add(Investigate);
        script.Add(Patrol);

    }

    // Rulebase
    void FollowAttack()
    {
        if (ammo > 0 && distance < attackDistance) {
            agent.SetDestination(target.position);
            shoot();
            Debug.Log("FollowAttack");
            rule[0].setActivated(true);
            rule[0].updateReward(0.2f);
        }
    }

    void StandingAttack()
    {
        if (ammo > 0) { 
            transform.LookAt(target.transform);
            shoot();
            Debug.Log("StandingAttack");
            rule[1].setActivated(true);
            rule[1].updateReward(0.2f);
        }
    }

    void SideStepAttack()
    {
        if (distance < attackDistance )
        {
            transform.LookAt(target.transform);
            agent.SetDestination(new Vector3(transform.position.x + 10F, transform.position.y, transform.position.z));
            shoot();
            Debug.Log("SideStepAttack");
            rule[2].setActivated(true);
            rule[2].updateReward(0.2f);
        }
    }
    
    void FindAmmo()
    {
        if (ammo < 25f && !agent.pathPending)
        {
            System.Random ran = new System.Random();
            int x = ran.Next(0, 4);
            agent.SetDestination(ammoPickup[x].position);
            Debug.Log("FindAmmo");
            rule[3].setActivated(true);
            rule[3].updateReward(0.2f);

        }
    }

    void FindHealth()
    {
        if (health < 20f && !agent.pathPending)
        {
            System.Random ran = new System.Random();
            int y = ran.Next(0, 3);
            agent.SetDestination(healthPickup[y].position);
            rule[4].setActivated(true);
            rule[4].updateReward(0.2f);
            Debug.Log("FindHealth");
        }
    }

    void Flee()
    {
        if (health < 25f || ammo <= 0 && distance < fleeDistance){
            Vector3 dirToPlayer = transform.position - target.transform.position;
            Vector3 newPos = transform.position + dirToPlayer;
            agent.SetDestination(newPos);
            rule[5].setActivated(true);
            rule[5].updateReward(0.2f);
            Debug.Log("Flee");
        }
    }

    void Idle()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            rule[6].setActivated(true);
            rule[6].updateReward(0.2f);
            Debug.Log("Idle");
        }
        timer = 5f;
    }

    void Investigate()
    {
        if (condition == 8)
        {
            rule[7].setActivated(true);
            rule[7].updateReward(0.2f);
            Debug.Log("Investigate");
        }
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (wayPoint.Length == 0)
            {
                return;
            }
        
            if (distance > attackDistance)
            {
                rule[8].setActivated(true);
                rule[8].updateReward(0.2f);
                agent.destination = wayPoint[destPoint].position;
                destPoint = (destPoint + 1) % wayPoint.Length;
            }
        }
    }

    // basic control
    void updateScore()
    {
        player.GetComponent<PlayerShoot>().Score += 1;
    }

    // on triggercollider
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "health" || col.gameObject.tag == "ammo")
        {
            Destroy(col.gameObject);
        }
    }

    // spawn
    Vector3 randSpawn()
    {
        System.Random ran = new System.Random();
        int i = ran.Next(0, 2);
        if (i == 0)
        {
            return GameObject.Find("spawnPoint1").transform.position;
        }
        else if (i == 1)
        {
            return GameObject.Find("spawnPoint2").transform.position;
        }
        else
        {
            return GameObject.Find("spawnPoint3").transform.position;
        }
    }

    // Damage and death
    public void takeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            transform.position = spawnPoint;  // die
            updateScore();
            weightAdjustment();
            health = 100f;
        }
    }

    //shooting
    void shoot()
    {
        if (ammo > 0 && Time.time >= cooldown)
        {
            cooldown = Time.time + 1f / fireRate;
            muzzleFlash.Play();
            ammo -= 1;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 100f))
            {

                PlayerShoot player = hit.transform.GetComponent<PlayerShoot>();
                if (player != null)
                {
                    player.takeDamage(damage);
                }

                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }
        }
    }
}
