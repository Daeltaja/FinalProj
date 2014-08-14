using UnityEngine;
using System.Collections;

public class PlayerWarrior : PlayerBase {

	public float damage;
	GUIText rangerScore;
	GameObject swordAttack, swordPower, hit, roll;

	new void Awake()
	{
		base.Awake();
		rangerScore = GameObject.Find ("GUIRangerScore").gameObject.GetComponent<GUIText>();
	}

	void Start () 
	{
		rollDir = Vector3.right;
		healthFrom = currHealth;
		stamFrom = currStam;
		rangerScore.text = ""+rangScore;
		_myAnim = GameObject.Find ("Vanguard").GetComponent<Animator>();
		swordAttack = GameObject.Find ("AudioAttack");
		swordPower = GameObject.Find ("AudioAttackPower");
		hit = GameObject.Find ("AudioHit1");
		roll = GameObject.Find ("AudioRoll1");
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
		hit.audio.Play ();

		if(currHealth <= 0)
		{
			//Play death pose
			inGame = false;
			rangScore++;
			rangerScore.text = "" +rangScore;
			currHealth = 0;
			_myAnim.SetTrigger("Die");
			StartCoroutine("RestartRound");

			if(rangScore == 3)
			{
				inGame = false;
				boutEnd = true;
				WINTEXT.text = "Ranger Wins!";
				rangScore = 0;
			}
		}
	}

	void Move()
	{
		if(!isRolling && !isResting && !isAttacking)
		{
			if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 )
			{
				_myAnim.SetBool ("Walking", true);
			}

			if(Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
			{
				_myAnim.SetBool ("Walking", false);
			}

			if(Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("HorizDPad") > 0)
			{
				_controller.Move (Vector3.right * speed * Time.deltaTime);
				if(!lockedOn)
				{
					rollDir = Vector3.right;
					_transform.GetChild(0).transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
				}
			}
			if(Input.GetAxisRaw("Horizontal") < 0 || Input.GetAxisRaw("HorizDPad") < 0)
			{
				_controller.Move (-Vector3.right * speed * Time.deltaTime);
				if(!lockedOn)
				{
					rollDir = -Vector3.right;
					_transform.GetChild(0).transform.localScale = new Vector3(-1.6f, 1.6f, 1.6f);
				}	
			}
			if(Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("VertDPad") > 0)
			{
				_controller.Move (Vector3.forward * speed * Time.deltaTime);
				if(!lockedOn)
				{
					rollDir = Vector3.forward;
				}
			}
			if(Input.GetAxisRaw("Vertical") < 0 || Input.GetAxisRaw("VertDPad") < 0)
			{
				_controller.Move (-Vector3.forward * speed * Time.deltaTime);
				if(!lockedOn)
				{
					rollDir = -Vector3.forward;
				}
			}
			
			if(!lockedOn)
			{
				if((Input.GetKey (KeyCode.A) && Input.GetKey (KeyCode.W)) || (Input.GetAxisRaw("HorizDPad") < 0 && Input.GetAxisRaw("VertDPad") > 0) || (Input.GetAxisRaw("Horizontal") < 0 && Input.GetAxisRaw("Vertical") > 0))
				{
					rollDir = new Vector3(-1, 0, 1);
				}
				if((Input.GetKey (KeyCode.D) && Input.GetKey (KeyCode.W)) || (Input.GetAxisRaw("HorizDPad") > 0 && Input.GetAxisRaw("VertDPad") > 0) || (Input.GetAxisRaw("Horizontal") > 0 && Input.GetAxisRaw("Vertical") > 0))
				{
					rollDir = new Vector3(1, 0, 1);
				}
				if((Input.GetKey (KeyCode.A) && Input.GetKey (KeyCode.S)) || (Input.GetAxisRaw("HorizDPad") < 0 && Input.GetAxisRaw("VertDPad") < 0) || (Input.GetAxisRaw("Horizontal") < 0 && Input.GetAxisRaw("Vertical") < 0))
				{
					rollDir = new Vector3(-1, 0, -1);
				}
				if((Input.GetKey (KeyCode.D) && Input.GetKey (KeyCode.S)) || (Input.GetAxisRaw("HorizDPad") > 0 && Input.GetAxisRaw("VertDPad") < 0) || (Input.GetAxisRaw("Horizontal") > 0 && Input.GetAxisRaw("Vertical") < 0))
				{
					rollDir = new Vector3(1, 0, -1);
				}
				hitDetect.position = _transform.position + rollDir*1.2f; //sets the location of the OverlapSphere for attacking direction
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
				stamFrom = currStam+10f;
				rechargeStam = false;
				isAttacking = true;
				_myAnim.SetTrigger("AttackNorm");
				swordAttack.audio.Play ();
				//attack
				hitDelay = 0f;
				damage = 15f;
				StartCoroutine("HitDetection", hitDelay);
				StartCoroutine("AttackResetDelay", hitDelay+0.4f);
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
				stamFrom = currStam+20f;
				rechargeStam = false;
				isAttacking = true;
				_myAnim.SetTrigger("AttackPower");
				swordAttack.audio.Play ();
				//attack
				hitDelay = 0.8f;
				damage = 30f;
				StartCoroutine("SlamShakeDelay");
				StartCoroutine("delaySlamNoise");
				StartCoroutine("HitDetection", hitDelay);
				StartCoroutine("AttackResetDelay", hitDelay+.4f);
				//stamina
				StopCoroutine("StaminaChargeDelay");
			}
		}
	}

