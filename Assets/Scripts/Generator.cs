﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Generator : MonoBehaviour {

	public GameObject instructions;

	public float enemiesPerSecond;
	private float secondsPerEnemy;
	private float countdownToNextSpawn;

	private float ENEMY_RANGE_X_MIN;
	private float ENEMY_RANGE_X_MAX;
	private float ENEMY_RANGE_Y_MIN;
	private float ENEMY_RANGE_Y_MAX;
	private float SCREENSIZE = 25;

	private int environmentDegradation = 0;

	public GameObject enemyPrefab;
 //	public GameObject groundPlane;

	private List<GameObject> inGameObjects;
	private int enemySaturation = 0;
	private int ENEMY_SATURATION_LIMIT = 5;

	private string TAG_FINISH = "Finish";
	private string TAG_FUEL = "Fuel";
	private string TAG_PLAYER = "Player";

	// Use this for initialization
	void Start () {
		ResetSpawnEnemyProperties ();
		inGameObjects = new List<GameObject> ();

		inGameObjects.Add (GameObject.FindGameObjectWithTag (TAG_FINISH));
		inGameObjects.Add (GameObject.FindGameObjectWithTag (TAG_PLAYER));
		inGameObjects.AddRange (GameObject.FindGameObjectsWithTag (TAG_FUEL));
	}
	
	// Update is called once per frame
	void Update () {

//		Debug.Log (environmentDegradation);
		if (instructions.gameObject.activeInHierarchy) {
			return;
		}

		// increase enemy spawn rate every set number of obstacles player destroys
		if (environmentDegradation % 5 == 4) {
			environmentDegradation++;
			enemiesPerSecond += 0.2f;
			ResetSpawnEnemyProperties ();
		}

		countdownToNextSpawn -= Time.deltaTime;

		if (countdownToNextSpawn <= 0) {
			countdownToNextSpawn = secondsPerEnemy;
			CreateNewEnemy ();
		}
	}

	void CreateNewEnemy() {

		if (enemySaturation == ENEMY_SATURATION_LIMIT) {
			return;
		}

		bool intersecting = false;
		Vector3 newEnemyPosition = getNewEnemyPosition ();
		GameObject newEnemy = (GameObject) Instantiate(enemyPrefab, newEnemyPosition, Quaternion.identity);
		
		newEnemy.gameObject.transform.SetParent (this.transform);

		for (int i = 0; i < inGameObjects.Count; i++) {

			if (inGameObjects [i] == null) {
				continue;

			} else if (newEnemy.GetComponent<PolygonCollider2D>().bounds.Intersects(inGameObjects[i].GetComponent<PolygonCollider2D>().bounds)) {
				intersecting = true;
				break;
			}
		}
			

		if (intersecting) {
			enemySaturation++;
			Destroy (newEnemy);
			CreateNewEnemy ();

		} else {
			inGameObjects.Add (newEnemy);
			enemySaturation = 0;
		}
	}

	private void ResetSpawnEnemyProperties () {
		secondsPerEnemy = 1f / enemiesPerSecond;
		countdownToNextSpawn = secondsPerEnemy;
	}

	// xyvalues -> xmin, xmax, ymin, ymax
	public void SetInstantiateRange(Vector4 xyvalues) {
		ENEMY_RANGE_X_MIN = xyvalues.x;
		ENEMY_RANGE_X_MAX = xyvalues.y;
		ENEMY_RANGE_Y_MIN = xyvalues.z;
		ENEMY_RANGE_Y_MAX = xyvalues.w;
	}

	Vector3 getNewEnemyPosition() {
		Vector3 newPosition = new Vector3 ();
		if (Random.value * 100 < 80) {
			// 80% chance of spawning obstacle in the same screen.
			GameObject player = GameObject.FindGameObjectWithTag (TAG_PLAYER);
			newPosition.x = Random.Range (player.transform.position.x, player.transform.position.x + SCREENSIZE);
			newPosition.y = Random.Range (ENEMY_RANGE_Y_MIN, ENEMY_RANGE_Y_MAX);

		} else {
			newPosition = new Vector3 (Random.Range (ENEMY_RANGE_X_MIN, ENEMY_RANGE_X_MAX), Random.Range (ENEMY_RANGE_Y_MIN, ENEMY_RANGE_Y_MAX), 0);		
		}
			
		return newPosition;
	}
}
