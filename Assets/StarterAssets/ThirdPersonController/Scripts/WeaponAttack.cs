using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class WeaponAttack : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Collider col;
    public Animator EnemyAnimator;
    public bool timeToggle;
    public float timeScale;
    float defaultTimeScale;
    float defaultFixedDeltaTime;
    public EnemyAi enemy;
    public bool knockback;
    void Start()
    {
        defaultTimeScale = Time.timeScale;
        defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemy = other.GetComponent<EnemyAi>();
            EnemyAnimator = other.GetComponent<Animator>();

            if (EnemyAnimator && enemy != null)
            {
                
                enemy.playerInAttackRange = false;
                enemy.playerInSightRange = false;
                EnemyAnimator.SetTrigger("hit");
                EnemyAnimator.ResetTrigger("attack");
                enemy.Health -= 1;

                if(enemy.dead == false && knockback == true){StartCoroutine(SmoothKnockback(other.transform)) ;}
                StartCoroutine(End());
            }

            
            Debug.Log("kena");
            
            // timeToggle = !timeToggle;
            // Time.timeScale = timeToggle ? timeScale : defaultTimeScale;
            // Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
            // StartCoroutine(Slow(0.1f));
        }
    }

    // void OnTriggerExit(Collider other){
    //     if(other.gameObject.CompareTag("Enemy")){
    //         Time.timeScale = defaultTimeScale;
    //     }
    // }

    IEnumerator End()
    {
        yield return new WaitForSeconds(0.1f);
        
        // if (enemy != null)
        // {
        //     enemy.agent.enabled = true;
            
        // }
        if(enemy != null){
            EnemyAnimator.ResetTrigger("hit");
        enemy.playerInAttackRange = true;
        enemy.playerInSightRange = true;
        }
    }

    IEnumerator Slow(float delay)
    {
        yield return new WaitForSeconds(delay);
        Time.timeScale = defaultTimeScale;
        timeToggle = false;
    }

    IEnumerator SmoothKnockback(Transform enemyTransform)
    {
        Vector3 startPosition = enemyTransform.position;
        Vector3 knockbackDirection = -enemyTransform.forward;
        Vector3 endPosition = startPosition + knockbackDirection * 1f; // Adjust the 1f to your desired knockback distance

        float duration = 0.3f; // Time in seconds for the knockback to complete
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            enemyTransform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        enemyTransform.position = endPosition; // Ensure the final position is set correctly
    }   
}
