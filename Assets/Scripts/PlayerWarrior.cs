using UnityEngine;
using System.Collections;

public class PlayerWarrior : PlayerBase {

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
		Move();
		Abilities();

		DebugStuff();

		if(rechargeStam) //stamina recharging
		{
			currStam += 25f * Time.deltaTime;
			if(currStam >= maxStam)
			{
				currStam = maxStam;
				rechargeStam = false;
			}
		}
		if(_transform.position.y > -5.46f) //check if the player moves up on Y axis
		{
			_transform.position = new Vector3 (_transform.position.x, -5.46f, _transform.position.z);
		}
	}

	void Move()
	{
		if(!isRolling && !isAttacking)
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
				{
					rollDir = Vector3.forward;
				}
			}
			if(Input.GetKey (KeyCode.S) || Input.GetAxisRaw("Vertical") < 0 || Input.GetAxisRaw("VertDPad") < 0)
			{
				_controller.Move (-Vector3.forward * speed * Time.deltaTime);
				if(!lockedOn)
				{
					rollDir = -Vector3.forward;
				}
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
		}
	}
		
	void Attack()
	{
		if(!lockedOn)
		{
			if(currStam >= 10f)
			{
				currStam -= 10f;
				rechargeStam = false;
				isAttacking = true;
				//attack
				hitDelay = 0f;
				StartCoroutine("HitDetection", hitDelay);
				StartCoroutine("AttackResetDelay", hitDelay+.2f);
				//stamina
				StopCoroutine("StaminaChargeDelay");
				StartCoroutine("StaminaChargeDelay", stamChargeDelay);
			}
		}
		else
		{
			if(currStam >= 20f)
			{
				currStam -= 20f;
				rechargeStam = false;
				isAttacking = true;
				//attack
				hitDelay = 0.8f;
				StartCoroutine("HitDetection", hitDelay);
				StartCoroutine("AttackResetDelay", hitDelay+.4f);
				//stamina
				StopCoroutine("StaminaChargeDelay");
			}
		}
	}

	void Abilities()
	{
		if(!isRolling && !isAttacking)
		{
			if(Input.GetButtonDown ("LockOn"))
			{
				speed = 3f;
				lockedOn = true;
				StopCoroutine("StaminaChargeDelay");
			}
			if(Input.GetButtonUp ("LockOn"))
			{
				speed = 6f;
				lockedOn = false;
				StartCoroutine("StaminaChargeDelay", stamChargeDelay);
			}
			if(Input.GetButtonDown ("Fire1"))
			{
				Attack ();
			}
		}
		if(!isRolling) //rolling
		{
			if(currStam >= 20)
			{
				if(Input.GetButtonDown("Roll"))
				{
					isRolling = true;
					rechargeStam = false;
					currStam -= 20f;
					StopCoroutine("RollDuration");
					StartCoroutine("RollDuration");
					StartCoroutine("StaminaChargeDelay", stamChargeDelay);
				}
			}
		}
		if(isRolling) //rolling
		{
			_controller.Move (rollDir * rollForce * Time.deltaTime); //moving the charController
		}
	}
	
	IEnumerator HitDetection(float hitDelay)
	{
		yield return new WaitForSeconds(hitDelay); //hitDelay could sync to frame in animation
		Collider[] hitColliders = Physics.OverlapSphere (hitDetect.position, .3f); //radius 
		attackGizmo = true;
		for(int i = 0; i < hitColliders.Length; i++)
		{
			//Destroy(hitColliders[i].gameObject);
			if(hitColliders[i].name == "TargetDummy")
			{
				hitColliders[i].transform.GetChild(0).GetComponent<Animator>().SetTrigger("Hit");
			}
		}
		yield return new WaitForSeconds(.2f);
		attackGizmo = false;
	}

	IEnumerator StaminaChargeDelay(float stamChargeDelay)
	{
		yield return new WaitForSeconds(stamChargeDelay);
		rechargeStam = true;
	}

	IEnumerator AttackResetDelay(float attackResetDelay)
	{
		yield return new WaitForSeconds(attackResetDelay);
		isAttacking = false;
	}

	IEnumerator RollDuration()
	{
		yield return new WaitForSeconds(rollDuration);
		isRolling = false;
	}

	void ApplyDamage(int amount)
	{
		currHealth -= amount;
		if(currHealth < 0)
		{
			//Splat
		}
	}

	void OnDrawGizmos() //for visual debug
	{
		Gizmos.color = Color.magenta;
		if(attackGizmo)
			Gizmos.DrawSphere(playerSprite.transform.position+rollDir, .6f);
	}

	void OnGUI()
	{
		GUI.DrawTexture (new Rect(10, 10, 100, 15), healthUnder);
		GUI.DrawTexture (new Rect(10, 10, currHealth, 15), healthOver);

		GUI.DrawTexture (new Rect(10, 30, 100, 15), stamUnder);
		GUI.DrawTexture (new Rect(10, 30, currStam, 15), stamOver);
	}

	void DebugStuff()
	{
		//Debug Stuff
		position.text = transform.position.x.ToString("0.0") + "x" + "  " +transform.position.z.ToString("0.0") + "z";
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 100f) && hit.normal.y > 0.999)
		{
			placementIndicator.transform.position = hit.point;
			lookAt = hit.point  + new Vector3(0f, 0.51f, 0f);
		}
		if(lookAt != null)
		{
			//playerSprite.transform.LookAt(lookAt);
		}
	}
}
