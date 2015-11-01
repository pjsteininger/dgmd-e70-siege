using System.Collections;
using UnityEngine;

class BoardMove : MonoBehaviour {

	public float maxAttackDistance = 2.01f;
	private float moveSpeed = 3f;
	private float gridSize = 1f;

	private bool isMoving = false;
	private string attackerToMove = "";
	private Vector3 startPosition;
	private Vector3 endPosition;
	private float t;
	private float factor;
	private float maxReach = Mathf.Infinity;
	private GameObject clickedGO = null;
	private GameManager gameScript;
	private string playerTurn;
	//gameSript = GetComponent<GameManager> ();

	public void Update() {
		/*
		 * Using Mouse Click to select which game object to move and to which location. The first click is to select the 
		 * Game Object. The second click is to select the new position for that game object. The object moves after the
		 * second click.
		 * for ref/background see: http://forum.unity3d.com/threads/unity-2d-raycast-from-mouse-to-screen.211708/ 
		 */
		if (Input.GetMouseButtonDown(0)) 
		{
			// Enter here on mouse button down input
			//Debug.Log ("Mouse Down");
			// using Physics2D's Raycast with the Raycast position point in world coordinates (1st argument)
			// setting a distance of zero means the Raycast will only intersect with the object directly under the cursor
			// maxReach is the default and being added here to get to the next arg: LayerMask.
			// LayerMask is used to only get a hit from "Scout" layer types (attackers) and avoid other object with colliders

			//boardScript = GetComponent<BoardManager> ();
			playerTurn = BoardManager.PlayTurn();
			//Debug.Log("Player Turn to:: " + playerTurn);
			RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, maxReach, LayerMask.GetMask(playerTurn));
			if (hitInfo.collider != null) 
			{
				Behaviour h;
				if (clickedGO != null) {
					h = (Behaviour) clickedGO.GetComponent("Halo");
					h.enabled = false;
				}
				clickedGO = hitInfo.transform.gameObject;
				// We got a hit: get the Attacker to Move tag. The tag is uniquely generated when the attackers are instantiated
				// in BoardManager. store tag in attackerToMove private string.
				//Debug.Log ("Hit G.O Tag: " + hitInfo.transform.gameObject.tag + " G.O Position: " + hitInfo.transform.gameObject.transform.position);
				attackerToMove = clickedGO.tag;
				// Enable Haloe to clicked G.O
				h = (Behaviour) clickedGO.GetComponent("Halo");
				h.enabled = true;
				// reset endPosition to zero to avoid temporary moving the attacker to position zero
				endPosition = Vector3.zero;
			}
			else if (attackerToMove != "")
			{
				// Mouse click was not of the type "Scout", then we should test for the grid position to move attacker to
				// Note: same args as first Raycast call except for the LayerMask being that of the target grid tile: "TargetTile"
				RaycastHit2D targetInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100, LayerMask.GetMask("TargetTile"));
				if (targetInfo.collider != null) 
				{
					//Debug.Log ("Start Position: " + gameObject.transform.position);
					//Debug.Log ("End Position: " + targetInfo.transform.gameObject.transform.position);
					if(Vector3.Distance(gameObject.transform.position, targetInfo.transform.gameObject.transform.position) <= maxAttackDistance) 
					{
						//Debug.Log ("Target G.O Name: " + targetInfo.transform.gameObject.name + " G.O Position: " + targetInfo.transform.gameObject.transform.position);
						endPosition = targetInfo.transform.gameObject.transform.position;
					}
					else 
					{
						//Debug.Log ("Target Too Far " + targetInfo.transform.gameObject.transform.position);
						return;
					}
					//Debug.Log ("Target G.O Name: " + targetInfo.transform.gameObject.name + " G.O Position: " + targetInfo.transform.gameObject.transform.position);
					endPosition = targetInfo.transform.gameObject.transform.position;
				}
				else 
				{
					Debug.Log ("No Hit/No Target. Target Info collider = " + targetInfo.collider);
				}
			}
		}

		if (!isMoving && endPosition != Vector3.zero) {
			// Only enter here if the object is not moving and endPosition != 0
			//Only move G.O that was clicked
			if (attackerToMove == gameObject.tag)
			{
				StartCoroutine(move(transform));
			}
		}
	}
	
	public IEnumerator move(Transform transform) 
	{
		isMoving = true;
		// get the start position of the attacker
		startPosition = transform.position;
		t = 0;

		factor = 1f;
				
		while (t < 1f) {
			t += Time.deltaTime * (moveSpeed/gridSize) * factor;
			transform.position = Vector3.Lerp(startPosition, endPosition, t);
			yield return null;
		}
		// reset all variables since the move is done
		isMoving = false;
		endPosition = Vector3.zero;
		// Disable Haloe to clicked G.O
		Behaviour h = (Behaviour) clickedGO.GetComponent("Halo");
		h.enabled = false;
		clickedGO = null;
		attackerToMove = "";
		BoardManager.SwitchPlayer();
		yield return 0;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		//Debug.Log ("other GO name: " + other.gameObject.name + "current GO name: " + this.gameObject.name);
		//This function will be called by both objects colliding (i.e attacker and Guard). To avoid destroying both 
		// object need to add a check for current active play (whose turn is it) and only destroy the other
		if (other.gameObject.name == "Wall(Clone)") 
		{
			Debug.Log ("HIT WALL!");
			endPosition = startPosition;
		}
		else if (playerTurn == "Scout" && this.gameObject.name == "Attacker(Clone)" && other.gameObject.name == "Guard(Clone)")  
		{
			Debug.Log("Destroy Guard :" + other.gameObject.tag);
			Destroy(other.gameObject);
		}
		else if (playerTurn == "Guard" && this.gameObject.name == "Guard(Clone)" && other.gameObject.name == "Attacker(Clone)") 
		{
			Debug.Log("Destroy Attacker :" + other.gameObject.tag);
			Destroy(other.gameObject);
		}
		else 
		{
			//Debug.Log("Collision : Do Nothing: " + other.gameObject.name);
			return;
		}
	}
}