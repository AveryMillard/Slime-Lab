using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : GenericEnemy
{
    [SerializeField] float enemySpeed;
    [SerializeField] public Transform[] patrolPoints;
    [SerializeField] float chaseRestTime = 5.0f;

    private Rigidbody rb;
    private EnemyFieldOfView enemyFov;
    private Vector3 playerDir;
    private Vector3 chaseStoppingAngle;
    private int currentPatrolPoint;
    private bool chasing;
    private bool cooldown;
    private float timeElapsedFromChase;
    private NavMeshAgent agent;
    private GameObject Player;
    private StealthRoomMechanics mechanicsScript;

    public AudioSource EnemySound;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyFov = GetComponent<EnemyFieldOfView>();
        agent = GetComponent<NavMeshAgent>();
        chasing = false;
        cooldown = false;
        Player = GameObject.FindGameObjectWithTag("Player");
        timeElapsedFromChase = 0.0f;
        chaseStoppingAngle = Vector3.zero;
        mechanicsScript = GameObject.FindGameObjectWithTag("Room").GetComponent<StealthRoomMechanics>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyFov.containsPlayer() && !cooldown)
        {
            ChasePlayer();
        }
        else
        {
            if (chasing)
            {
                if (chaseStoppingAngle == Vector3.zero)
                    chaseStoppingAngle = rb.transform.eulerAngles;
                timeElapsedFromChase += Time.deltaTime;
                if (cooldown)
                {
                    RotateInPlace();
                }
                if (timeElapsedFromChase >= chaseRestTime)
                {
                    agent.updatePosition = true;
                    agent.updateRotation = true;
                    agent.updateUpAxis = true;
                    agent.enabled = true;
                    chasing = false;
                    timeElapsedFromChase = 0.0f;
                    chaseStoppingAngle = Vector3.zero;
                }
                else
                {
                    RotateInPlace();
                }
            }
            else
            {
                Patrol();
            }
        }
    }

    void Patrol()
    {
        if (!agent.enabled)
            return;

        if (agent.velocity != Vector3.zero)
            rb.transform.rotation = Quaternion.LookRotation(agent.velocity, Vector3.up);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GotoNextPoint();
    }

    public void ChasePlayer()
    {
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.enabled = false;
        chasing = true;
        timeElapsedFromChase = 0.0f;
        chaseStoppingAngle = Vector3.zero;

        playerDir = (Player.transform.position - transform.position).normalized;
        rb.velocity = playerDir * enemySpeed;
        rb.rotation = Quaternion.LookRotation(playerDir, Vector3.up);
        if (CanHitPlayer())
        {
            Attack();
        }
    }

    void RotateInPlace()
    {
        float degreesOfMotion = enemyFov.viewAngle / 2;

        float point = timeElapsedFromChase / (chaseRestTime);

        if (point > 0.75f)
        {
            point = -(1.0f - point);
        }
        else if (point > 0.5f)
        {
            point = (-point) + 0.5f;
        }
        else if (point > 0.25f)
        {
            point = 0.5f - point;
        }

        Vector3 angle = rb.transform.eulerAngles;
        angle.y = chaseStoppingAngle.y + (degreesOfMotion * point);
        rb.transform.eulerAngles = angle;
    }

    public void GotoNextPoint()
    {
        if (patrolPoints.Length < 1)
            return;

        agent.destination = patrolPoints[currentPatrolPoint].position;
        currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;

    }

    bool CanHitPlayer()
    {
        if ((Player.transform.position - transform.position).magnitude <= 2.0f && !cooldown)
        {
            return true;
        }

        return false;
    }

    void Attack()
    {
        //EnemySound.Play();
        StartCoroutine(DeactivateAfterSound());
        StartCoroutine(HitCooldown());
        GameObject bossRoom = GameObject.Find("Boss Room");
        if (bossRoom != null)
        {
            bossRoom.GetComponent<BossRoomMechanics>().ResetEncounter();
        }
        //Player.GetComponent<PlayerManager>().Die();
    }

    private IEnumerator DeactivateAfterSound()
    {
        EnemySound.Play();
        yield return new WaitForSeconds(EnemySound.clip.length/2);
        Player.GetComponent<PlayerManager>().Die();
    }

    private IEnumerator HitCooldown()
    {
        cooldown = true;
        float timer = 0.0f;
        while (true)
        {
            yield return null;
            if (timer >= 4.0f)
            {
                cooldown = false;
                break;
            }
            timer += Time.deltaTime;
        }
    }
}
 