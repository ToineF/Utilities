using UnityEngine;

public class RagdollAnimation : MonoBehaviour
{
    [field:SerializeField] public Transform Target {get; set;}
    [field:SerializeField] public ConfigurableJoint Joint {get; set;}

    private Quaternion _startRotation;

    private void Start()
    {
        _startRotation = transform.localRotation;
    }

    private void Update()
    {
        Joint.SetTargetRotationLocal(Target.localRotation, _startRotation);
    }
}
