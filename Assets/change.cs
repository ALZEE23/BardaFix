using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class change : MonoBehaviour
{
   
    public void Cyber4()
    {
        SceneManager.LoadScene("Cyberpunk 2");
    }

    public void Cyber2()
    {
        SceneManager.LoadScene("Cyberpunk 3");
    }

    public void Cyber3()
    {
        SceneManager.LoadScene("CutScene");
    }


}
