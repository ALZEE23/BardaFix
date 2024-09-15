using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class jaja : MonoBehaviour
{
    // Nama scene yang akan dimuat
    private void Start()
    {
        StartCoroutine(LoadS());
        IEnumerator LoadS()
        {   
            yield return null; 
            
        }
       
    }

    private void Awake()
    {
        
            SceneManager.LoadScene("Cyberpunk 2");
    }
}
