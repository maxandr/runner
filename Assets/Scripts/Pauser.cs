﻿using UnityEngine;
using System.Collections;

public abstract class Pauser : MonoBehaviour // Any resemblance to lady stuff is purely coincidental ...
{
	public bool pause = false;

	protected IEnumerator Start1()
	{
		while( Application.isPlaying )
		{
			DoUpdate();

			yield return null;

			// You could even have a time variable here
		}
	}
	void Update() {
		if (Input.GetKeyDown (KeyCode.JoystickButton7)) {
			if(Time.timeScale==0) {
				Time.timeScale=1.0f;
				pause=false;
			}
			else if(Time.timeScale==1.0f) {
				Time.timeScale=0;
				pause=true;
			}
		}
		if(pause) {
			Time.timeScale=0.0f;
		}
		else {
			Time.timeScale=1.0f;
		}
	}
		// That's where it happens !
	protected void Pausegame( bool tpause) {
		pause = tpause;
	}
	protected abstract void DoUpdate();
}