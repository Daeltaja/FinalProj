using UnityEngine;
using System.Collections;

public class PlayerRanger : PlayerBase {

	public GameObject arrow;
	[HideInInspector]public Vector3 arrowDir, arrowSpriteDir;
	[HideInInspector]public int arrowsCurr, arrowMax = 6;
	float stamDynamic;
	bool powershot;
	float arrowForce = 40f;
	GUIText warriorScore;

	new void Awake()
	{
		base.Awake();
		warriorScore = GameObject.Find ("GUIWarriorScore").gameObject.GetComponent<GUIText>();
	}

	void Start () 
	{
		rollDir = Vector3.right;
		arrowsCurr = arrowMax;
		healthFrom = currHealth;
		stamFrom = currStam;
		warriorScore.text = ""+warrScore;
	}

	new void Update () 
	{
		if(inGame)
		{
			Move();
			Abilities();
			base.Update();
		}
	}

	void ApplyDamage(int amount)
	{
		currHealth -= amount;
		healthFrom = currHealth+amount;
		Shake();
		FlashRed();
		if(currHealth <= 0)
		{
			//Play death pose
			inGame = false;
			warrScore++;
			warriorScore.text = "" +warrScore;
			currHealth = 0;
			StartCoroutine("RestartRound");
			//if score = 3, player wins
			if(warrScore == 3)
			{
				//win pose
				//win GUI
				//restart buttons
			}
		}
	}

	void Move()
	{
		if(!isRolling && !isAttacking)
		{
			if(Input.GetKey (KeyCode.RightArrow) || Input.GetAxisRaw("Horizontal2") > 0 || Input.GetAxisRaw("HorizDPad2") > 0)
			{
				_controller.Move (Vector3.right * speed * Time.deltaTime);
				if(!lockedOn)
				{
					rollDir = Vector3.right;
					arrowDir = new Vector3(0, 0, 0);
					arrowSpriteDir = new Vector3(40, 0, 0);
					_transform.GetChild(0).transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
				}
			}
			if(Input.GetKey (KeyCode.LeftArrow) || Input.GetAxisRaw("Horizontal2") < 0 || Input.GetAxisRaw("HorizDPad2") < 0)
			{
				_controller.Move (-Vector3.right * speed * Time.deltaTime);
				if(!lockedOn)
				{
					rollDir = -Vector3.right;
					arrowDir = new Vector3(0, 0, 180);
					arrowSpriteDir = new Vector3(-40, 0, 0);
					_transform.GetChild(0).transform.localScale = new Vector3(-1.6f, 1.6f, 1.6f);
				}	
			}
			if(Input.GetKey (KeyCode.UpArrow) || Input.GetAxisRaw("Vertical2") > 0 || Input.GetAxisRaw("VertDPad2") > 0)
			{
				_controller.Move (Vector3.forward * speed * Time.deltaTime);
				if(!lockedOn)
				{
					arrowDir = new Vector3(0, -90, 0);
					arrowSpriteDir = new Vector3(90, 0, 0);
					rollDir = Vector3.forward;
				}
			}
			if(Input.GetKey (KeyCode.DownArrow) || Input.GetAxisRaw("Vertical2") < 0 || Input.GetAxisRaw("VertDPad2") < 0)
			{
				_controller.Move (-Vector3.forward * speed * Time.deltaTime);
				if(!lockedOn)
				{
					arrowDir = new Vector3(0, 90, 0);
					arrowSpriteDir = new Vector3(-90, 0, 0);
					rollDir = -Vector3.forward;
				}
			}
			
			if(!lockedOn)
			{
				if((Input.GetKey (KeyCode.A) && Input.GetKey (KeyCode.W)) || (Input.GetAxisRaw("HorizDPad2") < 0 && Input.GetAxisRaw("VertDPad2") > 0) || (Input.GetAxisRaw("Horizontal2") < 0 && Input.GetAxisRaw("Vertical2") > 0))
				{
					rollDir = new Vector3(-1, 0, 1);
				}
				if((Input.GetKey (KeyCode.D) && Input.GetKey (KeyCode.W)) || (Input.GetAxisRaw("HorizDPad2") > 0 && Input.GetAxisRaw("VertDPad2") > 0) || (Input.GetAxisRaw("Horizontal2") > 0 && Input.GetAxisRaw("Vertical2") > 0))
				{
					rollDir = new Vector3(1, 0, 1);
				}
				if((Input.GetKey (KeyCode.A) && Input.GetKey (KeyCode.S)) || (Input.GetAxisRaw("HorizDPad2") < 0 && Input.GetAxisRaw("VertDPad2") < 0) || (Input.GetAxisRaw("Horizontal2") < 0 && Input.GetAxisRaw("Vertical2") < 0))
				{
					rollDir = new Vector3(-1, 0, -1);
				}
				if((Input.GetKey (KeyCode.D) && Input.GetKey (KeyCode.S)) || (Input.GetAxisRaw("HorizDPad2") > 0 && Input.GetAxisRaw("VertDPad2") < 0) || (Input.GetAxisRaw("Horizontal2") > 0 && Input.GetAxisRaw("Vertical2") < 0))
				{
					rollDir = new Vector3(1, 0, -1);
				}



				hitDetect.position = _transform.position + rollDir; //sets the location of the OverlapSphere for attacking direction
			}
		}
	}
		
