using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public LayerMask layers;
    bool onGrass = false;

    void FixedUpdate()
    {
        if (onGrass)
        {
           //Activité a faire      
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        this.onGrass = col.gameObject.layer == 6;
    }
}
