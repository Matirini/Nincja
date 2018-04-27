using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public GameObject model;
    public float speed =10f;
    public float rotatingSpeed = 180f;
    public float lifetime = 2f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.forward * speed * Time.deltaTime;
        lifetime -= Time.deltaTime;
        if(lifetime <= 0)
        {
            Destroy(gameObject);
        }

        model.transform.Rotate(Vector3.up * rotatingSpeed * Time.deltaTime);
    }
    void OnTriggerEnter(Collider otherCollider)
    {

        if (otherCollider.transform.GetComponent<Player>() != null)
        {
            otherCollider.transform.GetComponent<Player>().Kill();
            Destroy(gameObject);
        }
    }
}
