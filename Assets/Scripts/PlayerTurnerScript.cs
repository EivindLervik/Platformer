using UnityEngine;
using System.Collections;

public class PlayerTurnerScript : MonoBehaviour {

    public Vector3 startTurn;
    public Vector3 endTurn;
    public Vector3 startVector;
    public Vector3 endVector;

    private float xDist;
    private float yDist;
    private float zDist;
    private BoxCollider coll;
    private bool reversed;
    private Vector3 playerStartVector;

	// Use this for initialization
	void Start () {
        reversed = true;
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public Vector3 GetDirection(Vector3 position)
    {
        float index;
        float startDistance;
        float endDistance;

        if (reversed)
        {
            xDist = startVector.x - playerStartVector.x;
            yDist = startVector.y - playerStartVector.y;
            zDist = startVector.z - playerStartVector.z;

            startDistance = Vector3.Distance(endTurn, position);
            endDistance = Vector3.Distance(position, startTurn);
        }
        else
        {
            xDist = endVector.x - playerStartVector.x;
            yDist = endVector.y - playerStartVector.y;
            zDist = endVector.z - playerStartVector.z;

            startDistance = Vector3.Distance(startTurn, position);
            endDistance = Vector3.Distance(position, endTurn);
        }

        index = startDistance / (startDistance + endDistance);
        Vector3 nyVector = new Vector3(playerStartVector.x + (xDist * index), playerStartVector.y + (yDist * index), playerStartVector.z + (zDist * index));

        return nyVector;
    }

    public void Traff(Vector3 start)
    {
        playerStartVector = start;
        reversed = !reversed;
    }
}
