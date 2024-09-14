using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;


public class PlayerAttack : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    // public Rigidbody rb;
    public int combo;
    public AudioClip attack1;
    public AudioClip attack2;
    public AudioClip attack3;
    public bool serang;
    public float comboResetTime = 1.0f; // Waktu tunggu untuk reset combo
    private float lastAttackTime; // Waktu serangan terakhir
    private StarterAssets.StarterAssetsInputs _input;


    [Header("States")]
    public bool isAttackingEnemy = false;
    public bool isCountering = false;
    public bool hidden = false;

    private CharacterController controller;
    public Collider Weapon;

    public ThirdPersonController player;
    public bool Keris;
    public bool Golok;


    public GameObject golok1;
    public GameObject keris2;
    public LaserBeam laserBeam;
    private EnemyDetection enemyDetection;
    public EnemyScript Bos;
    private EnemyScript lockedTarget;
    private EnemyManager enemyManager;
    private Coroutine counterCoroutine;
    private Coroutine attackCoroutine;
    private Coroutine damageCoroutine;
    [SerializeField] private ParticleSystemScript punchParticle;
    [SerializeField] private Transform punchPosition;

    public UnityEvent<EnemyScript> OnHit;
    public UnityEvent<EnemyScript> OnCounterAttack;
    public UnityEvent<EnemyScript> OnTrajectory;

    public bool impactLaser;

    int currentWeapon = 0;
    bool golokActive = true;

    // Start is called before the first frame update
    void Start()
    {
        //enemyManager = FindObjectOfType<EnemyManager>();
        animator = GetComponent<Animator>();
        _input = GetComponent<StarterAssets.StarterAssetsInputs>();
        combo = 0; // Mulai dari combo 0
        enemyDetection = GetComponentInChildren<EnemyDetection>();

        // laserBeam = FindObjectOfType<LaserBeam>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Keris", Keris);
        animator.SetBool("Golok", Golok);
        Combos();
        ResetComboWithTime();
        Switching();
        UnTouchable();
        enemyManager = enemyDetection.enemyManager;
        // if(laser.laserHit == true){
        //     impactLaser = true;
        // }
    }

    public void Switching()
    {
        if (_input.swap) // Ganti dengan input sistem yang sesuai
        {
            golokActive = !golokActive; // Membalik status senjata aktif

            if (golokActive)
            {
                Golok = true;
                Keris = false;
                Debug.Log("Golok aktif");
            }
            else
            {
                Golok = false;
                Keris = true;
                Debug.Log("Keris aktif");
            }
        }



        if (_input.axis > 0)
        {
            Golok = true;
            Keris = false;
        }
        else if (_input.axis < 0)
        {
            Keris = true;
            Golok = false;
        }



        if (Golok == true)
        {
            golok1.gameObject.SetActive(true);
        }
        else
        {
            golok1.gameObject.SetActive(false);
        }

        if (Keris == true)
        {
            keris2.gameObject.SetActive(true);
        }
        else
        {
            keris2.gameObject.SetActive(false);
        }
    }

    public void StartCombo()
    {
        serang = false;
        if (combo < 3)
        {
            combo++;

        }
    }

    void AttackCheck()
    {
        if (isAttackingEnemy)
            return;

        // Cek apakah behavior deteksi musuh memiliki target yang terdeteksi
        if (enemyDetection.CurrentTarget() == null)
        {
            // Jika tidak ada musuh yang terdeteksi dan tidak ada musuh hidup, hentikan serangan
            if (enemyManager.AliveEnemyCount() == 0)
            {
                AttackType(null);
                return;
            }
            else
            {
                // Jika tidak ada musuh yang terdeteksi, pilih musuh secara acak dari EnemyManager
                lockedTarget = enemyManager.RandomEnemy();
            }
        }
        else
        {
            // Jika musuh terdeteksi di sekitar pemain, set sebagai lockedTarget
            lockedTarget = enemyDetection.CurrentTarget();
        }

        // Cek ekstra apakah lockedTarget sudah di-set, jika tidak set secara acak
        if (lockedTarget == null)
            lockedTarget = enemyManager.RandomEnemy();

        // Serang target yang terkunci
        Attack(lockedTarget, TargetDistance(lockedTarget));
    }

    public void Attack(EnemyScript target, float distance)
    {
        //Types of attack animation
        //attacks = new string[] { "1", "2", "3" };

        //Attack nothing in case target is null
        if (target == null)
        {
            AttackType(null);
            return;
        }

        if (distance < 15)
        {
            //animationCount = (int) Mathf.Repeat((float) animationCount + 1, (float) attacks.Length);
            //string attackString = isLastHit() ? attacks[Random.Range(0, attacks.Length)] : attacks[animationCount];
            AttackType(target);
        }
        else
        {
            lockedTarget = null;
            AttackType(target);
        }

        //Change impulse
        //impulseSource.m_ImpulseDefinition.m_AmplitudeGain = Mathf.Max(3, 1 * distance);

    }

    float TargetDistance(EnemyScript target)
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }

    void AttackType(EnemyScript target)
    {


        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCoroutine(.65f));

        //Check if last enemy
        if (isLastHit())
            StartCoroutine(FinalBlowCoroutine());

        if (target == null)
            return;

        target.StopMoving();

        IEnumerator AttackCoroutine(float duration)
        {

            isAttackingEnemy = true;
            yield return new WaitForSeconds(duration);
            isAttackingEnemy = false;
            yield return new WaitForSeconds(.2f);
            LerpCharacterAcceleration();
        }

        IEnumerator FinalBlowCoroutine()
        {
            Time.timeScale = .5f;
            //lastHitCamera.SetActive(true);
            //lastHitFocusObject.position = lockedTarget.transform.position;
            yield return new WaitForSecondsRealtime(2);
            //lastHitCamera.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    bool isLastHit()
    {
        if (lockedTarget == null)
            return false;

        return enemyManager.AliveEnemyCount() == 1 && lockedTarget.health <= 1;
    }

    public void HitEvent()
    {
        if (lockedTarget == null || enemyManager.AliveEnemyCount() == 0)
        {
            Debug.LogWarning("HitEvent gagal: lockedTarget null atau tidak ada musuh hidup.");
            return;
        }

        Debug.Log("HitEvent dipanggil pada target: " + lockedTarget.name);
        OnHit.Invoke(lockedTarget);
        //Polish
        punchParticle.PlayParticleAtPosition(punchPosition.position);
    }

    public void FinishCombo()
    {
        isAttackingEnemy = false;
        serang = false;
        combo = 0; // Reset combo setelah stage terakhir

        // Pastikan semua trigger di-reset setelah combo selesai
        animator.ResetTrigger("1");
        animator.ResetTrigger("2");
        animator.ResetTrigger("3");
    }

    private void Combos()
    {
        if (_input.attack1 && !serang)
        {
            //isAttackingEnemy = true;
            isAttackingEnemy = true;
            serang = true;
            lastAttackTime = Time.time;

            // Increment combo stage up to 3
            if (combo < 3)
            {
                combo++;
            }

            player._speed = 1.0f;
            player.MoveSpeed = 1.0f;

            animator.SetTrigger("" + combo);

            // Coroutine untuk menunggu sebelum combo berikutnya
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        // Menunggu sampai animasi saat ini selesai
        yield return new WaitForSeconds(0.1f);

        _input.attack1 = false;
        isAttackingEnemy = false;

        // Jika combo telah mencapai tahap ketiga
        if (combo == 3)
        {
            FinishCombo();
            isAttackingEnemy = false;
        }
        else
        {
            serang = false; // Reset serangan agar bisa menerima input lagi
            player._speed = 3.5f;
            player.MoveSpeed = 3.5f;
        }
    }

    private void ResetComboWithTime()
    {
        if (Time.time - lastAttackTime > comboResetTime && combo > 0)
        {
            combo = 0; // Reset combo setelah waktu tertentu
            serang = false;
            isAttackingEnemy = false;
        }
    }

    private void onAttack1(AnimationEvent animationEvent)
    {
        ThirdPersonController player = this.GetComponent<ThirdPersonController>();
        controller = player._controller;
        AudioSource.PlayClipAtPoint(attack1, transform.TransformPoint(controller.center));
    }
    private void onAttack2(AnimationEvent animationEvent)
    {
        ThirdPersonController player = this.GetComponent<ThirdPersonController>();
        controller = player._controller;
        AudioSource.PlayClipAtPoint(attack2, transform.TransformPoint(controller.center));

    }
    private void onAttack3(AnimationEvent animationEvent)
    {
        ThirdPersonController player = this.GetComponent<ThirdPersonController>();
        controller = player._controller;
        AudioSource.PlayClipAtPoint(attack3, transform.TransformPoint(controller.center));
    }

    public void DamageEvent()
    {

        if (hidden == false)
        {
            animator.SetTrigger("hit");
            Debug.Log("kenahit");


            if (damageCoroutine != null)
                StopCoroutine(damageCoroutine);
            damageCoroutine = StartCoroutine(DamageCoroutine());

            IEnumerator DamageCoroutine()
            {
                player.health -= 2;
                player._speed = 0.5f;
                player.MoveSpeed = 0.5f;
                _input.enabled = false;
                yield return new WaitForSeconds(0.5f);
                player._speed = 3.5f;
                player.MoveSpeed = 3.5f;
                _input.enabled = true;
                LerpCharacterAcceleration();
            }

        }
    }

    public void DamageEventBos()
    {
        if (hidden == false && laserBeam.laserHit == true)
        {
            animator.SetTrigger("hit");
            Debug.Log("kenahit");


            if (damageCoroutine != null)
                StopCoroutine(damageCoroutine);
            damageCoroutine = StartCoroutine(DamageCoroutine());

            IEnumerator DamageCoroutine()
            {
                player.health -= 2;
                player._speed = 0.5f;
                player.MoveSpeed = 0.5f;
                _input.enabled = false;
                yield return new WaitForSeconds(0.5f);
                laserBeam.laserHit = false;
                player._speed = 3.5f;
                player.MoveSpeed = 3.5f;
                _input.enabled = true;
                LerpCharacterAcceleration();
            }

        }
    }

    void LerpCharacterAcceleration()
    {
        player.SpeedChangeRate = 0;
        DOVirtual.Float(0, 10.0f, 0, ((acceleration) => player.SpeedChangeRate = acceleration));
    }

    void UnTouchable()
    {
        if (_input.dash == true)
        {
            player._speed = 3.5f;
            player.MoveSpeed = 3.5f;
            StartCoroutine(Hide());
        }
    }

    IEnumerator Hide()
    {
        hidden = true;
        yield return new WaitForSeconds(0.7f);
        hidden = false;
    }


    private void OnAttack1()
    {
        AttackCheck();
    }
}
