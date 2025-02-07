using UnityEngine;

public class RagdollBalancer : MonoBehaviour
{
    [SerializeField] private Transform _body;

    private void Update()
    {
        transform.position = _body.position;
    }
}
