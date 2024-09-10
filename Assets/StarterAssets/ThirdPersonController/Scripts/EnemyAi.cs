using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public Animator animator;
    public int Health = 5;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public bool dead;
    public float speed = 3.0f;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
       // Check if player is within sight range or attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        
        if(agent.enabled && !dead){
        if (!playerInSightRange && !playerInAttackRange && !dead) Patroling();
        if (playerInSightRange && !playerInAttackRange && !dead) ChasePlayer();
        if (playerInAttackRange && playerInSightRange && !dead) AttackPlayer();


        if(Health < 1){
            animator.SetBool("dead", dead=true);
            agent.SetDestination(transform.position);
            StartCoroutine(Dead(5.0f));
        }}
        
    }

    private void Patroling()
    {
        // Optional: Add patrol logic here
        speed = 0.0f;
        agent.SetDestination(transform.position);
        animator.SetFloat("Speed", speed);
        animator.ResetTrigger("attack");
    }

    private void ChasePlayer()
    {
        speed = 2.0f    ;
        agent.SetDestination(player.position) ;
        animator.SetFloat("Speed", speed);
        animator.ResetTrigger("attack");
    }

    private void AttackPlayer()
    {
        // Stop AI from moving
        speed = 0.0f;
        agent.SetDestination(transform.position);
        // animator.SetTrigger("attack");
        if(!playerInAttackRange && playerInSightRange){
            StartCoroutine(Stop(0.2f));
            animator.ResetTrigger("attack");
        }

        if(playerInAttackRange){
            StartCoroutine(AttackAgain(0.5f));
        }
               // Optional: Add attack logic here (e.g., play an attack animation)

        // Ensure the AI is facing the player
        transform.LookAt(player);
    }

    IEnumerator Dead(float delay){
        animator.ResetTrigger("attack");
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);

    }

    IEnumerator Stop(float delay){
        yield return new WaitForSeconds(delay);
        animator.ResetTrigger("attack");
        speed = 2.0f;
    }

    IEnumerator Cooldown(float delay){
        yield return new WaitForSeconds(delay);
        animator.ResetTrigger("attack");
        speed = 0.0f;
        if(playerInAttackRange){
            StartCoroutine(AttackAgain(2.0f));
        } 
        
    }

    IEnumerator AttackAgain(float delay){
        yield return new WaitForSeconds(delay);
        speed = 0.0f;
        animator.SetTrigger("attack");
        if(playerInAttackRange){StartCoroutine(Cooldown(0.1f));}
        
    }
}
