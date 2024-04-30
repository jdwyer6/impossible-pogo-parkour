using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PogoStickController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody rb;
    private AudioManager am;

    [Header("Physics Parameters")]
    public float jumpForce = 10f;
    public float bouncePadForce = 10f;
    public float leanTorque = 50f;
    public float rocketForce = 10f;

    [Header("Bounce Controls")]
    public float minBounceHeight = 2f;
    public Transform groundCheck; 

    [Header("Rocket Parameters")]
    public GameObject rocket;
    public float fuel = 100f; 
    public Slider fuelSlider; 
    private bool rocketActive = false; 
    private IEnumerator fuelConsumptionCoroutine; 
    public Light rocketLight;

    [Header("Ragdoll")]
    public GameObject ragdoll;
    private bool canReset;

    [Header("Player State")]
    private bool isGrounded;
    private bool facingLeft = false;

    [Header("Coroutines")]
    private IEnumerator currentFlipCoroutine; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        am = FindObjectOfType<AudioManager>();
        IgnoreCollisionsWithPlayer();
        ragdoll.GetComponent<CharacterPoseSaver>().SaveCurrentPose();
        rocketLight.enabled = false;
    }

    void Update()
    {
        HandleLeaning();
        if (Input.GetKeyDown(KeyCode.R) && canReset)
        {
            Reset();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Flip();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && fuel > 0)
        {
            if (fuelConsumptionCoroutine != null) 
            {
                StopCoroutine(fuelConsumptionCoroutine);
            }
            fuelConsumptionCoroutine = DecreaseFuelOverTime();
            StartCoroutine(fuelConsumptionCoroutine);
        }

        // Stop fuel consumption when Left Shift is released
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (fuelConsumptionCoroutine != null) 
            {
                StopCoroutine(fuelConsumptionCoroutine);
            }
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && fuel > 0)
        {
            StartRocket();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || fuel <= 0)
        {
            StopRocket();
        }

    }

    void FixedUpdate()
    {
        CheckGrounded();
        if (isGrounded)
        {
            Bounce();
        }

        HandleRocket();
    }

    private void CheckGrounded()
    {
        RaycastHit hit;
        isGrounded = Physics.Raycast(groundCheck.position, -transform.up, out hit, 0.5f);
        if (isGrounded)
        {
            if (hit.collider.CompareTag("BouncePad"))
            {
                HandleBouncePad();
            }
        }
    }

    private void HandleBouncePad()
    {
        rb.AddForce(Vector3.up * (bouncePadForce - rb.velocity.y), ForceMode.VelocityChange);
        am.Play("Bounce_BouncePad");
    }

    private void Bounce()
    {
        // Only bounce if not already moving upwards
        if (rb.velocity.y < 0)
        {
            rb.AddForce(Vector3.up * (jumpForce - rb.velocity.y), ForceMode.VelocityChange);
            am.Play("Bounce");

            // Calculate the direction based on the current lean
            float zRotation = transform.localEulerAngles.z;
            zRotation = (zRotation > 180f) ? zRotation - 360f : zRotation;  // Normalize angle to -180 to 180

            float leanFactor = Mathf.Sin(Mathf.Deg2Rad * zRotation);
            Vector3 leanDirection = (zRotation > 0) ? -transform.right : transform.right;
            rb.AddForce(leanDirection * Mathf.Abs(leanFactor) * jumpForce * 0.5f, ForceMode.VelocityChange);

        }
    }

    private void HandleLeaning()
    {
        int facingDirectionVariable = facingLeft ? -1 : 1;

        if (Input.GetKey(KeyCode.A))
        {
            rb.AddRelativeTorque(Vector3.forward * leanTorque * facingDirectionVariable);  // Lean back
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.AddRelativeTorque(Vector3.back * leanTorque * facingDirectionVariable);  // Lean forward
        }
    }

    private void IgnoreCollisionsWithPlayer() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Collider[] playerColliders = player.GetComponents<Collider>();
            Collider myCollider = GetComponent<Collider>();

            if (myCollider != null)
            {
                foreach (var playerCollider in playerColliders)
                {
                    Physics.IgnoreCollision(myCollider, playerCollider);
                }
            }
        }
    }

    private void OnCollisionStay(Collision other) {
        var rotation = transform.eulerAngles;
        if (rotation.z > 60f || rotation.z < -60f) {
            canReset = true;
        }
    }

    private void Reset() {
        transform.position = new Vector3(transform.position.x, transform.position.y + 2, 0);
        transform.eulerAngles = new Vector3(0, 0, 0);
        ragdoll.GetComponent<CharacterPoseSaver>().ResetToSavedPose();
        if (facingLeft) {
            Flip();
        }
    }

    private void Flip() {
        if (currentFlipCoroutine != null) {
            StopCoroutine(currentFlipCoroutine);
        }
        currentFlipCoroutine = FlipCoroutine();
        StartCoroutine(currentFlipCoroutine);
    }

    private IEnumerator FlipCoroutine() {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if (facingLeft) {
            endRotation = Quaternion.Euler(0, 0, 0);
            rocket.transform.Rotate(0, 0, 0);
            facingLeft = false;
        } else {
            endRotation = Quaternion.Euler(0, 180, 0);
            rocket.transform.Rotate(0, 180, 0);
            facingLeft = true;
        }

        float timeElapsed = 0;
        float duration = 0.3f;

        while (timeElapsed < duration) {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation; // Ensure the final rotation is set correctly
    }

    private void HandleRocket()
    {
        // Determine the direction based on whether the player is facing left
        Vector3 nudgeDirection = facingLeft ? Vector3.left : Vector3.right;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.AddForce(nudgeDirection * rocketForce, ForceMode.Impulse);
        }
    }

    private IEnumerator DecreaseFuelOverTime()
    {
        while (fuel > 0 && Input.GetKey(KeyCode.LeftShift))
        {
            fuel -= 1f; // Adjust fuel consumption rate here
            fuelSlider.value = fuel / 100f; // Assuming your slider's max value is set to 1

            if (fuel <= 0)
            {
                // Optional: Do something when fuel is depleted
                rocketActive = false; // Stop the rocket
                yield break; // Exit the coroutine
            }

            yield return new WaitForSeconds(0.1f); // Adjust the fuel decrease interval here
        }
    }

    private void StartRocket() {
        if (!rocketActive) {
            am.Play("Rocket");
            foreach (Transform child in rocket.transform)
            {
                child.GetComponent<ParticleSystem>().Play();
            }
            rocketLight.enabled = true;
            rocketActive = true;
        }
    }

    private void StopRocket() {
        if(rocketActive) {
            am.Stop("Rocket");
            foreach (Transform child in rocket.transform)
            {
                child.GetComponent<ParticleSystem>().Stop();
            }
            rocketLight.enabled = false;
            rocketActive = false;
        }
    }

}
