using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using StarterAssets;

public class EnemyScript : MonoBehaviour
{
    //Declarations
    private Animator animator;
    private PlayerAttack playerCombat;
    public EnemyManager enemyManager;
    private EnemyDetection enemyDetection;
    private CharacterController characterController;
    private StarterAssetsInputs starterAssetsInputs;

    [Header("Stats")]
    public int health = 3;
    private float moveSpeed = 1;
    private Vector3 moveDirection;


    [Header("Type")]
    public bool bos;
    public bool bigSkeleton;

    [Header("Weapon")]
    public GameObject laserWeapon;
    public LaserBeam laser;
    public bool beam;

    [Header("States")]
    [SerializeField] private bool isAware;
    public bool isDead = false;
    private bool hasTriggeredAware = false;

    [SerializeField] private bool isPreparingAttack;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isRetreating;
    [SerializeField] private bool isLockedTarget;
    [SerializeField] private bool isStunned;
    [SerializeField] private bool isWaiting = true;

    [Header("Polish")]
    [SerializeField] private ParticleSystem counterParticle;

    private Coroutine PrepareAttackCoroutine;
    private Coroutine RetreatCoroutine;
    private Coroutine DamageCoroutine;
    private Coroutine MovementCoroutine;

    //Events
    public UnityEvent<EnemyScript> OnDamage;
    public UnityEvent<EnemyScript> OnStopMoving;
    public UnityEvent<EnemyScript> OnRetreat;

    void Start()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        enemyManager = GetComponentInParent<EnemyManager>();


        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        playerCombat = FindObjectOfType<PlayerAttack>();
        enemyDetection = playerCombat.GetComponentInChildren<EnemyDetection>();

        playerCombat.OnHit.AddListener((x) => OnPlayerHit(x));
        playerCombat.OnCounterAttack.AddListener((x) => OnPlayerCounter(x));
        playerCombat.OnTrajectory.AddListener((x) => OnPlayerTrajectory(x));

