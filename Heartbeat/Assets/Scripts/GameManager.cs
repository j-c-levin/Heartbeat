using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
	public Light heartbeatLight;
	public float heartbeatFadeRate;
	public float heartbeatGrowRate;
	public float heartbeatIntensityModifier;

	public Transform hunter;
	public Transform prey;

	public float playerMovement;
	public float baseHeartbeatRate;
	public float heartbeatDistanceModifier;
	public float roundDuration;
	public float hunterPowerSpeedBoost;
	public float hunterPowerDuration;
	public float preyPowerDuration;
	public float abilityRegenRate;

	public Text timeCounter;

	private float m_realDistance;
	private float m_duration;
	private float m_timeRemaining;

	private bool m_preyPower;
	private bool m_hunterPower;
	private float m_hunterPowerDurationRemaining;
	private float m_preyPowerDurationRemaining;

	private GAMESTATE currentState;

	private Coroutine heartbeatFaderCoroutine;

	private enum GAMESTATE
	{
		PRE_GAME,
		GAME,
		HUNTER_WIN,
		PREY_WIN
	}

	public float CalculatedHunterMovement {
		get {
			float speed = playerMovement * Time.deltaTime;

			if (HunterPowerEnabled ()) {
				speed += hunterPowerSpeedBoost;
			}
				
			return speed;
		}
		set {
			playerMovement = value;
		}
	}

	// Use this for initialization
	void Start ()
	{
		heartbeatFaderCoroutine = StartCoroutine (HeartbeatLightFader (0, 1));
		StopCoroutine (heartbeatFaderCoroutine);
		StartCoroutine ("Heartbeat");

		currentState = GAMESTATE.GAME;

		m_timeRemaining = roundDuration;

		m_hunterPowerDurationRemaining = hunterPowerDuration / 2;
		m_preyPowerDurationRemaining = preyPowerDuration / 2;
	}

	// Update is called once per frame
	void Update ()
	{
		if (currentState != GAMESTATE.GAME) {
			return;
		}

		UpdateAbilityTimers ();
		UpdateHunterMovement ();
		WinCondition ();
	}

	void UpdateHunterMovement ()
	{
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");
		hunter.transform.Translate (new Vector3 (horizontal * CalculatedHunterMovement, vertical * CalculatedHunterMovement, 0));

		float xDistance = hunter.transform.position.x - prey.transform.position.x;
		float yDistance = hunter.transform.position.y - prey.transform.position.y;
		m_realDistance = Mathf.Sqrt ((xDistance * xDistance) + (yDistance * yDistance));

		m_duration = baseHeartbeatRate * m_realDistance - (heartbeatDistanceModifier * (1 / m_realDistance));

		m_duration = Mathf.Clamp (m_duration, 0.05f, 1);
	}

	void WinCondition ()
	{
		if (m_realDistance <= hunter.transform.localScale.x) {
			Debug.Log ("Hunter has won");
			currentState = GAMESTATE.HUNTER_WIN;
		}

		m_timeRemaining -= Time.deltaTime;

		if (m_timeRemaining <= 0) {
			Debug.Log ("Prey has won");
			currentState = GAMESTATE.PREY_WIN;
		}
			
		string[] timeArray = m_timeRemaining.ToString ().Split ('.');

		string time = timeArray [0].Replace ("-", "") + ".";

		//Error checking for when the time is a whole number with no decimal point
		if (timeArray.Length > 1) {
			int size = (timeArray [1].Length > 2) ? 2 : timeArray [1].Length;
			time += timeArray [1].Substring (0, size);
		}

		timeCounter.text = time;
	}

	void UpdateAbilityTimers ()
	{
		if (Input.GetKey ("space")) {
			m_hunterPower = true;
		} else {
			m_hunterPower = false;
		}

		if (Input.GetKey ("b")) {
			m_preyPower = true;
		} else {
			m_preyPower = false;
		}

		if (m_hunterPower) {
			m_hunterPowerDurationRemaining -= Time.deltaTime;
		} else {
			m_hunterPowerDurationRemaining += Time.deltaTime / abilityRegenRate;
		}

		m_hunterPowerDurationRemaining = Mathf.Clamp (m_hunterPowerDurationRemaining, 0, hunterPowerDuration);

		if (m_preyPower) {
			m_preyPowerDurationRemaining -= Time.deltaTime; 
		} else {
			m_preyPowerDurationRemaining += Time.deltaTime / abilityRegenRate;
		}

		m_preyPowerDurationRemaining = Mathf.Clamp (m_preyPowerDurationRemaining, 0, preyPowerDuration);
	}

	float Intensity {
		get {
			return heartbeatIntensityModifier * (1 / m_realDistance);
		}
	}

	IEnumerator Heartbeat ()
	{
		StopCoroutine (heartbeatFaderCoroutine);
		heartbeatFaderCoroutine = StartCoroutine (HeartbeatLightFader (0, heartbeatFadeRate));
		prey.GetComponent<Renderer> ().material.color = new Color (0.09f, 0.4f, 0, 1); 

		yield return new WaitForSeconds (m_duration);

		if (!PreyPowerEnabled ()) {
			StopCoroutine (heartbeatFaderCoroutine);
			heartbeatFaderCoroutine = StartCoroutine (HeartbeatLightFader (Intensity, heartbeatGrowRate));
			prey.GetComponent<Renderer> ().material.color = new Color (1, 1, 1, 1);
		}

		yield return new WaitForSeconds (m_duration / 3);

		StopCoroutine (heartbeatFaderCoroutine);
		heartbeatFaderCoroutine = StartCoroutine (HeartbeatLightFader (0, heartbeatFadeRate));
		prey.GetComponent<Renderer> ().material.color = new Color (0.09f, 0.4f, 0, 1); 

		yield return new WaitForSeconds (m_duration / 3);

		if (!PreyPowerEnabled ()) {
			StopCoroutine (heartbeatFaderCoroutine);
			heartbeatFaderCoroutine = StartCoroutine (HeartbeatLightFader (Intensity, heartbeatGrowRate));
			prey.GetComponent<Renderer> ().material.color = new Color (1, 1, 1, 1);
		}

		yield return new WaitForSeconds (m_duration / 3);

		if (currentState == GAMESTATE.GAME) {
			StartCoroutine ("Heartbeat");
		}
	}

	IEnumerator HeartbeatLightFader (float intensity, float duration)
	{
		heartbeatLight.intensity = Mathf.Lerp (heartbeatLight.intensity, intensity, duration);
		yield return new WaitForFixedUpdate ();
		heartbeatFaderCoroutine = StartCoroutine (HeartbeatLightFader (intensity, duration));
	}

	private bool PreyPowerEnabled ()
	{
		return (m_preyPower && m_preyPowerDurationRemaining > 0);
	}

	private bool HunterPowerEnabled ()
	{
		return (m_hunterPower && m_hunterPowerDurationRemaining > 0);
	}
}