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

    public AudioSource rollingAudioSource;
    public AudioSource impactAudioSource;

    public float audioAirTime = 0.5f;
    
    public float collisionMinImpact = 5.0f;
    public float collisionMaxImpact = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.myRigidbody = this.GetComponent<Rigidbody2D>();
        this.myParticle = GameObject.Instantiate(particlePrefab);
        this.myParticleSystem = myParticle.GetComponent<ParticleSystem>();

        previousVelocity = new Vector2(0, 0);
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
            HandleParticle();
            HandleAudio();

            previousVelocity = this.myRigidbody.velocity;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D[] contactPoints = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contactPoints);

        Vector2 collisionNormal = new Vector2(0, 0);
        foreach (ContactPoint2D contact in contactPoints)
        {
            collisionNormal = collisionNormal + contact.normal;
        }

        if (collisionNormal.SqrMagnitude() > Mathf.Epsilon)
        {
            collisionNormal.Normalize();
            float impactSpeed = -Vector2.Dot(collisionNormal, previousVelocity);
            if (impactSpeed > collisionMinImpact)
            {
                float impactStrength = Mathf.InverseLerp(collisionMinImpact, collisionMaxImpact, impactSpeed);

                SetScale((1 - impactStrength) * this.scale);
                impactAudioSource.volume = 0.5f + (impactStrength * 0.5f);
                impactAudioSource.Play();
            }
        }
    }

    void HandleMovement()
    {
        if(IsOnGround())
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
        float newScale = this.scale;
        if (this.myRigidbody.IsTouchingLayers(LayerMask.GetMask("Snow")))
        {
            newScale += this.snowGrowingFactor * this.myRigidbody.velocity.magnitude * Time.fixedDeltaTime;
        }
        else if (this.myRigidbody.IsTouchingLayers(LayerMask.GetMask("Grass")))
        {
            newScale -= this.grassMeltingFactor * this.myRigidbody.velocity.magnitude * Time.fixedDeltaTime;
        }

        newScale -= this.airMeltingFactor * Time.fixedDeltaTime;

        SetScale(newScale);
    }

    void HandleParticle()
    {
        float xVelocity = this.myRigidbody.velocity.x;
        myParticle.transform.position = this.transform.position;

        if (xVelocity > minVelocityActivate)
        {
            PlayParticle();
            UnityEngine.ParticleSystem.ShapeModule shape = myParticleSystem.shape;
            shape.rotation = new Vector3(-25, -90, 0);
        }
        else if (xVelocity < -minVelocityActivate)
        {
            PlayParticle();
            UnityEngine.ParticleSystem.ShapeModule shape = myParticleSystem.shape;
            shape.rotation = new Vector3(-25, 90, 0);
        }
        else
        {
            StopParticle();
        }
    }

    void HandleAudio()
    {
        if (IsOnGround())
        {
            lastGroundContact = Time.fixedTime;
        }

        bool audioInAir = ((Time.fixedTime - lastGroundContact) >= audioAirTime);

        if (IsMoving() && !audioInAir)
        {
            if (!rollingAudioSource.isPlaying)
            {
                rollingAudioSource.Play();
            }

            rollingAudioSource.mute = false;
        }
        else
        {
            rollingAudioSource.mute = true;
        }
    }

    void SetScale(float newScale)
    {
        this.scale = Mathf.Clamp(newScale, this.minScale, this.maxScale);

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
        StopParticle();
        StopAudio();
    }

bool IsMoving()
    {
        float xVelocity = this.myRigidbody.velocity.x;
        return xVelocity > minVelocityActivate || xVelocity < -minVelocityActivate;
    }

    bool IsOnGround()
	{
        return this.myRigidbody.IsTouchingLayers(LayerMask.GetMask("Grass", "Snow"));
    }

    void PlayParticle()
    {
       if (myParticleSystem.isStopped)
        {
            myParticleSystem.Play();
        }
    }
    void StopParticle()
    {
        if (myParticleSystem.isPlaying)
        {
            myParticleSystem.Stop();
        }
    }

    void StopAudio()
	{
        if (rollingAudioSource.isPlaying)
        {
            rollingAudioSource.Stop();
        }
    }

    Rigidbody2D myRigidbody;
    ParticleSystem myParticleSystem;
    GameObject myParticle;

    bool isPlaying = true;

    bool jumpButton = false;
    float horizontalMove = 0.0f;

    float lastGroundContact = 0.0f;

    Vector2 previousVelocity;
}
