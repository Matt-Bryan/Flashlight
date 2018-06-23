using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class newGameManager : MonoBehaviour {
	private GameObject light;
	private float lightLeft = 55;
	private float lightRight = 305;

	private GameObject midMonster;
	private GameObject leftMonster;
	private GameObject rightMonster;
	private Camera camera;

	private GameObject deathSymbol;

	private float health = 100f;
	private float flashTimer = 0.0f;
	private float flashTimerMax = 60.0f;
	private Slider healthSlider;
	private Slider flashSlider;

	private float scoreTimer = 0.0f;

	private bool beingAttacked;

	private bool darkening = false;

	public AudioSource oofSoundSource;
	public AudioSource flashSoundSource;
	public AudioSource demonSoundSource;
	public GameObject restartButton;
	public GameObject scoreText;

	void Start () {
		light = GameObject.Find("Light");
		midMonster = GameObject.Find("middleMonster");
		leftMonster = GameObject.Find("leftMonster");
		rightMonster = GameObject.Find("rightMonster");
		deathSymbol = GameObject.Find("DeathSymbol");
		healthSlider = GameObject.Find("HealthBar").GetComponent<Slider>();
		flashSlider = GameObject.Find("FlashBar").GetComponent<Slider>();
		healthSlider.maxValue = health;
		flashSlider.maxValue = flashTimerMax;
		beingAttacked = false;
		camera = GetComponent<Camera>();
		demonSoundSource.Play();
		restartButton.GetComponent<Button>().onClick.AddListener(restart);
	}

	bool lightWithinBounds(float eulerNum) {
		return (!(eulerNum < 180 && eulerNum > lightLeft) && !(eulerNum > 180 && eulerNum < lightRight));
	}

	void lightCorrection() {
		float rotateNum = 0.0f;
		float eulerAnglesZ = light.transform.eulerAngles.z;
		if (eulerAnglesZ < lightLeft) {
			rotateNum = -0.1f;
		}
		else if (eulerAnglesZ > lightRight && eulerAnglesZ < 360) {
			rotateNum = 0.1f;
		}

		float eulerWithRotation = eulerAnglesZ + rotateNum;

		if (lightWithinBounds(eulerWithRotation)) {
			light.transform.RotateAround(new Vector3(0, -6, 0), new Vector3(0, 0, 1), rotateNum);
		}
	}

	void setAllMonsterActivation(bool newVal) {
		leftMonster.GetComponent<monster>().setMonsterActivation(newVal);
		midMonster.GetComponent<monster>().setMonsterActivation(newVal);
		rightMonster.GetComponent<monster>().setMonsterActivation(newVal);
	}

	void flash() {
		camera.backgroundColor = new Color(1, 1, 1);
		darkening = true;
		leftMonster.GetComponent<monster>().resetSize();
		midMonster.GetComponent<monster>().resetSize();
		rightMonster.GetComponent<monster>().resetSize();
		flashSoundSource.Play();
	}

	void takeInput() {
		float rotateNum = 0.0f;
		if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow)) {
			if (flashTimer < 0.01f) {
				flash();
				flashTimer = flashTimerMax;
			}
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow)) {
			rotateNum = -10.0f;
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			rotateNum = 10.0f;
		}

		float eulerWithRotation = light.transform.eulerAngles.z + rotateNum;

		if (lightWithinBounds(eulerWithRotation)) {
			light.transform.RotateAround(new Vector3(0, -6, 0), new Vector3(0, 0, 1), rotateNum);
		}
	}

	void updateBars() {
		healthSlider.value = health;
		flashSlider.value = -(flashTimer - flashTimerMax);
	}

	void checkMonsterSize() {
		beingAttacked = false;
		if (leftMonster.GetComponent<monster>().isMaxSize()) {
			health -= 0.3f;
			beingAttacked = true;
		}
		if (midMonster.GetComponent<monster>().isMaxSize()) {
			health -= 0.3f;
			beingAttacked = true;
		}
		if (rightMonster.GetComponent<monster>().isMaxSize()) {
			health -= 0.3f;
			beingAttacked = true;
		}

		if (beingAttacked && !oofSoundSource.isPlaying) {
			oofSoundSource.Play();
		}
		else if (!beingAttacked && oofSoundSource.isPlaying && health > 0.0f) {
			oofSoundSource.Stop();
		}
	}

	void gameOver() {
		demonSoundSource.Stop();
		oofSoundSource.Stop();

		setAllMonsterActivation(false);

		leftMonster.SetActive(false);
		midMonster.SetActive(false);
		rightMonster.SetActive(false);

		deathSymbol.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

		camera.backgroundColor = new Color(1, 1, 1);
		light.SetActive(false);

		restartButton.SetActive(true);

		scoreText.GetComponent<Text>().text = scoreTimer.ToString("F1") + "s";
	}

	public void restart() {
		restartButton.SetActive(false);

		health = 100.0f;
		flashTimer = 0.0f;
		scoreTimer = 0.0f;

		camera.backgroundColor = new Color(0, 0, 0);
		light.SetActive(true);

		deathSymbol.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

		leftMonster.SetActive(true);
		midMonster.SetActive(true);
		rightMonster.SetActive(true);

		leftMonster.GetComponent<monster>().resetSize();
		midMonster.GetComponent<monster>().resetSize();
		rightMonster.GetComponent<monster>().resetSize();

		setAllMonsterActivation(true);

		demonSoundSource.Play();
	}

	void updateCamera() {
		if (darkening) {
			camera.backgroundColor = new Color(camera.backgroundColor.r - 0.01f, camera.backgroundColor.g - 0.01f,
			 camera.backgroundColor.b - 0.01f);
			if (camera.backgroundColor.r == 0) {
				darkening = false;
			}
		}
	}

	void updateMusic() {
		float maxMonsterScale = Mathf.Max(Mathf.Max(leftMonster.transform.localScale.x, midMonster.transform.localScale.x), rightMonster.transform.localScale.x);

		demonSoundSource.volume = ((maxMonsterScale - 0.1f) / 4.9f) / 4.0f;
	}

	void updateTimers() {
		flashTimer -= Time.deltaTime;

		scoreTimer += Time.deltaTime;
		scoreText.GetComponent<Text>().text = scoreTimer.ToString("F1") + "s";
	}
	
	void Update () {
		if (health <= 0.0f) {
			gameOver();
			return;
		}
		updateTimers();
		updateMusic();
		takeInput();
		updateCamera();
		lightCorrection();
		checkMonsterSize();
		updateBars();
	}
}