	void Attack()
	{
		if(arrowsCurr > 0)
		{
			if(!lockedOn)
			{
				
				if(currStam >= 10f)
				{
					currStam -= 10f;
					stamFrom = currStam+10f;
					arrowsCurr--;
					rechargeStam = false;
					isAttacking = true;
					arrowForce = 40f;
					//attack
					GameObject arrowGO = Instantiate(arrow, _transform.position + rollDir, _transform.rotation) as GameObject;
					arrowGO.rigidbody.AddForce(rollDir * arrowForce, ForceMode.Impulse);
					arrowGO.transform.localEulerAngles = arrowDir;
					arrowGO.transform.GetChild(0).transform.localEulerAngles = arrowSpriteDir;
					arrowGO.GetComponent<Arrow>().damage = 10f;
					//StartCoroutine("HitDetection", hitDelay);
					StartCoroutine("AttackResetDelay", hitDelay);
					//stamina
					StopCoroutine("StaminaChargeDelay");
					StartCoroutine("StaminaChargeDelay", stamChargeDelay);
				}
			}
			else
			{
				rechargeStam = false;
				isAttacking = true;
				arrowForce = 50f;
				arrowsCurr--;
				//attack
				GameObject arrowGO = Instantiate(arrow, _transform.position + rollDir, _transform.rotation) as GameObject;
				arrowGO.rigidbody.AddForce(rollDir * arrowForce, ForceMode.Impulse);
				arrowGO.transform.localEulerAngles = arrowDir;
				arrowGO.transform.GetChild(0).transform.localEulerAngles = arrowSpriteDir;
				arrowGO.GetComponent<Arrow>().damage = 30f;
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
			if(Input.GetButtonDown ("LockOn2"))
			{
				speed = 2f;
				lockedOn = true;
				stamDynamic = currStam;
				rechargeStam = false;
				StopCoroutine("StaminaChargeDelay");
				powershot = true;
			}
			if(Input.GetButtonUp ("LockOn2"))
			{
				speed = 6f;
				lockedOn = false;
				StartCoroutine("StaminaChargeDelay", stamChargeDelay);
			}
			if(Input.GetButtonDown ("Fire2"))
			{
				Attack ();
			}
			if(lockedOn)
			{
				if(powershot)
				{
					if(currStam >= 40f)
					currStam -= 20f * Time.deltaTime;
					if(currStam <= stamDynamic-40f)
					{
						currStam = currStam;
						powershot = false;
					}
				}
			}
		}
		if(!isRolling) //rolling
		{
			if(currStam >= 20)
			{
				if(Input.GetButtonDown("Roll2"))
				{
					isRolling = true;
					rechargeStam = false;
					currStam -= 20f;
					stamFrom = currStam+20f;
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
		if(currHealth < healthFrom)
		{
			StartCoroutine("DelayHealthRecharge");
		}
		if(currStam < stamFrom)
		{
			stamFrom -= Time.deltaTime * 15f;
		}
	}

	IEnumerator DelayHealthRecharge()
	{
		yield return new WaitForSeconds(.5f);
		healthFrom -= Time.deltaTime * 12f;
	}

	public void StopHealthCharge()
	{
		StopCoroutine("DelayHealthRecharge");
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

	void OnDrawGizmos() //for visual debug
	{
		Gizmos.color = Color.magenta;
		if(attackGizmo)
			Gizmos.DrawSphere(playerSprite.transform.position+rollDir, .6f);
	}

	/*void DebugStuff()
	{
		//Debug Stuff
		//position.text = transform.position.x.ToString("0.0") + "x" + "  " +transform.position.z.ToString("0.0") + "z";
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
	}*/
}
