using UnityEngine;
using System.Collections;

public class Interface : MonoBehaviour {

	public GUISkin mySkin;
	GameObject warrior, ranger;
	PlayerWarrior pw;
	PlayerRanger pr;

	public Texture arrowUI, arrowUIEmpty;
	public Texture healthOver, healthUnder, stamOver, stamUnder, statusBarAnim;

	void Start () 
	{
		warrior = GameObject.FindWithTag("Warrior");
		ranger = GameObject.FindWithTag("Ranger");
		pw = warrior.GetComponent<PlayerWarrior>();
		pr = ranger.GetComponent<PlayerRanger>();
	}
	
	void OnGUI()
	{
		GUI.skin = mySkin;
		//WARRIOR
		GUI.DrawTexture (new Rect(10, 10, 100, 15), healthUnder);
		GUI.DrawTexture (new Rect(10, 10, pw.healthFrom, 15), statusBarAnim);
		GUI.DrawTexture (new Rect(10, 10, pw.currHealth, 15), healthOver);
		
		GUI.DrawTexture (new Rect(10, 30, 100, 15), stamUnder);
		GUI.DrawTexture (new Rect(10, 30, pw.stamFrom, 15), statusBarAnim);
		GUI.DrawTexture (new Rect(10, 30, pw.currStam, 15), stamOver);

		//RANGER
		GUI.DrawTexture (new Rect(Screen.width - 10, 10, -100, 15), healthUnder);
		GUI.DrawTexture (new Rect(Screen.width - 10, 10, -pr.healthFrom, 15), statusBarAnim);
		GUI.DrawTexture (new Rect(Screen.width - 10, 10, -pr.currHealth, 15), healthOver);
		
		GUI.DrawTexture (new Rect(Screen.width - 10, 30, -100, 15), stamUnder);
		GUI.DrawTexture (new Rect(Screen.width - 10, 30, -pr.stamFrom, 15), statusBarAnim);
		GUI.DrawTexture (new Rect(Screen.width - 10, 30, -pr.currStam, 15), stamOver);
		
		float xthing = 35f;
		float xthing2 = 35f;
		for(int i = 0; i < pr.arrowMax; i++)
		{
			GUI.DrawTexture(new Rect(Screen.width-xthing2, 45, arrowUI.width*2, arrowUI.height*2), arrowUIEmpty);
			xthing2 = xthing2 +18f;
		}
		for(int i = 0; i < pr.arrowsCurr; i++)
		{
			GUI.DrawTexture(new Rect(Screen.width-xthing, 45, arrowUI.width*2, arrowUI.height*2), arrowUI);
			xthing = xthing +18f;
		}

	}
}
