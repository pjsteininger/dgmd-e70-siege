using System.Collections;
using UnityEngine;

class BoardMove : MonoBehaviour {
	private float moveSpeed = 3f;
	private float gridSize = 1f;

	private bool isMoving = false;
	private string attackerToMove;
	private Vector3 startPosition;
	private Vector3 endPosition;
	private float t;
	private float factor;
	private float maxReach = Mathf.Infinity;
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
			RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, maxReach, LayerMask.GetMask("Scout"));
			if (hitInfo.collider != null) 
			{
				// We got a hit: get the Attacker to Move tag. The tag is uniquely generated when the attackers are instantiated
				// in BoardManager. store tag in attackerToMove private string.
				//Debug.Log ("Hit G.O Tag: " + hitInfo.transform.gameObject.tag + " G.O Position: " + hitInfo.transform.gameObject.transform.position);
				attackerToMove = hitInfo.transform.gameObject.tag;
				// reset endPosition to zero to avoid temporary moving the attacker to position zero
				endPosition = Vector3.zero;
			}
			else 
			{
				// Mouse click was not of the type "Scout", then we should test for the grid position to move attacker to
				// Note: same args as first Raycast call except for the LayerMask being that of the target grid tile: "TargetTile"
				RaycastHit2D targetInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100, LayerMask.GetMask("TargetTile"));
				if (targetInfo.collider != null) 
				{
					//Debug.Log ("Target G.O Name: " + targetInfo.transform.gameObject.name + " G.O Position: " + targetInfo.transform.gameObject.transform.position);
					endPosition = targetInfo.transform.gameObject.transform.position;
				}
				else 
				{
					//Debug.Log ("No Hit/No Target. Target Info collider = " + targetInfo.collider);
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
		attackerToMove = "";
		yield return 0;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		Debug.Log ("HIT WALL!");
		endPosition = startPosition;
		Debug.Log ("end position: " + endPosition);
	}
}