using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	//GUI Texture variables
	public Texture2D black_screen;
	public Texture2D splash_screen;
	public Texture2D background;
	public Texture2D play_button;

	//GUI fading variables
	private int splash_screen_phase;
	private float splash_screen_timer;
	private float splash_screen_duration;

	//Colors to draw GUI fade
	private Color normal_color;
	private Color alpha_color;

	void Start () {
		splash_screen_duration = 2.5f;
		splash_screen_timer = 0;
		splash_screen_phase = 1;
		normal_color = Color.white;
	}

	//Handling GUI fading progress
	void Update () {

		if (splash_screen_phase != 0) {

			if (splash_screen_phase == 1) {
				splash_screen_timer += Time.deltaTime;
				if (splash_screen_timer > splash_screen_duration) {
					splash_screen_phase = 2;
					splash_screen_timer = splash_screen_duration - (splash_screen_timer - splash_screen_duration);
				}
			} else if (splash_screen_phase == 2) {
				splash_screen_timer -= Time.deltaTime;
				if (splash_screen_timer < 0) {
					splash_screen_phase = 3;
				}
			} else if (splash_screen_phase == 3){
				splash_screen_timer -= Time.deltaTime;
				if (splash_screen_timer < -splash_screen_duration) {
					splash_screen_phase = 0;
				}
			}
		}

	}

	//Draw GUI fading
	void GUI_DisplaySplashScreen(){

		GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));


		if (splash_screen_phase == 1) {

			alpha_color = new Color(normal_color.r, normal_color.g, normal_color.b, Mathf.Abs(splash_screen_timer/splash_screen_duration));

			//Black background
			GUI.color = normal_color;
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), black_screen);

			//Splash
			GUI.color = alpha_color;
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), splash_screen);

		}else if (splash_screen_phase == 2) {

			alpha_color = new Color(normal_color.r, normal_color.g, normal_color.b, Mathf.Abs(splash_screen_timer/splash_screen_duration));

			//Black background
			GUI.color = normal_color;
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), black_screen);

			//Splash
			GUI.color = alpha_color;
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), splash_screen);

		}else if (splash_screen_phase == 3) {

			alpha_color = new Color(normal_color.r, normal_color.g, normal_color.b, 1-Mathf.Abs(splash_screen_timer/splash_screen_duration));

			//Black background
			GUI.color = alpha_color;
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), black_screen);

		}

		GUI.color = normal_color;

		GUI.EndGroup();
	}

	//Title Screen
	void OnGUI()
	{

		GUI.backgroundColor = Color.clear;

		GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));

		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background);
		GUI.DrawTexture(new Rect(Screen.width * 4 / 10, Screen.height * 7 / 10, Screen.width / 5, Screen.height / 5), play_button);

		GUI.EndGroup();

		if (splash_screen_phase != 0) {
			GUI_DisplaySplashScreen ();
		} else {
			if (GUI.Button(new Rect(Screen.width * 4 / 10, Screen.height * 7 / 10, Screen.width / 5, Screen.height / 5), "")){
				SceneManager.LoadScene(1,LoadSceneMode.Single);
			}
		}

	}
}
