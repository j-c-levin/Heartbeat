using UnityEngine;
using System.Collections;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class AirConsoleManager : MonoBehaviour
{
	private GameManager gameManager;

	void Awake ()
	{
		if (AirConsole.instance != null) {
			SetupAirConsoleEvents ();
		}
	}

	void Start ()
	{
		gameManager = GetComponent<GameManager> ();
	}

	void SetupAirConsoleEvents ()
	{
		AirConsole.instance.onReady += OnReady;
		AirConsole.instance.onMessage += OnMessage;
		AirConsole.instance.onConnect += OnConnect;
		AirConsole.instance.onDisconnect += OnDisconnect;
		AirConsole.instance.onDeviceStateChange += OnDeviceStateChange;
		AirConsole.instance.onCustomDeviceStateChange += OnCustomDeviceStateChange;
		AirConsole.instance.onDeviceProfileChange += OnDeviceProfileChange;
		AirConsole.instance.onAdShow += OnAdShow;
		AirConsole.instance.onAdComplete += OnAdComplete;
		AirConsole.instance.onGameEnd += OnGameEnd;
		Debug.Log ("Connecting...");
	}

	void OnReady (string code)
	{

	}

	void OnMessage (int from, JToken data)
	{
		Debug.Log ("Received from: " + from + " data: " + data.ToString ());
		AirConsole.instance.Message (from, "Full of pixels!");

		//Update player movement
		gameManager.SetHunterMovement ((float)data ["rightMovement"], (float)data ["upMovement"]);
	}

	void OnConnect (int device_id)
	{

	}

	void OnDisconnect (int device_id)
	{

	}

	void OnDeviceStateChange (int device_id, JToken data)
	{

	}

	void OnCustomDeviceStateChange (int device_id, JToken custom_data)
	{

	}

	void OnDeviceProfileChange (int device_id)
	{

	}

	void OnAdShow ()
	{

	}

	void OnAdComplete (bool adWasShown)
	{

	}

	void OnGameEnd ()
	{
		Debug.Log ("OnGameEnd is called");
	}

	void OnDestroy ()
	{
		// unregister events
		if (AirConsole.instance != null) {
			AirConsole.instance.onReady -= OnReady;
			AirConsole.instance.onMessage -= OnMessage;
			AirConsole.instance.onConnect -= OnConnect;
			AirConsole.instance.onDisconnect -= OnDisconnect;
			AirConsole.instance.onDeviceStateChange -= OnDeviceStateChange;
			AirConsole.instance.onCustomDeviceStateChange -= OnCustomDeviceStateChange;
			AirConsole.instance.onAdShow -= OnAdShow;
			AirConsole.instance.onAdComplete -= OnAdComplete;
			AirConsole.instance.onGameEnd -= OnGameEnd;
		}
	}
}
