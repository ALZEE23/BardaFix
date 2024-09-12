using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;
    private MovementInput movementInput;
    private CombatScript combatScript;
    public StarterAssetsInputs moveInput;

    public LayerMask layerMask;

    [SerializeField] Vector3 inputDirection;
    [SerializeField] private EnemyScript currentTarget;

    public GameObject cam;

    private void Start()
    {
        movementInput = GetComponentInParent<MovementInput>();
        combatScript = GetComponentInParent<CombatScript>();
    }

    private void Update()
    {
        DetectEnemiesWithSphereCast();
        RaycastToDetectEnemy();
    }

    // Fungsi yang sudah ada untuk mendeteksi musuh dengan SphereCast
    private void DetectEnemiesWithSphereCast()
    {
        var forward = transform.forward;  // Gunakan arah pemain
        var right = transform.right;

        forward.y = 0f;  // Kita abaikan perubahan di sumbu Y untuk tetap di bidang horizontal
        right.y = 0f;

        forward.Normalize();
        right.Normalize(); 

        // Mengambil input gerakan dari StarterAssetsInputs (tanpa kamera)
        inputDirection = forward * moveInput.move.y + right * moveInput.move.x;
        inputDirection = inputDirection.normalized;

        RaycastHit info;

        if (Physics.SphereCast(transform.position, 3f, inputDirection, out info, 10, layerMask))
        {
            EnemyScript enemy = info.collider.transform.GetComponent<EnemyScript>();
            if (enemy != null && enemy.IsAttackable())
            {
                currentTarget = enemy;
            }
        }
    }

    // Fungsi untuk mendeteksi musuh dengan Raycast ke arah depan pemain
    private void RaycastToDetectEnemy()
    {
        // Tentukan posisi awal raycast dari sedikit di depan pemain
        Vector3 rayOrigin = transform.position + transform.forward * 0.5f;

        // Gunakan arah pemain menghadap
        Vector3 rayDirection = transform.forward;

        // Panjang raycast
        float rayLength = 2.0f;

        // LayerMask untuk musuh
        LayerMask enemyMask = LayerMask.GetMask("Enemy");

        // Debugging raycast untuk visualisasi
        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

        // Melakukan raycast
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayLength, enemyMask))
        {
            Debug.Log("Musuh terdeteksi: " + hit.collider.name);

            // Mendapatkan EnemyScript dari collider yang terdeteksi
            EnemyScript enemy = hit.collider.GetComponent<EnemyScript>();
            if (enemy != null && enemy.IsAttackable())
            {
                currentTarget = enemy;
            }
        }
        else
        {
            // Debug.Log("Tidak ada musuh di depan.");
        }
    }

    public EnemyScript CurrentTarget()
    {
        return currentTarget;
    }

    public void SetCurrentTarget(EnemyScript target)
    {
        currentTarget = target;
    }

    public float InputMagnitude()
    {
        return inputDirection.magnitude;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, inputDirection);
        Gizmos.DrawWireSphere(transform.position, 1);
        if (CurrentTarget() != null)
            Gizmos.DrawSphere(CurrentTarget().transform.position, .5f);
    }
}
