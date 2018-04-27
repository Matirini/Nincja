using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingEnemy : MonoBehaviour
{

    public float speed;
    public float baseAngle = 60f;
    public float variableAngle = 15f;

    private float depthRange;
    private float horizontalRange;
    private bool movingLeft;
    private bool movingUp;
    private float angle;
    private Vector3 targetVelocity;

    public float DepthRange { set { depthRange = value; } }
    public float HorizontalRange { set { horizontalRange = value; } }

    // Use this for initialization
    void Start()
    {
        movingLeft = transform.position.x > 0;
        movingUp = Random.value > 0.5f;

        float targetAngle = baseAngle + Random.Range(0, variableAngle);
        if (movingLeft)
        {
            if (movingUp)
            {
                angle = 90 + targetAngle;
            }
            else
            {
                angle = 270 - targetAngle;
            }
        }
        else
        {
            if (movingUp)
            {
                angle = 90 - targetAngle;
            }
            else
            {
                angle = 270 + targetAngle;
            }
        }

        // Initial velocity.
        targetVelocity = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad) * speed,
            GetComponent<Rigidbody>().velocity.y,
            Mathf.Sin(angle * Mathf.Deg2Rad) * speed
        );
    }

    // Update is called once per frame
    void Update()
    {
        // Make the enemy move.
        Rigidbody enemyRigidbody = GetComponent<Rigidbody>();
        enemyRigidbody.velocity = new Vector3(targetVelocity.x, enemyRigidbody.velocity.y, targetVelocity.z);

        // Check for direction change.
        if (movingLeft && transform.position.x < -horizontalRange)
        {
            movingLeft = !movingLeft;
            transform.position = new Vector3(-horizontalRange, transform.position.y, transform.position.z);
            targetVelocity = new Vector3(-targetVelocity.x, enemyRigidbody.velocity.y, targetVelocity.z);
        }
        else if (!movingLeft && transform.position.x > horizontalRange)
        {
            movingLeft = !movingLeft;
            transform.position = new Vector3(horizontalRange, transform.position.y, transform.position.z);
            targetVelocity = new Vector3(-targetVelocity.x, enemyRigidbody.velocity.y, targetVelocity.z);
        }

        if (movingUp && transform.position.z > depthRange)
        {
            movingUp = !movingUp;
            transform.position = new Vector3(transform.position.x, transform.position.y, depthRange);
            targetVelocity = new Vector3(targetVelocity.x, enemyRigidbody.velocity.y, -targetVelocity.z);
        }
        else if (!movingUp && transform.position.z < -depthRange)
        {
            movingUp = !movingUp;
            transform.position = new Vector3(transform.position.x, transform.position.y, -depthRange);
            targetVelocity = new Vector3(targetVelocity.x, enemyRigidbody.velocity.y, -targetVelocity.z);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<Player>() != null)
        {
            collision.transform.GetComponent<Player>().Kill();
        }
    }
}