	IEnumerator delaySlamNoise()
	{
		yield return new WaitForSeconds(.7f);
		swordPower.audio.Play ();
	}

	void Abilities()
	{
		if(!isRolling && !isAttacking)
		{
			if(Input.GetButtonDown ("LockOn"))
			{
				speed = 3f;
				lockedOn = true;
				_myAnim.SetTrigger("Lock");
				StopCoroutine("StaminaChargeDelay");
			}

			if(Input.GetButtonDown ("Fire1"))
			{
				Attack ();
			}
		}
		if(Input.GetButtonUp ("LockOn"))
		{
			speed = 6f;
			lockedOn = false;
			_myAnim.SetTrigger("NoLock");
			StartCoroutine("StaminaChargeDelay", stamChargeDelay);
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
					stamFrom = currStam+20f;
					_myAnim.SetTrigger("Roll");
					roll.audio.Play ();
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
		Collider[] hitColliders = Physics.OverlapSphere (hitDetect.position, .8f); //radius 

		attackGizmo = true;

		for(int i = 0; i < hitColliders.Length; i++)
		{
			//Destroy(hitColliders[i].gameObject);
			if(hitColliders[i].name == "TargetDummy")
			{
				hitColliders[i].transform.GetChild(0).GetComponent<Animator>().SetTrigger("Hit");
			}
			if(hitColliders[i].tag == "Ranger")
			{
				hitColliders[i].collider.BroadcastMessage("ApplyDamage", damage);
				hitColliders[i].collider.BroadcastMessage("StopHealthCharge");
			}
		}
		yield return new WaitForSeconds(.2f);
		attackGizmo = false;
	}

	IEnumerator SlamShakeDelay()
	{
		yield return new WaitForSeconds(hitDelay); //hitDelay could sync to frame in animation
		myCamera.BroadcastMessage("ShakeSlam");
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
		isResting = true;
		yield return new WaitForSeconds(rollDuration);
		isRolling = false;
		yield return new WaitForSeconds(restDelay);
		isResting = false;
	}

	void OnDrawGizmos() //for visual debug
	{
		Gizmos.color = Color.magenta;
		//if(attackGizmo)
			//Gizmos.DrawSphere(playerSprite.transform.position+rollDir, .6f);
	}
}
