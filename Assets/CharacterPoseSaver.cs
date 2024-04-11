using System.Collections.Generic;
using UnityEngine;

public class CharacterPoseSaver : MonoBehaviour
{
    private Dictionary<Transform, Pose> savedPoses = new Dictionary<Transform, Pose>();

    private struct Pose
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    // Call this to save the current pose
    public void SaveCurrentPose()
    {
        savedPoses.Clear();
        SaveBonePose(transform);
    }

    // Recursively save the pose of each bone
    private void SaveBonePose(Transform bone)
    {
        if (bone == null) return;

        savedPoses[bone] = new Pose
        {
            position = bone.localPosition,
            rotation = bone.localRotation
        };

        foreach (Transform child in bone)
        {
            SaveBonePose(child);
        }
    }

    // Call this to reset to the saved pose
    public void ResetToSavedPose()
    {
        foreach (var bonePose in savedPoses)
        {
            if (bonePose.Key != null)
            {
                bonePose.Key.localPosition = bonePose.Value.position;
                bonePose.Key.localRotation = bonePose.Value.rotation;
            }
        }
    }
}
