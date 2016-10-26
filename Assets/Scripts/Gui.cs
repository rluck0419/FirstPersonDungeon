using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Gui : MonoBehaviour {

	public Texture2D crosshair;
	public Rect centerPosition;
	public Rect scorePosition;
	public int score_count = 0;
 
 	// Use this for initialization
	void Start () {
		centerPosition = new Rect((Screen.width - crosshair.width) / 2, (Screen.height - crosshair.height) /2, crosshair.width, crosshair.height);
		scorePosition = new Rect (0f, 0f, 100f, 20f);
 	}

	void OnGUI() {
    	GUI.DrawTexture (centerPosition, crosshair);
	}
}
