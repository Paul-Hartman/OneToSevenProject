using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToTarget : MonoBehaviour
{
    public Vector3 Target;
    public float RotationSpeed = 1;
    private bool m_activated = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ActivateRotation(Vector3 _target)
    {

        Target = _target;
        m_activated = true;
        

    }

    public void DeactivateRotation()
    {
        m_activated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_activated) return;
        var lookPos = Target - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * RotationSpeed);
    }
}
