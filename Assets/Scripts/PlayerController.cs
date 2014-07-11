using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed;
	float vert, horiz;
	Vector3 velocity = Vector3.zero;
	Vector3 forceAcc = Vector3.zero;
	float mass = 1f;
	public GameObject placementIndicator, playerSprite;
	Vector3 lookAt;
	Vector3 rollDir;
	bool isRolling, rechargeStam;
	bool isFocusing;

	float currHealth = 100f, maxHealth = 100f;
	float currStam = 100f, maxStam = 100f;

	public Texture healthOver, healthUnder, stamOver, stamUnder;

	void Start () 
	{
		rollDir = Vector3.right;
	}
	

	void Update () 
	{
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
				transform.Translate(Vector3.right * speed * Time.deltaTime);
				rollDir = Vector3.right;
				transform.GetChild(0).transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
			}
			if(Input.GetKey (KeyCode.A) || Input.GetAxisRaw("Horizontal") < 0 || Input.GetAxisRaw("HorizDPad") < 0)
			{
				transform.Translate(-Vector3.right * speed * Time.deltaTime);
				rollDir = -Vector3.right;
				transform.GetChild(0).transform.transform.localScale = new Vector3(-1.6f, 1.6f, 1.6f);
			}
			if(Input.GetKey (KeyCode.W) || Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("VertDPad") > 0)
			{
				transform.Translate(Vector3.forward * speed * Time.deltaTime);
				rollDir = Vector3.forward;
			}
			if(Input.GetKey (KeyCode.S) || Input.GetAxisRaw("Vertical") < 0 || Input.GetAxisRaw("VertDPad") < 0)
			{
				transform.Translate(-Vector3.forward * speed * Time.deltaTime);
				rollDir = -Vector3.forward;
			}

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
			Raycast();
		}


		if(rechargeStam)
		{
			currStam += 15f * Time.deltaTime;
			if(currStam >= maxStam)
			{
				currStam = maxStam;
				rechargeStam = false;
			}
		}
	}

	void Raycast()
	{
		RaycastHit hit;
		Debug.DrawRay(playerSprite.transform.position, rollDir * 3f, Color.magenta );
		if(Physics.Raycast (playerSprite.transform.position, rollDir, out hit, 1f))
		{

		}
	}

	IEnumerator rollTimer()
	{
		yield return new WaitForSeconds(.2f);
		rigidbody.velocity = Vector3.zero;
		isRolling = false;
		yield return new WaitForSeconds(.8f);
		rechargeStam = true;
	}

	void FixedUpdate ()
	{
		if(!isRolling)
		{
			if(currStam >= 30)
			{
				if(Input.GetButtonDown("Jump"))
				{
					rechargeStam = false;
					rigidbody.AddForce(rollDir * 15f , ForceMode.Impulse);
					isRolling = true;
					currStam -= 30f;
					StartCoroutine("rollTimer");
				}
			}
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
			forceAcc += Vector3.right * speed; //adds force to make it move!		
		}
		if(Input.GetAxisRaw ("Horizontal") < 0)
		{
			forceAcc += -Vector3.right * speed; //adds force to make it move!		
		}
		if(Input.GetAxisRaw ("Vertical") > 0)
		{
			forceAcc += Vector3.forward * speed; //adds force to make it move!		
		}
		if(Input.GetAxisRaw ("Vertical") < 0)
		{
			forceAcc += -Vector3.forward * speed; //adds force to make it move!		
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
