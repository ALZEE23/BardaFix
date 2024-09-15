using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustAnimation : MonoBehaviour
{

    public GameObject walkparticle;
    public Transform lefttransform;
    public Transform righttransform;
    public void ShowParticle()
    {

        var temp = Instantiate(walkparticle, lefttransform);
        temp.transform.localPosition = Vector3.zero;
    }

    public void ShowParticleRight()
    {

        var temp = Instantiate(walkparticle, righttransform);
        temp.transform.localPosition = Vector3.zero;

    }


}