using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager siegeGame = null;//singleton behavior for this game manager
	private BoardManager boardScript;

	// Use this for initialization
	void Awake () 
	{
		if (siegeGame == null)
			siegeGame = this;
		else if (siegeGame != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);

		boardScript = GetComponent<BoardManager> ();
		InitGame ();
	}

	void InitGame()
	{
		boardScript.SetupScene ();
	}
	// Update is called once per frame
	void Update () {
	
	}
}
