using UnityEngine;

public class PogoStickController : MonoBehaviour
{
    public float jumpForce = 10f;
    public float bouncePadForce = 10f;
    public float leanTorque = 50f;
    public float minBounceHeight = 2f;
    private Rigidbody rb;
    private bool isGrounded;
    public Transform groundCheck;
    private AudioManager am;
    public GameObject ragdoll;
    private bool canReset;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        am = FindObjectOfType<AudioManager>();
        IgnoreCollisionsWithPlayer();
        ragdoll.GetComponent<CharacterPoseSaver>().SaveCurrentPose();
    }

    void Update()
    {
        HandleLeaning();
        if (Input.GetKeyDown(KeyCode.R) && canReset)
        {
            Reset();
        }
    }

    void FixedUpdate()
    {
        CheckGrounded();
        if (isGrounded)
        {
            Bounce();
        }
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
        am.Play("Bounce");
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
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddRelativeTorque(Vector3.forward * leanTorque);  // Lean back
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.AddRelativeTorque(Vector3.back * leanTorque);  // Lean forward
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
        // ragdoll.SetActive(false);
        transform.position = new Vector3(transform.position.x, transform.position.y + 2, 0);
        transform.eulerAngles = new Vector3(0, 0, 0);
        // ragdoll.SetActive(true);
        ragdoll.GetComponent<CharacterPoseSaver>().ResetToSavedPose();
    }
}
