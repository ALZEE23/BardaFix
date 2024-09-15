using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Scene(AnimationEvent animation)
    {
        StartCoroutine(sen());

        IEnumerator sen()
        {
            yield return new WaitForSeconds(3); 
            SceneManager.LoadScene("Credit");
        }
       
    }
}
