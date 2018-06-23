using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monster : MonoBehaviour {

	private float scale;
	private float minSize = 0.1f;
	private float maxSize = 5.0f;
	private bool inLight;
	private bool monsterActive;

	void Start () {
		scale = minSize;
		monsterActive = true;
		inLight = false;
	}

	public void setMonsterActivation(bool newVal) {
		monsterActive = newVal;
	}

	public bool isMaxSize() {
		return transform.localScale.x > (maxSize - 0.1f);
	}

	public void resetSize() {
		transform.localScale = new Vector3(minSize, minSize, 1);
	}
	
	void Update () {
		if (monsterActive) {
			if (!inLight) {
				scale = 0.01f;
			}
			else {
				scale = -0.01f;
			}

			transform.localScale += new Vector3(scale, scale, 0);

			if (transform.localScale.x < minSize) {
				transform.localScale = new Vector3(minSize, minSize, 0);
			}
			else if (transform.localScale.x > maxSize) {
				transform.localScale = new Vector3(maxSize, maxSize, 0);
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D other) {
		inLight = true;
	}

	private void OnTriggerExit2D(Collider2D other) {
		inLight = false;
	}
}
