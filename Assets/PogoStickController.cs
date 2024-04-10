using UnityEngine;

public class PogoStickController : MonoBehaviour
{
    public float jumpForce = 10f;
    public float leanTorque = 50f;
    public float minBounceHeight = 2f;
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleLeaning();
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
        // Simple ground check below the pogo stick
        isGrounded = Physics.Raycast(transform.position, -transform.up, 0.5f);
    }

    private void Bounce()
    {
        // Only bounce if not already moving upwards
        if (rb.velocity.y < 0)
        {
            rb.AddForce(Vector3.up * (jumpForce - rb.velocity.y), ForceMode.VelocityChange);

            // Calculate the direction based on the current lean
            float zRotation = transform.localEulerAngles.z;
            zRotation = (zRotation > 180f) ? zRotation - 360f : zRotation;  // Normalize angle to -180 to 180

            // Apply a forward or backward force based on the lean
            // Apply a forward or backward force based on the lean
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
}
