using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed, rollForce;
	float vert, horiz;
	Vector3 velocity = Vector3.zero;
	Vector3 forceAcc = Vector3.zero;
	float mass = 1f;
	public GameObject placementIndicator, playerSprite;
	public Transform hitDetect;

	Vector3 lookAt;
	Vector3 rollDir;
	bool isRolling, rechargeStam;
	bool lockedOn;

	float currHealth = 100f, maxHealth = 100f;
	float currStam = 100f, maxStam = 100f;

	public Texture healthOver, healthUnder, stamOver, stamUnder;

	private Transform _transform;
	private CharacterController _controller;

	private void Awake()
	{
		_controller = GetComponent<CharacterController>();
		_transform = this.transform;
	}

	void Start () 
	{
		rollDir = Vector3.right;
	}

	void Update () 
	{

		Debug.Log(rigidbody.velocity.magnitude);
		vert = Input.GetAxisRaw ("Vertical");
		horiz = Input.GetAxisRaw ("Horizontal");
		//rollDir = transform.TransformDirection(new Vector3(transform.position.x, 0f, transform.position.z)).normalized; //direction of movement or facing direction?
		//ForceIntegrator();
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 100f) && hit.normal.y > 0.999)
		{
			placementIndicator.transform.position = hit.point;
			lookAt = hit.point  + new Vector3(0f, 0.51f, 0f);
		}
		if(lookAt != null)
		{
			playerSprite.transform.LookAt(lookAt);
		}
		if(!isRolling)
		{
			if(Input.GetKey (KeyCode.D) || Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("HorizDPad") > 0)
			{
				_controller.Move (Vector3.right * speed * Time.deltaTime);
				if(!lockedOn)
				{
					rollDir = Vector3.right;
					_transform.GetChild(0).transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
				}
			}
			if(Input.GetKey (KeyCode.A) || Input.GetAxisRaw("Horizontal") < 0 || Input.GetAxisRaw("HorizDPad") < 0)
			{
				_controller.Move (-Vector3.right * speed * Time.deltaTime);
				if(!lockedOn)
				{
					rollDir = -Vector3.right;
					_transform.GetChild(0).transform.localScale = new Vector3(-1.6f, 1.6f, 1.6f);
				}	
			}
			if(Input.GetKey (KeyCode.W) || Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("VertDPad") > 0)
			{
				_controller.Move (Vector3.forward * speed * Time.deltaTime);
				if(!lockedOn)
				rollDir = Vector3.forward;
			}
			if(Input.GetKey (KeyCode.S) || Input.GetAxisRaw("Vertical") < 0 || Input.GetAxisRaw("VertDPad") < 0)
			{
				_controller.Move (-Vector3.forward * speed * Time.deltaTime);
				if(!lockedOn)
				rollDir = -Vector3.forward;
			}

			if(!lockedOn)
			{
				if((Input.GetKey (KeyCode.A) && Input.GetKey (KeyCode.W)) || (Input.GetAxisRaw("HorizDPad") < 0 && Input.GetAxisRaw("VertDPad") > 0))
				{
					rollDir = new Vector3(-1, 0, 1);
				}
				if((Input.GetKey (KeyCode.D) && Input.GetKey (KeyCode.W)) || (Input.GetAxisRaw("HorizDPad") > 0 && Input.GetAxisRaw("VertDPad") > 0))
				{
					rollDir = new Vector3(1, 0, 1);
				}
				if((Input.GetKey (KeyCode.A) && Input.GetKey (KeyCode.S)) || (Input.GetAxisRaw("HorizDPad") < 0 && Input.GetAxisRaw("VertDPad") < 0))
				{
					rollDir = new Vector3(-1, 0, -1);
				}
				if((Input.GetKey (KeyCode.D) && Input.GetKey (KeyCode.S)) || (Input.GetAxisRaw("HorizDPad") > 0 && Input.GetAxisRaw("VertDPad") < 0))
				{
					rollDir = new Vector3(1, 0, -1);
				}
				hitDetect.position = _transform.position + rollDir; //sets the location of the OverlapSphere for attacking direction
			}

			if(Input.GetButtonDown ("Fire1"))
			{
				if(currStam >= 10f)
				{
					rechargeStam = false;
					StopCoroutine("staminaChargeDelay");
					Raycast();
					currStam-=10f;
					StartCoroutine("staminaChargeDelay");
				}
			}
			if(Input.GetButtonDown ("LockOn"))
			{
				Debug.Log("Locked");
				speed = 3f;
				lockedOn = true;
			}
			if(Input.GetButtonUp ("LockOn"))
			{
				lockedOn = false;
				speed = 6f;
			}
		}
		if(rechargeStam) //stamina recharging
		{
			currStam += 25f * Time.deltaTime;
			if(currStam >= maxStam)
			{
				currStam = maxStam;
				rechargeStam = false;
			}
		}

		RaycastHit hitRoll;
		if(Physics.Raycast(_transform.position, rollDir, out hitRoll, 1f))
		{
			Debug.Log ("Not Rolling");
			if(isRolling)
			{
				rigidbody.velocity = Vector3.zero;
			}
		}
		else
		{
			if(!isRolling)
			{
				if(currStam >= 20)
				{
					if(Input.GetButtonDown("Roll"))
					{
						StopCoroutine("rollTimer");
						isRolling = true;
						rechargeStam = false;
						StartCoroutine("rollTimer");
						currStam -= 20f;
					}
				}
			}
		}

		//_controller.Move (rollDir * speed * rollForce * Time.deltaTime); //moving the charController

		if(_transform.position.y > -5.46f) //check if the player moves up on Y axis
		{
			_transform.position = new Vector3 (_transform.position.x, -5.46f, _transform.position.z);
		}
	}

	void Raycast()
	{
		Collider[] hitColliders = Physics.OverlapSphere (hitDetect.position, .3f);

		for(int i = 0; i < hitColliders.Length; i++)
		{
			Destroy(hitColliders[i].gameObject);
		}
	}

	void OnDrawGizmos() //for visual debug
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawSphere(playerSprite.transform.position+rollDir, .6f);
	}
	
	IEnumerator rollTimer()
	{
		yield return new WaitForSeconds(.2f);
		rigidbody.velocity = Vector3.zero;
		transform.position = transform.position;
		isRolling = false;
		yield return new WaitForSeconds(.8f);
		rechargeStam = true;
	}

	IEnumerator staminaChargeDelay()
	{
		yield return new WaitForSeconds(.8f);
		rechargeStam = true;
	}

	void FixedUpdate ()
	{
		if(isRolling)
		{
			rigidbody.AddForce(rollDir * rollForce, ForceMode.Impulse);
		}
	}
	
	/*void ForceIntegrator()
	{
		Vector3 accel = forceAcc / mass; //new vector for acceleration, which is the forceAcc divided by the mass of object
		velocity = velocity + accel * Time.deltaTime; //add velocity to itself, with the accel multiplied by time
		transform.position = transform.position + velocity * Time.deltaTime; //adds its position to itself, with the calculate velocity multiplied by time
		forceAcc = Vector3.zero; //reset forceAcc to zero so each cycle
		if(velocity.magnitude > float.Epsilon) //if there is any velocity past smallest possible number, normalize its forward direction 
		{
			transform.forward = Vector3.Normalize(velocity);
		}

		if(Input.GetAxisRaw ("Horizontal") > 0)
		{
			forceAcc += Vector3.right * ; //adds force to make it move!		
		}
		if(Input.GetAxisRaw ("Horizontal") < 0)
		{
			forceAcc += -Vector3.right * ; //adds force to make it move!		
		}
		if(Input.GetAxisRaw ("Vertical") > 0)
		{
			forceAcc += Vector3.forward * ; //adds force to make it move!		
		}
		if(Input.GetAxisRaw ("Vertical") < 0)
		{
			forceAcc += -Vector3.forward * ; //adds force to make it move!		
		}
		velocity *= 0.9f;
		if(Input.GetAxisRaw ("Horizontal") == 0 && Input.GetAxisRaw ("Vertical") == 0)
		{
			velocity *= .1f; //adds some damping for smoother movement
		}
	}*/ //maybe a different way at moving

	void OnGUI()
	{
		GUI.DrawTexture (new Rect(10, 10, 100, 15), healthUnder);
		GUI.DrawTexture (new Rect(10, 10, currHealth, 15), healthOver);

		GUI.DrawTexture (new Rect(10, 30, 100, 15), stamUnder);
		GUI.DrawTexture (new Rect(10, 30, currStam, 15), stamOver);
	}
}
