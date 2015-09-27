﻿using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

    public GameObject target;
    public Vector3 offset;
    public float cameraHeight;
    public bool lookAtPlayer;

    private bool move;
    private float targetHeight;
    private float maxDistance;

	// Use this for initialization
	void Start () {
        maxDistance = 7.0f;
	}
	
	// Update is called once per frame
	void Update () {
        if (move)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, offset.y, transform.position.z), 0.05f);
        }
	}

    void LateUpdate()
    {
        if (Mathf.Abs((target.transform.position - transform.position).magnitude) > offset.magnitude)
        {
            Vector3 desiredPosition = new Vector3(target.transform.position.x + offset.x, transform.position.y, target.transform.position.z + offset.z);
            transform.position = desiredPosition;
            print(Mathf.Abs((target.transform.position - transform.position).magnitude));
            print(maxDistance);
        }
        if (lookAtPlayer)
        {
            transform.LookAt(target.transform);
        }
    }

    public void UpdateHeight(float height)
    {
        move = true;
        offset.y = cameraHeight + height;
    }

    public Vector3 GetForward()
    {
        return transform.forward;
    }
}
