using UnityEngine;
using System.Collections;

public class PlayerBase : MonoBehaviour {

	public float speed, rollForce, rollDuration, hitDelay, stamChargeDelay, attackResetDelay, restDelay;

	public GameObject placementIndicator, playerSprite, playerSpriteChild;
	public Transform hitDetect;

	public GUIText WINTEXT;

	public static bool inGame = true, boutEnd = false;

	[HideInInspector]public Vector3 lookAt;
	[HideInInspector]public Vector3 rollDir;
	[HideInInspector]public bool isRolling, isResting, isAttacking, rechargeStam;
	[HideInInspector]public bool lockedOn;
	[HideInInspector]public bool attackGizmo;
	[HideInInspector]public AudioSource _audioSource;
	
	[HideInInspector]public float currHealth = 100f, maxHealth = 100f, healthFrom;
	[HideInInspector]public float currStam = 100f, maxStam = 100f, stamFrom;
	
	[HideInInspector]public Transform _transform;
	[HideInInspector]public CharacterController _controller;

	public float shakeAmount, shakeDuration;
	public iTween.EaseType easeType;
	[HideInInspector]public Color _colorValue;
	[HideInInspector]public GameObject myCamera;
	[HideInInspector]public static int warrScore, rangScore;
	[HideInInspector]public Animator _myAnim;
	
	protected void Awake () 
	{
		_controller = GetComponent<CharacterController>();
		_transform = this.transform;
		_colorValue = playerSpriteChild.GetComponent<SpriteRenderer>().color;
		myCamera = GameObject.Find("CameraParent");
	}

	protected void Update () 
	{
		if(rechargeStam) //stamina recharging
		{
			currStam += 25f * Time.deltaTime;
			if(currStam >= maxStam)
			{
				currStam = maxStam;
				rechargeStam = false;
			}
			if(currStam <= 0f)
			{
				currStam = 0f;
			}
		}
		if(_transform.position.y > -5.46f) //check if the player moves up on Y axis
		{
			_transform.position = new Vector3 (_transform.position.x, -5.46f, _transform.position.z);
		}
	}

	public IEnumerator RestartRound()
	{
		yield return new WaitForSeconds(3f);
		Application.LoadLevel(Application.loadedLevelName);
		inGame = true;
	}

	public void Shake()
	{	
		iTween.ShakePosition(playerSprite.gameObject, iTween.Hash("x", shakeAmount, "time", shakeDuration, "easetype", easeType, "islocal", true));
	}

	public void FlashRed()
	{
		iTween.ColorFrom(playerSprite.transform.GetChild(0).gameObject, iTween.Hash("r", 1, "b", 0, "g", 0, "a", 1,"time", 0.3f));
	}
}
