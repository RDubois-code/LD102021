using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // FixedUpdate is called at a fixed rate
    void FixedUpdate()
    {
        this.transform.position = target.position + offset;
    }

    public Vector3 offset;
    public Transform target;
}
