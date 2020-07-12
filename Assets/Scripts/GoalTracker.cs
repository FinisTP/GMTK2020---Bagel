using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTracker : MonoBehaviour
{
    public Transform target;
    public Transform player;
    private void Update()
    {
        var relPos = target.position - player.position;
        var ang = Mathf.Atan2(relPos.y, relPos.x) * Mathf.Rad2Deg - 90;
        var rotation = Quaternion.AngleAxis(ang, Vector3.forward);
        transform.rotation = rotation;
    }
}
