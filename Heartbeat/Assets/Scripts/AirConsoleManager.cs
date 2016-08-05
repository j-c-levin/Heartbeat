using UnityEngine;
using System.Collections;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class AirConsoleManager : MonoBehaviour
{
	void Start ()
	{
		if (AirConsole.instance != null) {
			AirConsole.instance.onMessage += OnMessage;
		}
	}

	void OnMessage (int from, JToken data)
	{
		Debug.Log ("Received from: " + from + " data: " + data.ToString ());
		AirConsole.instance.Message (from, "Full of pixels!");
	}
}
