﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {

	public float INITIAL_HP;
	private float currentHp;

	private float createTime;
	private bool creating;
	public AudioClip falling;
	private bool fallingPlaying = false;

	void Start () {
		currentHp = INITIAL_HP;
		createTime = Time.time;
		creating = true;
		GetComponent<PolygonCollider2D> ().enabled = false;
	}

	void Update () {

		if (Time.time - createTime < 0.5) {
			if (!fallingPlaying) {
				fallingPlaying = true;
				if (gameObject.GetComponent<Renderer> ().isVisible) {
					AudioSource.PlayClipAtPoint (falling, transform.position);
				}
			}

			Vector3 p = transform.position;
			p.x -= 2 * Time.deltaTime;
			p.y -= 6 * Time.deltaTime;
			transform.position = p;

			Color c = GetComponent<SpriteRenderer> ().color;
			c.a = (Time.time - createTime) / 0.85f;
			GetComponent<SpriteRenderer> ().color = c;

		} else {

			GetComponent<PolygonCollider2D> ().enabled = true;
			creating = false;
		}

		if (currentHp <= 0) {
			gameObject.GetComponent<AudioSource> ().Stop ();
			Destroy (this.gameObject);
		}
	}

	void LateUpdate() {
		if (creating) {
			return;
		}

		Color tempColor = this.gameObject.GetComponent<SpriteRenderer> ().color;
		tempColor.a = currentHp / INITIAL_HP;
		this.gameObject.GetComponent<SpriteRenderer> ().color = tempColor;
	}
	void OnCollisionExit2D(Collision2D other) {
		gameObject.GetComponent<AudioSource> ().Stop ();
	}

	void TakeDamage(float playerAttackRate) {
		currentHp -= Time.deltaTime * playerAttackRate;
		if (!gameObject.GetComponent<AudioSource> ().isPlaying) {
			gameObject.GetComponent<AudioSource> ().Play ();
		}
	}
}
