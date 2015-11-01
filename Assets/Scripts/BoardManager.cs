using UnityEngine;
using System; 
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	public int columns = 9;
	public int rows = 11;
	public int wallCount = 8;
	public GameObject[] floorTile;
	public GameObject wallTile;
	public GameObject outerWallTile;
	public GameObject scoutTile;
	public GameObject guardTile;

	private static string playerTurn = "Scout";

	private Transform boardContainer;
	private List <Vector3> gridPositions = new List<Vector3>();

	//TEMP 

	void InitList()
	{
		gridPositions.Clear ();

		for (int x = 1; x < columns - 1; x++) {
			for (int y = 1; y < rows -1; y++) {
				gridPositions.Add (new Vector3 (y, x, 0f));
			}
		}

	}

	void BoardSetup() 
	{
		boardContainer = new GameObject ("Board").transform;
		for (int x = -1; x < columns + 1; x++) {
			for (int y = -1; y < rows + 1; y++) {
				GameObject tile = floorTile[0];
		
				if (x == -1 || x == columns || y == -1 || y == rows) {
					tile = outerWallTile;
				} else if (tile == floorTile[0]) {
					tile = floorTile[1];

				} else {
					tile = floorTile[0];
				
				}
				GameObject instance = Instantiate (tile, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent (boardContainer);
			}
		}
	}

	void LayoutWalls() 
	{
		int row = 5;
		for (int col = 0; col < columns -2; col++)
		{
			GameObject instance = 
				Instantiate (wallTile, gridPositions[(columns*(row-1)+col)], Quaternion.identity) as GameObject;
			instance.transform.SetParent (boardContainer);
		}
	}

	void LayoutScouts()
	{
		int row = 2;
		for (int col = 2; col < columns - 4; col++)
		{
			//GameObject instance = 
			GameObject instance = Instantiate (scoutTile, gridPositions[(columns*(row-1)+col)], Quaternion.identity) as GameObject;
			instance.transform.SetParent (boardContainer);
			instance.tag = "Attacker_" + row + col;
		}
	}
	void LayoutGuards()
	{
		int row = 7;
		for (int col = 2; col < 5; col++)
		{
			//Vector3 pos = new Vector3(col,row, 0f);
			GameObject instance = Instantiate (guardTile, gridPositions[(columns*(row-1)+col)], Quaternion.identity) as GameObject;
			instance.transform.SetParent (boardContainer);
			instance.tag = "Defender_" + row + col;
		}
	}

	public void SetupScene()
	{
		BoardSetup ();

		InitList ();

		LayoutWalls ();

		LayoutScouts ();

		LayoutGuards ();
	}

	public static string PlayTurn() 
	{
		return playerTurn;
	}
	public static void SwitchPlayer() 
	{
		if (playerTurn == "Scout") 
		{
			playerTurn = "Guard";
		} 
		else {
			playerTurn = "Scout";
		}
	}

}
