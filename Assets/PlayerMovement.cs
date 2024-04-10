using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public ConfigurableJoint[] joints; // Assign the joints connecting the ragdoll to the pogo stick
    public float defaultMassScale = 1f;
    public float inactiveMassScale = 1000f; // A large value to minimize the impact

    void Start()
    {
        SetJointsMassScale(inactiveMassScale);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            // Player is actively controlling the ragdoll, allow it to affect the pogo stick
            SetJointsMassScale(defaultMassScale);
        }
        else
        {
            // No relevant input, minimize the ragdoll's effect on the pogo stick
            SetJointsMassScale(inactiveMassScale);
        }
    }

    void SetJointsMassScale(float scale)
    {
        foreach (var joint in joints)
        {
            joint.massScale = scale;
            joint.connectedMassScale = scale;
        }
    }
}
