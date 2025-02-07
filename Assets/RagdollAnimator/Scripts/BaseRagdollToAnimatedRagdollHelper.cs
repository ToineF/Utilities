using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseRagdollToAnimatedRagdollHelper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] _ragdollJoints;
    [SerializeField] private GameObject[] _animationJoints;
    [SerializeField] private Rigidbody _hips;

    [Header("Drive")]
    [SerializeField] private float _jointSpring = 900;
    [SerializeField] private float _jointDamper = 0;
    [SerializeField] private float _hipsSpring = 900;
    [SerializeField] private float _hipsDamper = 100;

    public void ReplaceCharacterJoints()
    {
        foreach (var joint in _ragdollJoints)
        {
            CharacterToConfigurableJoint(joint);
        }

        var configurableJoint = AddConfigurableJoint(_hips.gameObject, ConfigurableJointMotion.Free);
        _hips.isKinematic = true;
        configurableJoint.angularXDrive = SetAngularDrive(configurableJoint.angularXDrive, _hipsSpring, _hipsDamper);
        configurableJoint.angularYZDrive = SetAngularDrive(configurableJoint.angularYZDrive, _hipsSpring, _hipsDamper);
        
        //EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }
    public void SetAnimations()
    {
        if (_ragdollJoints.Length != _animationJoints.Length)
        {
            Debug.LogError("Ragdolls joints and Animation joints must be the same length!");
            return;
        }

        for (int i = 0; i < _ragdollJoints.Length; i++)
        {
            SetJointAnimation(i);
        }
        
        //EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }

    private void CharacterToConfigurableJoint(GameObject go)
    {
        Rigidbody connectedBody;
        if (go.TryGetComponent(out Joint oldJoint) == false) return;

        connectedBody = oldJoint.connectedBody;
        if (oldJoint is not ConfigurableJoint) DestroyImmediate(oldJoint);

        ConfigurableJoint configurableJoint = AddConfigurableJoint(go, ConfigurableJointMotion.Locked);
        configurableJoint.connectedBody = connectedBody;
    }

    private ConfigurableJoint AddConfigurableJoint(GameObject go, ConfigurableJointMotion jointMode)
    {
        ConfigurableJoint configurableJoint; 
        if (go.TryGetComponent(out ConfigurableJoint oldConfigurableJoint)) configurableJoint = oldConfigurableJoint;
        else configurableJoint = go.AddComponent<ConfigurableJoint>();
        configurableJoint.xMotion = jointMode;
        configurableJoint.yMotion = jointMode;
        configurableJoint.zMotion = jointMode;
        configurableJoint.angularXDrive = SetAngularDrive(configurableJoint.angularXDrive, _jointSpring, _jointDamper);
        configurableJoint.angularYZDrive = SetAngularDrive(configurableJoint.angularYZDrive, _jointSpring, _jointDamper);
        PrefabUtility.RecordPrefabInstancePropertyModifications(configurableJoint);
        return configurableJoint;
    }

    private JointDrive SetAngularDrive(JointDrive angularXDrive, float positionSpring, float positionDamper)
    {
        angularXDrive.positionSpring = positionSpring;
        angularXDrive.positionDamper = positionDamper;
        return angularXDrive;
    }

    private void SetJointAnimation(int index)
    {
        var ragdollJoint = _ragdollJoints[index];
        var ragdollAnimation = ragdollJoint.GetComponent<RagdollAnimation>() ?? ragdollJoint.AddComponent<RagdollAnimation>();
        ragdollAnimation.Target = _animationJoints[index].transform;
        ragdollAnimation.Joint = _ragdollJoints[index].GetComponent<ConfigurableJoint>();
        PrefabUtility.RecordPrefabInstancePropertyModifications(ragdollAnimation);
    }
}