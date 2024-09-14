using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public EnemyManager enemyManager;
    [SerializeField] private EnemyScript currentTarget;
    [SerializeField] private float detectionRadius = 5.0f; // Jarak deteksi musuh
    public LayerMask enemyLayerMask; // Layer musuh

    private void Update()
    {
        DetectEnemiesAroundPlayer();
    }

    private void DetectEnemiesAroundPlayer()
    {
        // Mendapatkan semua collider dalam radius deteksi
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayerMask);

        // Jika ada musuh yang terdeteksi
        if (hitColliders.Length > 0)
        {
            float closestDistance = Mathf.Infinity;
            EnemyScript closestEnemy = null;

            // Loop melalui collider yang ditemukan
            foreach (Collider hitCollider in hitColliders)
            {
                EnemyScript enemy = hitCollider.GetComponent<EnemyScript>();

                if (enemy != null && enemy.IsAttackable()) // Pastikan collider punya EnemyScript
                {
                    // Hitung jarak dari pemain ke musuh
                    float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

                    // Cari musuh yang paling dekat
                    if (distanceToEnemy < closestDistance)
                    {
                        closestDistance = distanceToEnemy;
                        closestEnemy = enemy;
                    }
                }
            }

            // Tetapkan musuh terdekat sebagai target
            if (closestEnemy != null)
            {
                currentTarget = closestEnemy;
                Debug.Log("Target terkunci: " + currentTarget.name);

                // Set EnemyManager berdasarkan musuh yang ditemukan
                enemyManager = currentTarget.enemyManager;
                Debug.Log("EnemyManager set to: " + enemyManager.aliveEnemyCount);
            }
        }
        else
        {
            // Tidak ada musuh, set target dan enemy manager ke null
            currentTarget = null;
            enemyManager = null;
            //Debug.Log("Tidak ada musuh di sekitar.");
        }
    }

    private void OnDrawGizmos()
    {
        // Visualisasi radius deteksi di scene
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(currentTarget.transform.position, 0.5f);
        }
    }

    public EnemyScript CurrentTarget()
    {
        return currentTarget;
    }

    public void SetCurrentTarget(EnemyScript target)
    {
        currentTarget = target;

        // Set juga EnemyManager sesuai target
        if (currentTarget != null)
        {
            enemyManager = currentTarget.enemyManager;
        }
    }
}