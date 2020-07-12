using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTracker : MonoBehaviour
{
    public Transform target;
    private void Update()
    {
        var relPos = target.position - transform.position;
        var ang = Mathf.Atan2(relPos.y, relPos.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.AngleAxis(ang, Vector3.forward);
        transform.rotation = rotation;
    }
}
