using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Vector3 offset;

    public float maxShiftUp = 2.0f;
    public float maxShiftDown = 2.0f;

    public float maxSpring = 4.0f;
    public float springFactor = 3.0f;

    public float maxProjection = 4.0f;
    public float maxProjectedVelocity = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        targetRigidBody = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        transform.position = targetRigidBody.transform.position + offset;
    }

    // Update is called once per frame
    void Update()
    {
        float verticalAxis = Input.GetAxis("Vertical");
        verticalMove = (verticalAxis > 0) ? Mathf.Lerp(0, maxShiftUp, verticalAxis) : Mathf.Lerp(0, -maxShiftDown, -verticalAxis);
    }

    // FixedUpdate is called at a fixed rate
    void FixedUpdate()
    {
        Vector3 targetPosition = targetRigidBody.transform.position + offset;

        if ((maxProjection > 0) && (maxProjectedVelocity > 0))
        {
            Vector2 clampedVelocity = (targetRigidBody.velocity.magnitude > maxProjectedVelocity) ? (targetRigidBody.velocity * (maxProjection / targetRigidBody.velocity.magnitude)) : targetRigidBody.velocity;
            Vector3 desiredProjection = new Vector3((clampedVelocity.x * (maxProjection / maxProjectedVelocity)), (clampedVelocity.y * (maxProjection / maxProjectedVelocity)), 0);
            projection = SpringInter(projection, desiredProjection, Time.fixedDeltaTime);
        }

        Vector3 desiredPosition = targetPosition + projection + (Vector3.up * verticalMove);

        transform.position = SpringInter(transform.position, desiredPosition, Time.fixedDeltaTime);
    }

    float SpringInter(float current, float target, float deltaTime)
	{
        if((deltaTime > 0) && !Mathf.Approximately((current - target), 0) && (springFactor > 0))
        {
            float dist = (target - current);
            float sqrDist = dist * dist;
            float interpolatedValue = current + Mathf.Lerp(0, sqrDist, (deltaTime * springFactor)) / dist;

            return Mathf.Clamp(interpolatedValue, (target - maxSpring), (target + maxSpring));
        }
        else
		{
            return target;
        }
    }
    Vector3 SpringInter(Vector3 current, Vector3 target, float deltaTime)
    {
        return new Vector3(SpringInter(current.x, target.x, deltaTime), SpringInter(current.y, target.y, deltaTime), SpringInter(current.z, target.z, deltaTime));
    }

    public Rigidbody2D targetRigidBody;

    Vector3 projection;

    float verticalMove = 0.0f;
}
