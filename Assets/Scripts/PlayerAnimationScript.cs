using UnityEngine;
using System.Collections;

public class PlayerAnimationScript : MonoBehaviour {

    public Material leftEyeMat;
    public Material rightEyeMat;
    public GameObject leftEye;
    public GameObject rightEye;
    public GameObject kamera;

    public Vector3 lookPosition;

    private Vector2 leftTargetOffset;
    private Vector2 rightTargetOffset;
    private float lookSpeed;
    private Animator animator;

    // Use this for initialization
    void Start () {
        lookSpeed = 0.1f;
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        //EyesTest();
        EyesUpdate();
        if (Input.GetKeyDown(KeyCode.V))
        {
            animator.SetBool("Waiting", true);
        }
    }

    private void EyesUpdate()
    {
        leftEyeMat.SetTextureOffset("_MainTex", Vector2.Lerp(leftEyeMat.GetTextureOffset("_MainTex"), leftTargetOffset, lookSpeed));
        rightEyeMat.SetTextureOffset("_MainTex", Vector2.Lerp(rightEyeMat.GetTextureOffset("_MainTex"), rightTargetOffset, lookSpeed));
    }

    private void Look(string direction)
    {
        if (direction.Equals("forward"))
        {
            LookInDirection(transform.forward+lookPosition);
        }
        else if (direction.Equals("right"))
        {
            LookInDirection(-transform.right);
        }
        else if (direction.Equals("left"))
        {
            LookInDirection(transform.right);
        }
        else if (direction.Equals("up"))
        {
            LookInDirection(transform.up);
        }
        else if (direction.Equals("camera"))
        {
            LookInDirection(kamera.transform.position - transform.position);
            //print(kamera.transform.position - transform.position);
        }
    }

    private void LookInDirection(Vector3 direction, float lookSpeed)
    {
        this.lookSpeed = lookSpeed;
        LookInDirection(direction);
    }

    private void LookInDirection(Vector3 direction)
    {
        LookAtPosition(transform.position+direction);
    }

    private void LookAtPosition(Vector3 position, float lookSpeed)
    {
        this.lookSpeed = lookSpeed;
        LookAtPosition(position);
    }

    private void LookAtPosition(Vector3 position)
    {
        Vector3 lEyePos = leftEye.transform.position;
        Vector3 rEyePos = rightEye.transform.position;

        Vector3 lRetningsvektor = (position - lEyePos).normalized;
        Vector3 rRetningsvektor = (position - rEyePos).normalized;

        leftTargetOffset = new Vector2(lRetningsvektor.x/4, lRetningsvektor.y/4);
        rightTargetOffset = new Vector2(rRetningsvektor.x/4, rRetningsvektor.y/4);
    }

    public void SetForward(Vector3 forward)
    {
        if((forward.x + forward.z) != 0 && forward != Vector3.zero)
        {
            transform.forward = new Vector3(forward.x, 0, forward.z);
        }
    }































    private void EyesTest()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            rightTargetOffset = new Vector2(rightTargetOffset.x, rightTargetOffset.y + 0.1f);

        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            rightTargetOffset = new Vector2(rightTargetOffset.x - 0.1f, rightTargetOffset.y);

        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            rightTargetOffset = new Vector2(rightTargetOffset.x, rightTargetOffset.y - 0.1f);

        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            rightTargetOffset = new Vector2(rightTargetOffset.x + 0.1f, rightTargetOffset.y);

        }



        if (Input.GetKeyDown(KeyCode.I))
        {
            leftTargetOffset = new Vector2(leftTargetOffset.x, leftTargetOffset.y + 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            leftTargetOffset = new Vector2(leftTargetOffset.x - 0.1f, leftTargetOffset.y);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            leftTargetOffset = new Vector2(leftTargetOffset.x, leftTargetOffset.y - 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            leftTargetOffset = new Vector2(leftTargetOffset.x + 0.1f, leftTargetOffset.y);
        }



        if (Input.GetKeyDown(KeyCode.Z))
        {
            leftTargetOffset = new Vector2(leftTargetOffset.x + 3.0f, leftTargetOffset.y);
            rightTargetOffset = new Vector2(rightTargetOffset.x + 3.0f, rightTargetOffset.y);
        }
    }
}
