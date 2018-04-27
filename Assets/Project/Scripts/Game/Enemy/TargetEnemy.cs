using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetEnemy : MonoBehaviour {

    public GameObject bulletPrefab;
    public float scalingSpeed = 10f;

    private Vector3 targetScale;
    private float shootingDuration =1f;

    private float shootingTimer;
    private bool shot;

    public float ShootingDuration { set { shootingDuration = value; } }

	// Use this for initialization
	void Start () {
        transform.localScale = Vector3.zero;
        targetScale = Vector3.one;
        shootingTimer = shootingDuration;
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime* scalingSpeed);

        shootingTimer -= Time.deltaTime;
        if(shootingTimer <=0 && shot == false)
        {
            shot = true;
            
            GameObject playerObject = GameObject.Find("Player");
            if (playerObject != null)
            {
                GameObject bulletObject = Instantiate(bulletPrefab);
                bulletObject.transform.position = transform.position;
                bulletObject.transform.forward = playerObject.transform.position - transform.position;
            }
        }
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.GetComponent<PlayerWeapon>() != null)
        {
            Destroy(gameObject);
            Destroy(otherCollider.gameObject);
        }
    }
}
