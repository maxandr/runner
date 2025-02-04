﻿using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.


	public float moveForce = 365f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 5f;				// The fastest the player can travel in the x axis.
	public float Speed = 5f;				// The fastest the player can travel in the x axis.
	public AudioClip[] jumpClips;			// Array of clips for when the player jumps.
	public float jumpForce = 1000f;			// Amount of force added when the player jumps.
	public AudioClip[] taunts;				// Array of clips for when the player taunts.
	public float tauntProbability = 50f;	// Chance of a taunt happening.
	public float tauntDelay = 1f;			// Delay for when the taunt should happen.


	private int tauntIndex;					// The index of the taunts array indicating the most recent taunt.
	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private bool grounded = false;			// Whether or not the player is grounded.
	private Animator anim;					// Reference to the player's animator component.
	private int movingDirection;
	void Awake()
	{
		// Setting up references.
		groundCheck = transform.Find("groundCheck");
		anim = GetComponent<Animator>();
		movingDirection = 0;
	}


	public void Restart () {
		Application.LoadLevel (Application.loadedLevel);
	}
	public void Jump () {
		grounded = Physics.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));  
		// If the jump button is pressed and the player is grounded then the player should jump.
			if (grounded) {
				jump = true;
			}
	}
	public void SetMovingDirection(int x) {
		movingDirection = x;
	}
	void Update ()
	{
		grounded = Physics.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));  
		// If the jump button is pressed and the player is grounded then the player should jump.
		if (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown (KeyCode.Joystick1Button0)) {
			if (grounded) {
				jump = true;
			}
		}
		if (Input.GetKeyDown (KeyCode.JoystickButton6)) {
			Restart();
		}
	/*	if (facingRight) {
			transform.position -= transform.right * Speed * Time.deltaTime;
			Camera.main.transform.position -= transform.right * Speed * Time.deltaTime;
		}
		else {
			transform.position += transform.right * Speed * Time.deltaTime;
			Camera.main.transform.position += transform.right * Speed * Time.deltaTime;
		}*/
		// Cache the horizontal input.
		float h;
		 h = movingDirection;
#if UNITY_EDITOR_WIN
		h = Input.GetAxis ("Horizontal");
#endif
		Vector3 t = Camera.main.transform.position;
		t.y = transform.position.y;
		Camera.main.transform.position = t;
		
		// The Speed animator parameter is set to the absolute value of the horizontal input.
		anim.SetFloat ("Speed", Mathf.Abs (h));

		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if (h > 0) {
			transform.position -= transform.right * Speed * Time.deltaTime;
			Camera.main.transform.position -= transform.right * Speed * Time.deltaTime;
		}
		if (h < 0) {
			transform.position += transform.right * Speed * Time.deltaTime;
			Camera.main.transform.position += transform.right * Speed * Time.deltaTime;
		}
		if (h > 0 && !facingRight)
			Flip ();
		else if (h < 0 && facingRight)
			Flip ();

			// If the player should jump...
		if (jump) {
				// Set the Jump animator trigger parameter.
			anim.SetTrigger ("Jump");

				// Play a random jump audio clip.
			int i = Random.Range (0, jumpClips.Length);
			AudioSource.PlayClipAtPoint (jumpClips [i], transform.position);

				// Add a vertical force to the player.
			GetComponent<Rigidbody> ().AddForce (new Vector3 (0f, jumpForce, 0f));

				// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			jump = false;
		}
	}

	
	public void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}


	public IEnumerator Taunt()
	{
		// Check the random chance of taunting.
		float tauntChance = Random.Range(0f, 100f);
		if(tauntChance > tauntProbability)
		{
			// Wait for tauntDelay number of seconds.
			yield return new WaitForSeconds(tauntDelay);

			// If there is no clip currently playing.
			if(!GetComponent<AudioSource>().isPlaying)
			{
				// Choose a random, but different taunt.
				tauntIndex = TauntRandom();

				// Play the new taunt.
				GetComponent<AudioSource>().clip = taunts[tauntIndex];
				GetComponent<AudioSource>().Play();
			}
		}
	}


	int TauntRandom()
	{
		// Choose a random index of the taunts array.
		int i = Random.Range(0, taunts.Length);

		// If it's the same as the previous taunt...
		if(i == tauntIndex)
			// ... try another random taunt.
			return TauntRandom();
		else
			// Otherwise return this index.
			return i;
	}
}
