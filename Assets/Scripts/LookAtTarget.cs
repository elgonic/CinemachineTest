using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform Target;
    void Update()
    {
        transform.LookAt(Target);
    }
}
