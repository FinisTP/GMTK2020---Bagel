using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform[] backgrounds;
    private float[] parallaxScales;
    public float smoothing = 1f;

    private Transform cam;
    private Vector3 previousCamPos;

    private void Awake()
    {
        cam = Camera.main.transform;
    }
    private void Start()
    {
        previousCamPos = cam.position;
        parallaxScales = new float[backgrounds.Length];
        for (int i = 0; i <backgrounds.Length; ++i)
        {
            parallaxScales[i] = backgrounds[i].position.z * -1;
        }
    }
    private void LateUpdate()
    {
        for (int i = 0; i < backgrounds.Length; ++i)
        {
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];
            float backgroundTargetPosX = backgrounds[i].position.x * parallax;

            Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, cam.position.y, backgrounds[i].position.z);

            backgrounds[i].position = backgroundTargetPos;
        }
        previousCamPos = cam.position;
    }
}
