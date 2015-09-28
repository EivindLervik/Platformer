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

	private float medianScale;
	private float characterRadius;
	private float characterUsableRadius;

    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
        grounded = true;
        canJump = true;
		canClimb = false;
		medianScale = (transform.localScale.x + transform.localScale.z)/2.0f;
		characterRadius = coll.radius * medianScale;
		characterUsableRadius = characterRadius - 0.05f;
	}
	
	// Update is called once per frame
	void Update () {
        if (useCamera)
        {
            Vector3 forward = kamera.GetForward();
            transform.forward = (Vector3.Project(forward, Vector3.right) + Vector3.Project(forward, Vector3.forward));
        }
		if (!canClimb) {
			//print (canJump);
			DoJump (transform.up);
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

		if (Input.GetButton("Jump") && canJump){
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

	private void DoJump(Vector3 direction){
		if (Input.GetButtonDown("Jump") && canJump){
			canJump = false;
			grounded = false;
			body.AddForce(direction*jumpForce);
		}
	}
	private void DoBounce(Vector3 direction){
		body.AddForce(direction*jumpForce);
	}

    private void DoCinematicMovement()
    {
        
    }

    void OnCollisionStay(Collision collision)
    {
        if(collision.collider.transform.tag == "Ground")
        {
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z), -transform.up);
			RaycastHit[] hits = Physics.SphereCastAll(ray, characterUsableRadius, (transform.localScale.y / 2)+ 0.05f);
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
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z), -transform.up);
			RaycastHit[] hits = Physics.SphereCastAll(ray, characterUsableRadius, (transform.localScale.y / 2.0f));

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.tag.Equals("Ground"))
                {
                    grounded = true;
					canJump = true;
					canClimb = false;
                }
            }
        }
	}

	void OnCollisionExit(Collision collision)
	{
		if(collision.collider.transform.tag == "Ground")
		{
			grounded = false;
			canJump = false;
            
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z), -transform.up);
			RaycastHit[] hits = Physics.SphereCastAll(ray, characterUsableRadius, (transform.localScale.y / 2.0f)-characterUsableRadius);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.tag.Equals("Ground"))
                {
                    grounded = true;
					canJump = true;
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
			body.velocity = transform.right*0.1f;
		}
		if (collider.transform.tag == "Climbeable/ClimbeableLeft")
		{
			climbWay = "Left";
			canClimb = true;
			body.velocity = -transform.right*0.1f;
		}
		if (collider.transform.tag == "Climbeable/ClimbeableForward")
		{
			climbWay = "Forward";
			canClimb = true;
			body.velocity = transform.forward*0.1f;
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
			if(canClimb){
				if(canJump){
					Vector3 jumpWay = transform.up*0.5f;
					if(climbWay.Equals("Right"))
					{
						jumpWay += transform.right*0.3f;
					}
					else if(climbWay.Equals("Left"))
					{
						jumpWay -= transform.right*0.3f;
					}
					else{
						jumpWay += transform.forward*0.3f;
					}

					print ("Did jump");
					DoBounce(jumpWay);
					canJump = false;
				}

				canClimb = false;
			}
		}
    }
}
