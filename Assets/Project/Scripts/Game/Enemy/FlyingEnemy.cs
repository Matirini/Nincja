using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour {

    public GameObject model;
    public float rotatingSpeed = 200f;
    public float movingSpeed = 5f;
    public float lifetime = 8f;

    private float initialVerticalPosition;
    private float rotatingDirection = 1;
    private float movingDirection = 1;
    // Use this for initialization
    void Start() {
        rotatingDirection = Random.value > 0.5f ? 1 : -1;
        movingDirection = transform.position.x > 0 ? -1 : 1;
        initialVerticalPosition = transform.position.y;
    }
	// Update is called once per frame
	void Update () {
        //Move the enemy.
        transform.position += Vector3.right * movingSpeed * movingDirection * Time.deltaTime;

        // Make the enemy fly in  wave pattern.
        transform.position = new Vector3(transform.position.x,initialVerticalPosition + Mathf.Cos(transform.position.x) , transform.position.z);

        //Rotate the model.
        model.transform.Rotate(Vector3.up * rotatingSpeed * Time.deltaTime);

        // Make the lifetime destroy the enemy.
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider otherCollider)
    {
        if(otherCollider.GetComponent<Player>()!= null)
        {
            otherCollider.GetComponent<Player>().Kill();
        }
    }
}
