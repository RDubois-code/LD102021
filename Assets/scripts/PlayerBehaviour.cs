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

    public GameObject rollingParticlePrefab;

    public GameObject impactParticlePrefab;

    public AudioSource rollingAudioSource;
    public AudioSource impactAudioSource;

    public float audioAirTime = 0.5f;
    
    public float collisionMinImpact = 5.0f;
    public float collisionMaxImpact = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCircleCollider = GetComponent<CircleCollider2D>();

        myRollingParticle = GameObject.Instantiate(rollingParticlePrefab);
        myRollingParticleSystem = myRollingParticle.GetComponent<ParticleSystem>();

        myImpactParticle = GameObject.Instantiate(impactParticlePrefab, transform.position, transform.rotation, transform);
        myImpactParticleSystem = myImpactParticle.GetComponent<ParticleSystem>();

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
        if(isPlaying)
        {
            HandleMovement();
            HandleScaling();
            HandleRollingParticle();
            HandleAudio();

            previousVelocity = myRigidbody.velocity;
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

                ApplyCollision(impactStrength);
            }
        }
    }

    void HandleMovement()
    {
        if(IsOnGround())
        {
            myRigidbody.AddForce(Vector3.right * horizontalMove * groundControl);

            if(jumpButton)
			{
                myRigidbody.AddForce(Vector3.up * jumpForce);
                jumpButton = false;
            }
        }
        else
		{
            myRigidbody.AddForce(Vector3.right * horizontalMove * airControl);
        }
    }

    void HandleScaling()
    {
        float newScale = scale;
        if (myRigidbody.IsTouchingLayers(LayerMask.GetMask("Snow")))
        {
            newScale += snowGrowingFactor * myRigidbody.velocity.magnitude * Time.fixedDeltaTime;
        }
        else if (myRigidbody.IsTouchingLayers(LayerMask.GetMask("Grass")))
        {
            newScale -= grassMeltingFactor * myRigidbody.velocity.magnitude * Time.fixedDeltaTime;
        }

        newScale -= airMeltingFactor * Time.fixedDeltaTime;

        SetScale(newScale);
    }

    void HandleRollingParticle()
    {
        float xVelocity = myRigidbody.velocity.x;
        myRollingParticle.transform.position = transform.position + (Vector3.down * myCircleCollider.radius * scale);

        if (xVelocity > minVelocityActivate)
        {
            PlayRollingParticle();
            UnityEngine.ParticleSystem.ShapeModule shape = myRollingParticleSystem.shape;
            shape.rotation = new Vector3(-25, -90, 0);
        }
        else if (xVelocity < -minVelocityActivate)
        {
            PlayRollingParticle();
            UnityEngine.ParticleSystem.ShapeModule shape = myRollingParticleSystem.shape;
            shape.rotation = new Vector3(-25, 90, 0);
        }
        else
        {
            StopRollingParticle();
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
        scale = Mathf.Clamp(newScale, minScale, maxScale);

        transform.localScale = Vector3.one * scale;
        myRigidbody.mass = scale;
    }

    public void GameStart()
	{
        isPlaying = true;
    }

    public void GameOver(bool victory)
    {
        isPlaying = false;
        myRigidbody.simulated = false;
        StopRollingParticle();
        StopAudio();

        if(!victory)
        {
            impactAudioSource.volume = 1.0f;
            impactAudioSource.Play();

            myImpactParticleSystem.Play();

            this.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

bool IsMoving()
    {
        float xVelocity = myRigidbody.velocity.x;
        return xVelocity > minVelocityActivate || xVelocity < -minVelocityActivate;
    }

    bool IsOnGround()
	{
        return myRigidbody.IsTouchingLayers(LayerMask.GetMask("Grass", "Snow"));
    }

    void PlayRollingParticle()
    {
       if (myRollingParticleSystem.isStopped)
        {
            myRollingParticleSystem.Play();
        }
    }
    void StopRollingParticle()
    {
        if (myRollingParticleSystem.isPlaying)
        {
            myRollingParticleSystem.Stop();
        }
    }

    void StopAudio()
	{
        if (rollingAudioSource.isPlaying)
        {
            rollingAudioSource.Stop();
        }
    }

    void ApplyCollision(float impactStrength)
    {
        SetScale((1 - impactStrength) * scale);

        impactAudioSource.volume = 0.5f + (impactStrength * 0.5f);
        impactAudioSource.Play();

        myImpactParticleSystem.Play();
    }

    Rigidbody2D myRigidbody;
    CircleCollider2D myCircleCollider;

    GameObject myRollingParticle;
    ParticleSystem myRollingParticleSystem;

    GameObject myImpactParticle;
    ParticleSystem myImpactParticleSystem;

    bool isPlaying = true;

    bool jumpButton = false;
    float horizontalMove = 0.0f;

    float lastGroundContact = 0.0f;

    Vector2 previousVelocity;
}