        MovementCoroutine = StartCoroutine(EnemyMovement());

    }

    IEnumerator EnemyMovement()
    {
        //Waits until the enemy is not assigned to no action like attacking or retreating
        yield return new WaitUntil(() => isWaiting == true);

        int randomChance = Random.Range(0, 2);

        if (randomChance == 1)
        {
            int randomDir = Random.Range(0, 2);
            moveDirection = randomDir == 1 ? Vector3.right : Vector3.left;
            isMoving = true;
        }
        else
        {
            StopMoving();
        }

        yield return new WaitForSeconds(1);

        MovementCoroutine = StartCoroutine(EnemyMovement());
    }

    void Update()
    {
        

        //Constantly look at player
        if(this.health < 1)
        {
            Death();
        }

        if (Vector3.Distance(transform.position, playerCombat.transform.position) < 15)
        {
            isAware = true;
        }

        // if (isAware && !hasTriggeredAware)
        // {
        //     // Trigger the "Aware" animation only once
        //     StartCoroutine(OnAware());
        //     IEnumerator OnAware()
        //     {
        //         animator.SetBool("Aware", true);
        //         yield return new WaitForSeconds(0.05f);
        //         hasTriggeredAware = true;
        //         animator.SetBool("Aware", false);
        //     }
        //     // Set flag to true after triggering
        // }

        if (isAware)
        {
            // Rotate to look at player
            transform.LookAt(new Vector3(playerCombat.transform.position.x, transform.position.y, playerCombat.transform.position.z));
            MoveEnemy(moveDirection);
        }
        //Only moves if the direction is set

    }

    //Listened event from Player Animation
    void OnPlayerHit(EnemyScript target)
    {
        if (target == this && GetAvailability())
        {
            StopEnemyCoroutines();
            DamageCoroutine = StartCoroutine(HitCoroutine());

            enemyDetection.SetCurrentTarget(null);
            isLockedTarget = false;
            OnDamage.Invoke(this);

            health--;

            if (health < 0 || health == 0 || health <= 0 || health < 1)
            {
                Death();
                return;
            }

            animator.SetTrigger("Hit");
            transform.DOMove(transform.position - (transform.forward / 2), .3f).SetDelay(.1f);

            StopMoving();
        }

        IEnumerator HitCoroutine()
        {
            isStunned = true;
            yield return new WaitForSeconds(.5f);
            isStunned = false;
        }
    }

    void OnPlayerCounter(EnemyScript target)
    {
        if (target == this)
        {
            PrepareAttack(false);
        }
    }

    void OnPlayerTrajectory(EnemyScript target)
    {
        if (target == this)
        {
            StopEnemyCoroutines();
            isLockedTarget = true;
            PrepareAttack(false);
            StopMoving();
        }
    }

    void Death()
    {
        StopEnemyCoroutines();

        isDead = true;
        characterController.enabled = false;
        animator.SetTrigger("Death");
        enemyManager.SetEnemyAvailiability(this, false);
        StartCoroutine(DestroyEnemy());
        this.enabled = false;
        IEnumerator DestroyEnemy()
        {
            if (bigSkeleton)
            {
                yield return new WaitForSeconds(0.3f); this.enabled = false;
            } else
            {
                yield return new WaitForSeconds(0.3f); 

            }
           
             
        }
    }
    public bool GetAvailability()
    {
        for (int i = 0; i < enemyManager.allEnemies.Length; i++)
        {
            if (enemyManager.allEnemies[i].enemyScript == this)
            {
                return enemyManager.allEnemies[i].enemyAvailability;
            }
        }
        return false; // Default jika tidak ditemukan
    }

    // Set enemyAvailability
    public void SetAvailability(bool state)
    {
        for (int i = 0; i < enemyManager.allEnemies.Length; i++)
        {
            if (enemyManager.allEnemies[i].enemyScript == this)
            {
                enemyManager.allEnemies[i].enemyAvailability = state;
                break;
            }
        }
    }


    public void SetRetreat()
    {
        StopEnemyCoroutines();

        RetreatCoroutine = StartCoroutine(PrepRetreat());

        IEnumerator PrepRetreat()
        {
            yield return new WaitForSeconds(1.4f);
            OnRetreat.Invoke(this);
            isRetreating = true;
            moveDirection = -Vector3.forward;
            isMoving = true;
            if (bigSkeleton) { 
                yield return new WaitUntil(() => Vector3.Distance(transform.position, playerCombat.transform.position) > 1); 
            } else
            {
                 yield return new WaitUntil(() => Vector3.Distance(transform.position, playerCombat.transform.position) > 4);
            }
           
            isRetreating = false;
            StopMoving();

            //Free 
            isWaiting = true;
            MovementCoroutine = StartCoroutine(EnemyMovement());
        }
    }

    public void SetAttack()
    {
        isWaiting = false;

        PrepareAttackCoroutine = StartCoroutine(PrepAttack());

        IEnumerator PrepAttack()
        {
            PrepareAttack(true);
            yield return new WaitForSeconds(Random.Range(.2f, 2f));
            moveDirection = Vector3.forward;
            isMoving = true;
        }
    }


    void PrepareAttack(bool active)
    {
        isPreparingAttack = active;

        if (active)
        {
            counterParticle.Play();
        }
        else
        {
            StopMoving();
            counterParticle.Clear();
            counterParticle.Stop();
        }
    }

    void MoveEnemy(Vector3 direction)
    {
        //Set movespeed based on direction
        moveSpeed = 1;
        
        if (direction == Vector3.forward)
            moveSpeed = 5;
        if (direction == -Vector3.forward)
            moveSpeed = 2;

        //Set Animator values
        animator.SetFloat("InputMagnitude", (characterController.velocity.normalized.magnitude * direction.z) / (5 / moveSpeed), .2f, Time.deltaTime);
        animator.SetBool("Strafe", (direction == Vector3.right || direction == Vector3.left));
        animator.SetFloat("StrafeDirection", direction.normalized.x, .2f, Time.deltaTime);

        //Don't do anything if isMoving is false
        if (!isMoving)
            return;

        Vector3 dir = (playerCombat.transform.position - transform.position).normalized;
        Vector3 pDir = Quaternion.AngleAxis(90, Vector3.up) * dir; //Vector perpendicular to direction
        Vector3 movedir = Vector3.zero;

        Vector3 finalDirection = Vector3.zero;

        if (direction == Vector3.forward)
            finalDirection = dir;
        if (direction == Vector3.right || direction == Vector3.left)
            finalDirection = (pDir * direction.normalized.x);
        if (direction == -Vector3.forward)
            finalDirection = -transform.forward;

        if (direction == Vector3.right || direction == Vector3.left)
            moveSpeed /= 1.5f;

        movedir += finalDirection * moveSpeed * Time.deltaTime;

        characterController.Move(movedir);

        if (!isPreparingAttack)
            return;

        if (bos == true)
        {
            if (Vector3.Distance(transform.position, playerCombat.transform.position) < 4)
            {
                StopMoving();
                if (!playerCombat.isCountering && !playerCombat.isAttackingEnemy)
                    Attack();
                else
                    PrepareAttack(false);
            }
            else if (Vector3.Distance(transform.position, playerCombat.transform.position) > 4)
            {

            }
        }
        else if (!bos)
        {
            if (Vector3.Distance(transform.position, playerCombat.transform.position) < 2)
            {
                StopMoving();
                if (!playerCombat.isCountering && !playerCombat.isAttackingEnemy)
                    Attack();
                else
                    PrepareAttack(false);
            }
        }


    }

    private void Attack()
    {
        transform.DOMove(transform.position + (transform.forward / 1), .5f);
        animator.SetTrigger("AirPunch");
        if (bos == true && Vector3.Distance(transform.position, playerCombat.transform.position) < 4)
        {
            // laserWeapon.SetActive(true);

            animator.SetBool("Beam", true);
        }

    }

    public void StopHit()
    {
        laserWeapon.SetActive(false);
    }

    public void HitEvent()
    {
        if (!playerCombat.isAttackingEnemy && bos == true)
        {
            playerCombat.DamageEventBos();
            // laserWeapon.SetActive(true);
        }

        if (!playerCombat.isCountering && !playerCombat.isAttackingEnemy && bos == false)
            playerCombat.DamageEvent();

        PrepareAttack(false);
    }

    public void StopMoving()
    {
        isMoving = false;
        moveDirection = Vector3.zero;
        if (characterController.enabled)
            characterController.Move(moveDirection);
    }

    void StopEnemyCoroutines()
    {
        PrepareAttack(false);

        if (isRetreating)
        {
            if (RetreatCoroutine != null)
                StopCoroutine(RetreatCoroutine);
        }

        if (PrepareAttackCoroutine != null)
            StopCoroutine(PrepareAttackCoroutine);

        if (DamageCoroutine != null)
            StopCoroutine(DamageCoroutine);

        if (MovementCoroutine != null)
            StopCoroutine(MovementCoroutine);
    }

    #region Public Booleans

    public bool IsAttackable()
    {
        return health > 0;
    }

    public bool IsPreparingAttack()
    {
        return isPreparingAttack;
    }

    public bool IsRetreating()
    {
        return isRetreating;
    }

    public bool IsLockedTarget()
    {
        return isLockedTarget;
    }

    public bool IsStunned()
    {
        return isStunned;
    }

    #endregion
}
