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
            print(Vector3.Project(forward, Vector3.right) + Vector3.Project(forward, Vector3.forward));
        }
	}

    void FixedUpdate(){
        if (canMove)
        {
            DoMovement();
        }
        if (cinematic)
        {
            DoCinematicMovement();
        }
    }

    private void DoMovement()
    {
        Vector3 force = Vector3.zero;

        if (Input.GetButton("Jump") && canJump){
            force.y += jumpForce;
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

    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.transform.tag == "Ground")
        {
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y-coll.height/2, transform.position.z), -Vector3.Cross(overallForward, overallIn));
            RaycastHit[] hits = Physics.SphereCastAll(ray, coll.radius, 0.05f);
            foreach(RaycastHit hit in hits)
            {
                if (hit.transform.tag.Equals("Ground"))
                {
                    grounded = true;
                    kamera.UpdateHeight(transform.position.y);
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
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.transform.tag == "Turner")
        {
            useTurner = false;
        }
    }
}
