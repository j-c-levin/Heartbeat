﻿using UnityEngine;
using System.Collections;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class AirConsoleManager : MonoBehaviour
{
	private GameManager m_gameManager;

	void Awake ()
	{
		if (AirConsole.instance != null) {
			SetupAirConsoleEvents ();
		}
	}

	void Start ()
	{
		m_gameManager = GetComponent<GameManager> ();
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
		Debug.Log ("Ready: " + code);
	}

	void OnMessage (int from, JToken data)
	{ 
		if ((object)data ["joystick-left"] != null) {
			ReceivedMovement (from, data);
			return;
		}

		if ((object)data ["special"] != null) {
			ReceivedSpecial (from, data);
			return;
		}

		Debug.LogError ("unknown input: " + data.ToString ());
	}

	void ReceivedMovement (int from, JToken data)
	{
		if (!(bool)data ["joystick-left"] ["pressed"]) { 
			m_gameManager.SetHunterMovement (0, 0);
			return;
		}

		//Update player movement
		m_gameManager.SetHunterMovement ((float)data ["joystick-left"] ["message"] ["x"], -(float)data ["joystick-left"] ["message"] ["y"]);
	}

	void ReceivedSpecial (int from, JToken data)
	{
		m_gameManager.SetHunterPower ((bool)data ["special"] ["pressed"]);
	}

	void OnConnect (int device_id)
	{
		AirConsole.instance.Message (device_id, "Hello back!");
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
