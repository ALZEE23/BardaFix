using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public List<EnemyManager> enemyManagers; // Daftar semua EnemyManager
    private int activeManagerIndex = 0; // Indeks gelombang yang sedang aktif
    private int finishedWaveCount = 0; // Jumlah EnemyManager yang sudah selesai

    void Start()
    {
        foreach (EnemyManager manager in enemyManagers)
        {
            manager.gameObject.SetActive(false); // Hide semua EnemyManager di awal
        }

        if (enemyManagers.Count > 0)
        {
            ActivateNextWave();
        }
    }

    // Mengecek jika ada EnemyManager yang sudah selesai (musuh 0)
    public void CheckNextWave(EnemyManager manager)
    {
        if (manager.AliveEnemyCount() == 1)
        {
            finishedWaveCount++;
            manager.gameObject.SetActive(false); // Sembunyikan EnemyManager yang sudah selesai

            // Jika 3 EnemyManager selesai, munculkan yang berikutnya
            if (finishedWaveCount % 3 == 0 && activeManagerIndex < enemyManagers.Count - 1)
            {
                ActivateNextWave();
            }
        }
    }

    // Mengaktifkan wave/EnemyManager berikutnya
    private void ActivateNextWave()
    {
        activeManagerIndex++; // Pindah ke wave berikutnya
        if (activeManagerIndex < enemyManagers.Count)
        {
            enemyManagers[activeManagerIndex].gameObject.SetActive(true); // Munculkan EnemyManager
            enemyManagers[activeManagerIndex].StartAI(); // Jalankan AI di EnemyManager
        }
    }
}