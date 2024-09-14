using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    public GameObject scene;
    public GameObject player;
    // Start is called before the first frame update
    public void Swap(AnimationEvent animationEvent)
    {
        scene.gameObject.SetActive(false);
        player.gameObject.SetActive(true);
    }
}
