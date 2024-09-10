using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator PlayerAnimator;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            ThirdPersonController player = other.GetComponent<ThirdPersonController>();
            if(PlayerAnimator){
                player.health -= 2;
                PlayerAnimator.SetTrigger("hit");
            }
        }
    }
    
}
