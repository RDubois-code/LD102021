using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float groundControl = 10.0f;
    public float airControl = 5.0f;

    public float jumpForce = 100.0f;

    public float grassMeltingFactor = 0.1f;
    public float snowGrowingFactor = 0.1f;

    public float scale = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.myRigidbody = this.GetComponent<Rigidbody2D>();
    }

    // FixedUpdate is called at a fixed rate
    void FixedUpdate()
    {
        HandleMovement();

        HandleScaling();
    }

    void HandleMovement()
    {
        if(this.myRigidbody.IsTouchingLayers(LayerMask.GetMask("Grass", "Snow")))
        {
            this.myRigidbody.AddForce(Vector3.right * Input.GetAxis("Horizontal") * this.groundControl);

            if (Input.GetButton("Jump"))
            {
                this.myRigidbody.AddForce(Vector3.up * this.jumpForce);
            }
        }
        else
		{
            this.myRigidbody.AddForce(Vector3.right * Input.GetAxis("Horizontal") * this.airControl);
        }
    }

    void HandleScaling()
    {
        if (this.myRigidbody.IsTouchingLayers(LayerMask.GetMask("Grass")))
        {
            this.scale -= this.grassMeltingFactor * Time.deltaTime;
        }
        else if (this.myRigidbody.IsTouchingLayers(LayerMask.GetMask("Snow")))
        {
            this.scale += this.snowGrowingFactor * Time.deltaTime;
        }

        this.transform.localScale = Vector3.one * this.scale;
        this.myRigidbody.mass = this.scale;
    }

    Rigidbody2D myRigidbody;
}
