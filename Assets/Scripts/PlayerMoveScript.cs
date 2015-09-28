using UnityEngine;
using System.Collections;

public class PlayerMoveScript : MonoBehaviour {

    public CameraScript kamera;
    public GameObject character;
    public Vector3 overallForward;
    public Vector3 overallIn;
    public float gravity;
    public float moveForce;
    public float deacceleration;
    public float maxMoveSpeed;
    public float jumpForce;
    public float jumpHoldIndex;
    public bool useCamera;

    public bool useGravity;
    public bool canMove;
    public bool cinematic;
    public bool threeDee;

	public float climbSpeed;

	private bool canClimb;
	private string climbWay;

    private Rigidbody body;
    private CapsuleCollider coll;
    private bool grounded;
    private bool canJump;
    private bool useTurner;
    private PlayerTurnerScript turnerObject;

    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
        grounded = true;
        canJump = true;
		canClimb = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(grounded != canJump)
        {
            canJump = grounded;
        }
        if (useCamera)
        {
            Vector3 forward = kamera.GetForward();
            transform.forward = (Vector3.Project(forward, Vector3.right) + Vector3.Project(forward, Vector3.forward));
        }
	}

    void FixedUpdate(){
        if (canMove)
        {
			if(canClimb)
			{
				DoClimb();
			}else{
            	DoMovement();
			}
        }
        if (cinematic)
        {
            DoCinematicMovement();
        }
    }

	private void DoClimb(){
		if (climbWay.Equals ("Right")) {
			if (Input.GetAxis("Horizontal") > 0)
			{
				transform.Translate(transform.up*climbSpeed/100);
			}
			else if (Input.GetAxis("Horizontal") < 0)
			{
				transform.Translate(-transform.up*climbSpeed/100);
			}
		}
		else if (climbWay.Equals ("Left")) {
			if (Input.GetAxis("Horizontal") > 0)
			{
				transform.Translate(-transform.up*climbSpeed/100);
			}
			else if (Input.GetAxis("Horizontal") < 0)
			{
				transform.Translate(transform.up*climbSpeed/100);
			}
		}
		else if (climbWay.Equals ("Forward")) {
			if (Input.GetAxis("Vertical") > 0)
			{
				transform.Translate(transform.up*climbSpeed/100);
			}
			else if (Input.GetAxis("Vertical") < 0)
			{
				transform.Translate(-transform.up*climbSpeed/100);
			}
		}

		if (Input.GetButtonDown("Jump") && canJump){
			Vector3 jumpWay = transform.up;
			if(climbWay.Equals("Right"))
			{
				jumpWay -= transform.right;
			}
			else if(climbWay.Equals("Left"))
			{
				jumpWay += transform.right;
			}
			else{
				jumpWay -= transform.forward;
			}
			body.AddForce(jumpWay*jumpForce);
			canJump = false;
		}
	}

    private void DoMovement()
    {
        Vector3 force = Vector3.zero;

        if (Input.GetButtonDown("Jump") && canJump){
            force.y += jumpForce;
            canJump = false;
            grounded = false;
        }
        
        if(body.velocity.x < maxMoveSpeed && body.velocity.z < maxMoveSpeed)
        {
            if (threeDee)
            {
                if (Input.GetAxis("Vertical") > 0)
                {
                    force += transform.forward * moveForce * Time.deltaTime * 100;
                }
                if (Input.GetAxis("Vertical") < 0)
                {
                    force -= transform.forward * moveForce * Time.deltaTime * 100;
                }
            }
            if (Input.GetAxis("Horizontal") > 0)
            {
                force += transform.right * moveForce * Time.deltaTime * 100;
            }else if (Input.GetAxis("Horizontal") < 0)
            {
                force -= transform.right * moveForce * Time.deltaTime * 100;
            }
        }

        if (useGravity)
        {
            force.y -= gravity * Time.deltaTime * 100;
        }
        if (grounded)
        {

        }

        body.AddForce(force);

        float deAccX = body.velocity.x / deacceleration;
        float deAccZ = body.velocity.z / deacceleration;
        Vector3 newVel = new Vector3(deAccX, body.velocity.y, deAccZ);
        body.velocity = newVel;
    }

    private void DoCinematicMovement()
    {
        
    }

    void OnCollisionStay(Collision collision)
    {
        if(collision.collider.transform.tag == "Ground")
        {
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y-(coll.height*2/3), transform.position.z), -Vector3.Cross(overallForward, overallIn));
			RaycastHit[] hits = Physics.SphereCastAll(ray, coll.radius - 0.05f, (coll.height/3)+0.1f);
            foreach(RaycastHit hit in hits)
            {
                if (hit.transform.tag.Equals("Ground"))
                {
                    kamera.UpdateHeight(transform.position.y);
                }
            }
        }
    }

	void OnCollisionEnter(Collision collision)
	{
		if(collision.collider.transform.tag == "Ground")
		{
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y - (coll.height*2/3), transform.position.z), -Vector3.Cross(overallForward, overallIn));
            RaycastHit[] hits = Physics.SphereCastAll(ray, coll.radius - 0.05f, (coll.height / 6) + 0.1f);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.tag.Equals("Ground"))
                {
                    grounded = true;
                }
            }
        }
	}

	void OnCollisionExit(Collision collision)
	{
		if(collision.collider.transform.tag == "Ground")
		{
			grounded = false;
            
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y - (coll.height*2/3), transform.position.z), -Vector3.Cross(overallForward, overallIn));
            RaycastHit[] hits = Physics.SphereCastAll(ray, coll.radius-0.05f, 0.01f);

            print(transform.position.y);
            print(ray.origin.y);
            print((coll.height / 6));

            foreach (RaycastHit hit in hits)
            {
                print(hit.point.y);
                if (hit.transform.tag.Equals("Ground"))
                {
                    print(hit.point.y);
                    grounded = true;
                }
            }
            
        }
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Turner")
        {
            turnerObject = collider.transform.gameObject.GetComponent<PlayerTurnerScript>();
            turnerObject.Traff(transform.forward);
            useTurner = true;
		}
		if (collider.transform.tag == "Climbeable/ClimbeableRight")
		{
			climbWay = "Right";
			canClimb = true;
		}
		if (collider.transform.tag == "Climbeable/ClimbeableLeft")
		{
			climbWay = "Left";
			canClimb = true;
		}
		if (collider.transform.tag == "Climbeable/ClimbeableForward")
		{
			climbWay = "Forward";
			canClimb = true;
		}
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.transform.tag == "Turner")
        {
            useTurner = false;
		}
		if (collider.transform.tag == "Climbeable/ClimbeableRight" 
		         || collider.transform.tag == "Climbeable/ClimbeableRight" 
		         || collider.transform.tag == "Climbeable/ClimbeableRight")
		{
			canClimb = false;
		}
    }
}
