using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDeath : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject.Find("Snowman").GetComponent<GameEnd>().GameOver();
    }
}
