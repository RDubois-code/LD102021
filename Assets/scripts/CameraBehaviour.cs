using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Vector3 offset;

    public float maxHorizontalLag = 1;

    public float maxSpring = 0.5f;
    public float springFactor = 3;

    public float maxShiftUp = 0.5f;
    public float maxShiftDown = 0.5f;

    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = target.position + offset;
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
        Vector3 targetPosition = target.position + offset + Vector3.up * verticalMove;

        targetPosition.x = Mathf.Clamp(this.transform.position.x, (targetPosition.x - maxHorizontalLag), (targetPosition.x + maxHorizontalLag));
        this.transform.position = new Vector3(Interp(this.transform.position.x, targetPosition.x, Time.fixedDeltaTime), Interp(this.transform.position.y, targetPosition.y, Time.fixedDeltaTime), Interp(this.transform.position.z, targetPosition.z, Time.fixedDeltaTime));
    }

    float Interp(float current, float target, float deltaTime)
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

    float verticalMove = 0.0f;
}
