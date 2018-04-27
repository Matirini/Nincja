using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingEnemy : MonoBehaviour {

    public float jumpingForce = 5f;
    public float speed = 3f;

    private bool movingDown;
    private float targetHorizontalPosition;
    private float horizontalRange;

    public float HorizontalRange { set { horizontalRange = value; } }

    // Use this for initialization
	void Start () {
        movingDown = true;
        targetHorizontalPosition = Random.Range(-horizontalRange, horizontalRange);
    }
	
	// Update is called once per frame
	void Update () {
		if(movingDown == false)
        {
            if (GetComponent<Rigidbody>().velocity.y < 0f)
            {
                movingDown = true;

                targetHorizontalPosition = Random.Range(-horizontalRange, horizontalRange);
            }
        }
        Vector3 targetPosition = new Vector3(targetHorizontalPosition, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
	}
    void OnCollisionEnter(Collision collision)
    {
        movingDown = false;
        GetComponent<Rigidbody>().AddForce(0, jumpingForce, 0);
        if (collision.transform.GetComponent<Player>()!= null)
        {
            collision.transform.GetComponent<Player>().Kill();
        }
    }
}
