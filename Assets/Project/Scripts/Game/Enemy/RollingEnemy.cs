using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingEnemy : MonoBehaviour {

    public float speed;
    public float waitingDuration = 2f;
    public float[] depthPercentages;

    private float depthRange;
    private float horizontalRange;
    private bool movingLeft;
    private float waitingTimer;
    private bool frozen;

    public float DepthRange { set { depthRange = value; } }
	public float HorizontalRange { set { horizontalRange = value; } }
    // Use this for initialization
	void Start () {
        transform.position = new Vector3(Random.value > 0.5f ? horizontalRange : -horizontalRange, transform.position.y, transform.position.z);
        movingLeft =transform.position.x >0;
        SetValues();
	}
	
	// Update is called once per frame
	void Update () {

        if (waitingTimer> 0 || frozen)
        {
            waitingTimer -= Time.deltaTime;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else
        {
            GetComponent<Rigidbody>().velocity = new Vector3(movingLeft ? -speed : speed, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
        }
        
        if((movingLeft && transform.position.x < -horizontalRange)||
        (! movingLeft && transform.position.x > horizontalRange))
        {
            movingLeft = !movingLeft;
            SetValues();

        }
	}
    private void SetValues()
    {
        waitingTimer = waitingDuration;

        transform.position = new Vector3(transform.position.x, transform.position.y,((depthRange + depthRange) * depthPercentages[Random.Range(0, depthPercentages.Length)])- depthRange);
    }
    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.transform.GetComponent<Player>() != null)
        {
            frozen = true;
            collision.transform.GetComponent<Player>().Kill();
        }
    }
}
