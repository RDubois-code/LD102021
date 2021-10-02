using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float moveForce = 10.0f;
    public float jumpForce = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.myRigidbody = this.GetComponent<Rigidbody2D>();
    }

    // FixedUpdate is called at a fixed rate
    void FixedUpdate()
    {
        this.myRigidbody.AddForce(Vector3.right * Input.GetAxis("Horizontal") * this.moveForce);

        if(Input.GetButton("Jump") && this.myRigidbody.IsTouchingLayers(Physics2D.AllLayers))
		{
            this.myRigidbody.AddForce(Vector3.up * this.jumpForce);
        }
    }

    Rigidbody2D myRigidbody;
}
