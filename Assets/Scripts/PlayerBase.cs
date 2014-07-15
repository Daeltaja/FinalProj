using UnityEngine;
using System.Collections;

public class PlayerBase : MonoBehaviour {

	public float speed, rollForce, rollDuration, hitDelay, stamChargeDelay, attackResetDelay;

	public GameObject placementIndicator, playerSprite;
	public Transform hitDetect;
	public GUIText position;

	[HideInInspector]public Vector3 lookAt;
	[HideInInspector]public Vector3 rollDir;
	[HideInInspector]public bool isRolling, isAttacking, rechargeStam;
	[HideInInspector]public bool lockedOn;
	[HideInInspector]public bool attackGizmo;
	
	[HideInInspector]public float currHealth = 100f, maxHealth = 100f;
	[HideInInspector]public float currStam = 100f, maxStam = 100f;
	
	public Texture healthOver, healthUnder, stamOver, stamUnder;
	
	[HideInInspector]public Transform _transform;
	[HideInInspector]public CharacterController _controller;

	void Start () 
	{
		
	}

	void Update () 
	{
	
	}
}
