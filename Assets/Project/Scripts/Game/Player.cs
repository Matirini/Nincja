using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{

    [Header("General variables")]
    public float speed = 8f;
    public float movementAmplitude = 10f;
    public float jumpingAngle = 45f;
    public float jumpingForce = 10f;
    public float jetpackForce = 5f;
    public float maximumVerticalVelocity = 8f;
    public float maximumVerticalPosition = 9f;
    public GameObject model;
    public GameObject jetpack;
    public GameObject[] jetpackThrusters;
    public GameObject weaponPrefab;
    public Action onKill;

    [Header("Effects")]
    public GameObject explosionEffectPrefab;

    private Rigidbody playerRigidbody;
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector2 clickOrigin;
    private bool invincible = false;
    private bool lockZ = false;
    private bool canJump = false;
    private bool canAttack = false;
    private bool hasJetpack = false;
    private bool demonstrationMode = false;
    private bool lookingLeft = false;
    private float depthRange;
    private float horizontalRange;
    private bool isJumping;

    public bool Invincible { set { invincible = value; } }
    public bool LockZ { set { lockZ = value; } }
    public bool CanJump { set { canJump = value; } }
    public bool CanAttack { set { canAttack = value; } }
    public bool HasJetpack { set { hasJetpack = value; } }
    public bool DemonstrationMode { set { demonstrationMode = value; } }
    public float DepthRange { set { depthRange = value; } }
    public float HorizontalRange { set { horizontalRange = value; } }

    // Use this for initialization
    void Start()
    {
        clickOrigin = Vector2.zero;
        targetRotation = Quaternion.identity;
        jetpack.SetActive(hasJetpack);
        playerRigidbody = GetComponent<Rigidbody>();

        if (hasJetpack)
        {
            targetRotation = Quaternion.Euler(0, 180, 0);
        }

        if (demonstrationMode)
        {
            playerRigidbody.useGravity = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canAttack == false && hasJetpack == false)
        {
            Vector2 viewportCoordinates = new Vector2(
                                             Input.mousePosition.x / Screen.width,
                                             Input.mousePosition.y / Screen.height
                                         );

            // Clicking or touching logic
            if (isJumping == false && Input.GetMouseButton(0))
            {
                if (clickOrigin == Vector2.zero)
                {
                    originalPosition = transform.position;
                    clickOrigin = viewportCoordinates;
                }
                else
                {
                    Vector2 variation = viewportCoordinates - clickOrigin;

                    // Set the player's target position.
                    targetPosition = new Vector3(
                        originalPosition.x + variation.x * movementAmplitude,
                        transform.position.y,
                        lockZ ? transform.position.z : originalPosition.z + variation.y * movementAmplitude
                    );

                    lookingLeft = targetPosition.x < transform.position.x;
                }
            }
            else
            {
                if (clickOrigin != Vector2.zero)
                {
                    if (canJump)
                    {
                        isJumping = true;

                        GetComponent<Rigidbody>().AddForce(
                            Mathf.Cos(jumpingAngle * Mathf.Deg2Rad) * jumpingForce * (lookingLeft ? -1 : 1),
                            Mathf.Sin(jumpingAngle * Mathf.Deg2Rad) * jumpingForce,
                            0
                        );
                    }
                }

                clickOrigin = Vector2.zero;
            }

            // Movement logic.
            if (isJumping == false)
            {
                Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
                transform.position = new Vector3(smoothPosition.x, transform.position.y, smoothPosition.z);
            }

            targetRotation = Quaternion.Euler(0, (lookingLeft ? -90 : 90), 0);
        }

        // Rotate the player's model.
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

        // Jetpack activation logic.
        if (hasJetpack)
        {
            // Add force if the player is touching the screen.
            if (Input.GetMouseButton(0) && demonstrationMode == false)
            {
                playerRigidbody.AddForce(0, jetpackForce, 0);
            }

            // Keep the player's velocity within limits.
            if (playerRigidbody.velocity.y > maximumVerticalVelocity)
            {
                playerRigidbody.velocity = new Vector3(
                    playerRigidbody.velocity.x,
                    maximumVerticalVelocity,
                    playerRigidbody.velocity.z
                );
            }

            // Adjust thrusters' visibility.
            foreach (GameObject thruster in jetpackThrusters)
            {
                thruster.SetActive(Input.GetMouseButton(0) || demonstrationMode);
            }
        }

        // Keep the player within depth range.
        if (transform.position.z > depthRange)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, depthRange);
        }
        else if (transform.position.z < -depthRange)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -depthRange);
        }

        // Keep the player within horizontal range.
        if (transform.position.x > horizontalRange)
        {
            transform.position = new Vector3(horizontalRange, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -horizontalRange)
        {
            transform.position = new Vector3(-horizontalRange, transform.position.y, transform.position.z);
        }

        // Keep the player within vertical range.
        if (transform.position.y > maximumVerticalPosition)
        {
            transform.position = new Vector3(transform.position.x, maximumVerticalPosition, transform.position.z);
            playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
        }
    }

    public void Kill(bool shouldExplode = false)
    {
        if (invincible)
        {
            return;
        }

        if (shouldExplode)
        {
            GameObject explosionEffect = Instantiate(explosionEffectPrefab);
            explosionEffect.transform.parent = transform.parent;
            explosionEffect.transform.position = transform.position;
        }

        if (onKill != null)
        {
            onKill();
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Floor")
        {
            isJumping = false;
            targetPosition = new Vector3(transform.position.x, targetPosition.y, targetPosition.z);
        }
    }

    public void ThrowWeapon(Vector3 referencePosition)
    {
        Vector3 targetPosition = new Vector3(referencePosition.x, transform.position.y, referencePosition.z);

        targetRotation = Quaternion.LookRotation(transform.position - targetPosition);

        GameObject weaponObject = Instantiate(weaponPrefab);
        weaponObject.transform.position = transform.position;
        weaponObject.transform.forward = transform.position - targetPosition;
    }
}