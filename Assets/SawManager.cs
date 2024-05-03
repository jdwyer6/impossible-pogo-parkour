using UnityEngine;

public class SawManager : MonoBehaviour
{
    public GameObject deadRagdoll;  // Prefab for the dead ragdoll
    public GameObject pogoStick;    // Prefab for the pogo stick
    public float forceMagnitude = 10f; // Public variable to control the magnitude of force applied

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Saw") {
            Debug.Log("Saw collision detected!");
            // Instantiate ragdoll and pogo stick at the collision point
            GameObject ragdollInstance = Instantiate(deadRagdoll, transform.position, transform.rotation);
            GameObject pogoStickInstance = Instantiate(pogoStick, transform.position, transform.rotation);

            // Apply random force
            ApplyRandomForce(ragdollInstance);
            ApplyRandomForce(pogoStickInstance);
        }
    }

    private void ApplyRandomForce(GameObject obj) {
        // Get the Rigidbody component
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (rb != null) {
            // Generate a random direction in 3D space
            Vector3 forceDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            forceDirection.Normalize(); // Normalize to ensure the force magnitude does not change

            // Apply the force
            rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
        }
    }
}
