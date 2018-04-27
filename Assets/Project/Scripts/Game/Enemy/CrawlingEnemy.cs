using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlingEnemy : MonoBehaviour {

    public float speed = 2f;
    public float lifetime = 5f;

    private bool movingLeft;
	// Use this for initialization
	void Start () {
        int randomIndex = Random.Range(0, transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == randomIndex);
        }
        movingLeft = transform.position.x > 0;
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Rigidbody>().velocity = new Vector3(movingLeft ? -speed : speed, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);

        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
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
