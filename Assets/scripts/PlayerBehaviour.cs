using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
    public float groundControl = 10.0f;
    public float airControl = 5.0f;

    public float jumpForce = 100.0f;

    public float grassMeltingFactor = 0.05f;
    public float snowGrowingFactor = 0.02f;
    public float airMeltingFactor = 0.01f;

    public float scale = 1.0f;

    public float minScale = 0.1f;
    public float maxScale = 10.0f;

    public float collisionDeadZone = 5.0f;
    public float collisionImpact = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        this.myRigidbody = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            SceneManager.LoadScene("Menu");
        }
		else
        {
            if (Input.GetButtonDown("Jump"))
            {
                jumpButton = true;
            }

            horizontalMove = Input.GetAxis("Horizontal");
        }
    }

    // FixedUpdate is called at a fixed rate
    void FixedUpdate()
    {
        if(this.isPlaying)
        {
            HandleMovement();
            HandleScaling();
        }
    }

    void HandleMovement()
    {
        if(this.myRigidbody.IsTouchingLayers(LayerMask.GetMask("Grass", "Snow")))
        {
            this.myRigidbody.AddForce(Vector3.right * horizontalMove * this.groundControl);

            if(this.jumpButton)
			{
                this.myRigidbody.AddForce(Vector3.up * this.jumpForce);
                this.jumpButton = false;
            }
        }
        else
		{
            this.myRigidbody.AddForce(Vector3.right * horizontalMove * this.airControl);
        }
    }

    void HandleScaling()
    {
        if (this.myRigidbody.IsTouchingLayers(LayerMask.GetMask("Snow")))
        {
            this.scale += this.snowGrowingFactor * this.myRigidbody.velocity.magnitude * Time.fixedDeltaTime;
        }
        else if (this.myRigidbody.IsTouchingLayers(LayerMask.GetMask("Grass")))
        {
            this.scale -= this.grassMeltingFactor * this.myRigidbody.velocity.magnitude * Time.fixedDeltaTime;
        }

        this.scale -= this.airMeltingFactor * Time.fixedDeltaTime;

        this.scale = Mathf.Clamp(this.scale, this.minScale, this.maxScale);

        this.transform.localScale = Vector3.one * this.scale;
        this.myRigidbody.mass = this.scale;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.relativeVelocity.magnitude > this.collisionDeadZone)
		{
            this.scale = this.scale - (this.collisionImpact * this.scale);
        }
    }

    public void GameStart()
	{
        isPlaying = true;
    }

    public void GameOver()
    {
        isPlaying = false;
        this.myRigidbody.simulated = false;
    }

    Rigidbody2D myRigidbody;
    bool isPlaying = true;

    bool jumpButton = false;
    float horizontalMove = 0.0f;
}
