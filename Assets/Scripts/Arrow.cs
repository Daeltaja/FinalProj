using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	Animator _anim;
	Vector3 facing;
	public float rayDist;
	bool fall;
	Vector3 yPos;
	public float fallTime;
	Transform _myTransform;
	Vector3 boxScale;
	BoxCollider boxColl;
	[HideInInspector]public float damage;
	PlayerRanger pr;
	GameObject ranger;

	void Awake()
	{
		//_anim.GetComponent<Animator>();
		boxColl = GetComponent<BoxCollider>();
		boxScale = new Vector3(boxColl.size.x, boxColl.size.y, boxColl.size.z+.2f);
		_myTransform = transform;
		yPos = new Vector3(0, 1, 0);
		transform.up = yPos;
		ranger = GameObject.FindWithTag("Ranger");
	}
	
	void OnCollisionEnter(Collision other)
	{
		rigidbody.velocity = Vector3.zero;
		fall = true;
	}

	void OnTriggerEnter (Collider other)
	{
		if(other.tag == "Ranger")
		{
			other.GetComponent<PlayerRanger>().arrowsCurr++;
			Destroy(gameObject);
		}
	}

	void Update()
	{
		Debug.DrawRay (transform.position, transform.right * rayDist, Color.magenta);
		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.right, out hit, rayDist))
		{
			if(hit.collider.tag == "Prop")
			{
				rayDist = 0.015f;
				Collection();
			}
			if(hit.collider.tag == "Warrior")
			{
				rayDist = 0.015f;
				if(!hit.collider.GetComponent<PlayerWarrior>().lockedOn)
				{
					hit.collider.BroadcastMessage("ApplyDamage", damage);
					hit.collider.BroadcastMessage("StopHealthCharge");
					ranger.GetComponent<PlayerRanger>().arrowsCurr++;
					Destroy(gameObject);
				}
			}
		}

		if(fall)
		{
			rigidbody.useGravity = true;
			if(_myTransform.position.y <= -5.94f)
			{
				rigidbody.velocity = Vector3.zero;
				rigidbody.useGravity = false;
				Collection();
				GetComponent<BoxCollider>().size = boxScale;
				fall = false;
			}
		}
	}

	void Collection()
	{
		rigidbody.isKinematic = true;
		GetComponent<BoxCollider>().isTrigger = true;
		gameObject.layer = LayerMask.NameToLayer("Default");
	}
}
