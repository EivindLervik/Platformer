using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

    public GameObject target;
    public Vector3 offset;
    public float cameraHeight;
    public bool lookAtPlayer;
    public bool movable;

    private bool move;
    private Vector3 targetPosition;

	// Use this for initialization
	void Start () {
		move = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (move)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.05f);
        }
	}

    void LateUpdate()
    {
        if (movable)
        {
            Vector3 desiredPosition = new Vector3(target.transform.position.x + offset.x, transform.position.y, target.transform.position.z + offset.z);
            transform.position = desiredPosition;
        }
        if (lookAtPlayer)
        {
            transform.LookAt(target.transform);
        }
    }

    public void UpdateHeight(float height, bool climbing)
    {
		if (!climbing) {
			offset.y = cameraHeight + height;
			targetPosition = offset;
		}
    }

	public void SetClimbing(string direction)
	{
		Vector3 newPos = target.transform.position;
		newPos.y += 1.0f;
		newPos.z -= 2.0f;

		if (direction.Equals ("Right")) 
		{
			newPos.x -= 1.0f;
		} else if (direction.Equals ("Left")) 
		{
			newPos.x += 1.0f;
		}

		targetPosition = newPos;
	}

    public Vector3 GetForward()
    {
        return transform.forward;
    }
}
