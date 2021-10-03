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

    public float minVelocityActivate = 1f;

    public GameObject particlePrefab;
    public AudioSource audioSource;


    public float collisionDeadZone = 5.0f;
    public float collisionImpact = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        this.myRigidbody = this.GetComponent<Rigidbody2D>();
        this.myParticle = GameObject.Instantiate(particlePrefab);
        this.myParticleSystem = myParticle.GetComponent<ParticleSystem>();

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
            handleParticle();
        }

        if (isMoving() && !audioSource.isPlaying && this.isPlaying)
        {
            audioSource.Play();
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
        stopParticle();
    }

    void handleParticle()
    {
        float xVelocity = this.myRigidbody.velocity.x;
        myParticle.transform.position = this.transform.position;
         
        if (xVelocity > minVelocityActivate)
        {
            playParticle();
            UnityEngine.ParticleSystem.ShapeModule shape = myParticleSystem.shape;
            shape.rotation = new Vector3(-25, -90, 0);
        }
        else if (xVelocity < -minVelocityActivate)
        {
            playParticle();
            UnityEngine.ParticleSystem.ShapeModule shape = myParticleSystem.shape;
            shape.rotation = new Vector3(-25, 90, 0);
        }
        else
        {
            stopParticle();
            
        }
    }

     bool isMoving()
    {
        float xVelocity = this.myRigidbody.velocity.x;
        return xVelocity > minVelocityActivate || xVelocity < -minVelocityActivate;
    }

    void playParticle()
    {
       if (myParticleSystem.isStopped)
        {
            myParticleSystem.Play();
        }
    }
    void stopParticle()
    {
        if (myParticleSystem.isPlaying)
        {
            myParticleSystem.Stop();
        }
        
    }

    Rigidbody2D myRigidbody;
    ParticleSystem myParticleSystem;
    bool isPlaying = true;
    GameObject myParticle;

    bool jumpButton = false;
    float horizontalMove = 0.0f;
}
