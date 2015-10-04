using UnityEngine;
using System.Collections;

public class PlayerMoveScript : MonoBehaviour {

    public CameraScript kamera;
    public PlayerAnimationScript character;
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

	public bool canBounce;

	public float climbSpeed;

	private bool canClimb;
	private string climbWay;

    private Rigidbody body;
    private CapsuleCollider coll;
    private bool grounded;
    private bool canJump;
	private bool isWalljumping;
    private bool useTurner;
    private PlayerTurnerScript turnerObject;

	private float characterRadius;
	private float characterUsableRadius;

    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
        grounded = true;
        canJump = true;
		canClimb = false;
		characterRadius = coll.radius;
		characterUsableRadius = characterRadius - 0.1f;
	}
	
	// Update is called once per frame
	void Update () {
        if (useCamera)
        {
            //Vector3 forward = kamera.GetForward();
            //transform.forward = (Vector3.Project(forward, Vector3.right) + Vector3.Project(forward, Vector3.forward));

            Vector3 newDir = new Vector3(body.velocity.x, 0, body.velocity.z);
            Vector3 lerpDir = Vector3.Lerp(character.transform.forward, newDir, 0.5f);
            if ((body.velocity.x + body.velocity.z) != 0 && body.velocity != Vector3.zero && body.velocity.magnitude > 0.2f)
            {
                character.SetForward(lerpDir);
            }
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
			isWalljumping = true;
		}

		float deAccY = body.velocity.y / 1.05f;
		Vector3 newVel = new Vector3(body.velocity.x, deAccY, body.velocity.z);
		body.velocity = newVel;
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
        if (collision.collider.transform.tag == "Ground")
        {
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + characterRadius, transform.position.z), -transform.up);
            RaycastHit[] hits = Physics.SphereCastAll(ray, characterUsableRadius, 0.05f);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.tag.Equals("Ground"))
                {
                    //print(canJump);
                    grounded = true;
                    canJump = true;
                    canClimb = false;
                    isWalljumping = false;
                    kamera.UpdateHeight(transform.position.y, canClimb);
                }
            }
        }
    }

	void OnCollisionEnter(Collision collision)
	{
		// On collision
	}

	void OnCollisionExit(Collision collision)
	{
		if(collision.collider.transform.tag == "Ground")
		{
			grounded = false;
			canJump = false;

            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + characterRadius, transform.position.z), -transform.up);
			RaycastHit[] hits = Physics.SphereCastAll(ray, characterUsableRadius, 0.05f);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.tag.Equals("Ground"))
                {
                    //print(transform.position.y - characterRadius + characterUsableRadius + 0.01f);
                    //print((ray.origin - hit.point).magnitude);
                    //print(ray.origin.y);
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
			body.velocity = (transform.right + transform.up)*0.1f;
			kamera.SetClimbing(climbWay);
		}
		if (collider.transform.tag == "Climbeable/ClimbeableLeft")
		{
			climbWay = "Left";
			canClimb = true;
			body.velocity = (-transform.right + transform.up)*0.1f;
			kamera.SetClimbing(climbWay);
		}
		if (collider.transform.tag == "Climbeable/ClimbeableForward")
		{
			climbWay = "Forward";
			canClimb = true;
			body.velocity = (transform.forward + transform.up)*0.1f;
			kamera.SetClimbing(climbWay);
		}
    }

    void OnTriggerStay(Collider collider)
    {
        if (Input.GetButton("Jump") && !grounded)
        {
            DetectWallJump(collider);
        }
    }

    private void DetectWallJump(Collider collider)
    {
        if (true)
        {
            if (collider.transform.tag == "WallJumpWall/Right")
            {
                Vector3 jumpWay = transform.up * 0.8f;
                jumpWay -= transform.right * 2.0f;

                print("Did jump");
                DoBounce(jumpWay);
            }
            if (collider.transform.tag == "WallJumpWall/Left")
            {
                Vector3 jumpWay = transform.up * 0.8f;
                jumpWay += transform.right * 2.0f;

                print("Did jump");
                DoBounce(jumpWay);
            }
            if (collider.transform.tag == "WallJumpWall/Forward")
            {

            }
            if (collider.transform.tag == "WallJumpWall/Back")
            {

            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.transform.tag == "Turner")
        {
            useTurner = false;
		}
		if (collider.transform.tag == "Climbeable/ClimbeableRight" 
		         || collider.transform.tag == "Climbeable/ClimbeableLeft" 
		         || collider.transform.tag == "Climbeable/ClimbeableForward")
		{
			if(canClimb){
				if(!isWalljumping){
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

					//print ("Did jump");
					DoBounce(jumpWay);
					canJump = false;
				}

				canClimb = false;
			}
		}
    }
}
