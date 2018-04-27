using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretEnemy : MonoBehaviour {

    public enum TurretState
    {
        Waiting,
        Moving,
        Aiming
    }

    public GameObject bulletPrefab;
    public float movementSpeed;
    public float aimingSpeed;

    private TurretState state;
    private float horizontalRange;
    private float waitingDuration;
    private float movingDuration;
    private float aimingDuration;

    private float stateTimer;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

	public float HorizontalRange { set { horizontalRange = value; } }
    public float WaitingDuration { set { waitingDuration = value; } }
    public float MovingDuration { set { movingDuration = value; } }
    public float AimingDuration { set { aimingDuration = value; } }


    // Use this for initialization
    void Start () {
        state = TurretState.Waiting;
        stateTimer = waitingDuration;
        targetPosition = transform.position;
        targetRotation = Quaternion.Euler(Vector3.down);
	}
	
	// Update is called once per frame
	void Update () {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            switch (state)
            {
                case TurretState.Waiting:
                    state = TurretState.Moving;
                    stateTimer = movingDuration;

                    targetPosition = new Vector3((transform.position.x>0) ? Random.Range(horizontalRange*0.4f,horizontalRange*0.9f):Random.Range(-horizontalRange*0.4f,-horizontalRange*0.9f), transform.position.y, transform.position.z);
                    
                    break;

                case TurretState.Moving:
                    state = TurretState.Aiming;
                    stateTimer = aimingDuration;

                    Vector3 positionToShootAt = new Vector3(Random.Range(-horizontalRange, horizontalRange), 1, 0);
                    targetRotation = Quaternion.LookRotation(positionToShootAt - transform.position);

                    break;
                case TurretState.Aiming:
                    state = TurretState.Waiting;
                    stateTimer = waitingDuration;
                    GameObject bulletObcject = Instantiate(bulletPrefab);
                    bulletObcject.transform.position = transform.position;
                    bulletObcject.transform.forward = transform.forward;
                    break;
            }
        }
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movementSpeed);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * aimingSpeed);
    }
}
