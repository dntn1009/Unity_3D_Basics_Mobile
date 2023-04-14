using UnityEngine;
using System.Collections;

public class PlayManager : MonoBehaviour 
{
	public Animator[] playerGroup; 
	private string[] animClipNameGroup;
	private int currentNumber;

	// Use this for initialization
	void Start () {

		animClipNameGroup = new string[] {
			"attack",
			"stun",
			"skill1",
			"skill2",
			"skill3",
			"skill4",
			"run",
			"getup",
			"knockdown",
			"defence",
			"hit",
			"die",
			"victory"

		};

		currentNumber = 0;


		playerGroup = GameObject.Find ("PlayerGroup").transform.GetComponentsInChildren<Animator>();

		for(int i = 0; i < playerGroup.Length; i++)
		{
			playerGroup[i].speed = 1f;
			playerGroup[i].Play(animClipNameGroup[currentNumber]);
		}
	}


	void OnGUI()
	{
		if(GUI.Button(new Rect(50,50,50,50),"<"))
		{
			currentNumber--;

			if(currentNumber < 0 )
			{
				currentNumber = animClipNameGroup.Length - 1;
			}

			for(int i = 0; i < playerGroup.Length; i++)
			{
				playerGroup[i].speed = 1f;
				playerGroup[i].Play(animClipNameGroup[currentNumber]);
			}

		}

		if(GUI.Button(new Rect(160,50,50,50),">"))
		{
			currentNumber++;

			if(currentNumber == animClipNameGroup.Length)
			{
				currentNumber = 0;
			}

			for(int i = 0; i < playerGroup.Length; i++)
			{
				playerGroup[i].speed = 1f;
				playerGroup[i].Play(animClipNameGroup[currentNumber]);
			}
		}

		GUI.Label (new Rect(240, 50, 200,100), animClipNameGroup[currentNumber].ToString() );

	}
}
