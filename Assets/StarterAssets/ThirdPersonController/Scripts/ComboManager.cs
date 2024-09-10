using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

[Serializable]
public class MoveSet
{
    public AudioClip[] AttackSound;
    public string NameAnimation;
    public bool AnimationActive;
    public int Input;

}

[Serializable]
public class AddCombo
{

    public string ComboName;

    public List<MoveSet> moveSets = new List<MoveSet>();

}
public class ComboManager : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Add Combo")]
    public Animator Anim;
    public List<AddCombo> combos = new List<AddCombo>();




    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
