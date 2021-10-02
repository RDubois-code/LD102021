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

    // Start is called before the first frame update
    void Start()
    {
        this.myRigidbody = this.GetComponent<Rigidbody2D>();
    }

    // FixedUpdate is called at a fixed rate
    void FixedUpdate()
    {
        if(this.isPlaying)
        {
            HandleMovement();
            HandleScaling();
        }
        if (Input.GetButtonDown("Cancel"))
        {
            SceneManager.LoadScene("Menu");
        }
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
            this.scale -= this.grassMeltingFactor * this.myRigidbody.velocity.magnitude * Time.deltaTime;
        }
        else if (this.myRigidbody.IsTouchingLayers(LayerMask.GetMask("Snow")))
        {
            this.scale += this.snowGrowingFactor * this.myRigidbody.velocity.magnitude * Time.deltaTime;
        }

        this.scale -= this.airMeltingFactor * Time.deltaTime;

        this.scale = Mathf.Clamp(this.scale, this.minScale, this.maxScale);

        this.transform.localScale = Vector3.one * this.scale;
        this.myRigidbody.mass = this.scale;
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
}
