using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	private Rigidbody ball_rb;
	public GameObject camera_object;
	public GameObject pivot_object;
	public GameObject ref_object;
	public GameObject point_prefab;
	public GameObject bonus_prefab;

	Vector2 mousePos;
	float camera_angle_x;

	bool paused;
	bool out_of_bounds;
	bool time_out;

	float spawn_timer;

	float game_score;
	float game_timer;
	float pivot_timer;
	float pivot_duration;

	public Texture2D[] gui_textures;
	GUIStyle guistyle_banner_text;
	GUIStyle guistyle_pause_text;

	void Reset(){

		pivot_object.transform.eulerAngles = Vector3.zero;
		transform.position = new Vector3 (0, 3.5f, 0);
		ball_rb.velocity = Vector3.zero;
		ball_rb.angularVelocity = Vector3.zero;

		time_out = false;
		out_of_bounds = false;
		mousePos = Input.mousePosition;
		game_score = 0;
		camera_angle_x = 0;
		pivot_timer = 0;
		game_timer = 60;
		spawn_timer = 0;

		Unpause ();

	}

	void Start () {
		time_out = false;
		out_of_bounds = false;
		paused = false;
		mousePos = Input.mousePosition;
		game_score = 0;
		camera_angle_x = 0;
		ball_rb = GetComponent<Rigidbody> ();
		pivot_duration = 20.0f;
		pivot_timer = 0;
		game_timer = 60;
		ConstructGUIStyles ();
		spawn_timer = 0;
	}

	void ConstructGUIStyles(){
		guistyle_banner_text = new GUIStyle();
		guistyle_banner_text.fontSize = Screen.height / 24;
		guistyle_banner_text.normal.textColor = Color.white;
		guistyle_banner_text.alignment = TextAnchor.MiddleCenter;

		guistyle_pause_text = new GUIStyle();
		guistyle_pause_text.fontSize = Screen.height / 12;
		guistyle_pause_text.normal.textColor = Color.yellow;
		guistyle_pause_text.alignment = TextAnchor.MiddleCenter;
	}

	void Pause(){
		Time.timeScale = 0;
		paused = true;
	}

	void Unpause(){
		Time.timeScale = 1;
		paused = false;
	}

	void DestroyLevel(){
		GameObject[] points_to_remove = GameObject.FindGameObjectsWithTag ("Point");
		for (int i = points_to_remove.Length; i > 0; i--) {
			GameObject.Destroy(points_to_remove[i-1]);
		}
		GameObject[] bonuses_to_remove = GameObject.FindGameObjectsWithTag ("Bonus");
		for (int i = bonuses_to_remove.Length; i > 0; i--) {
			GameObject.Destroy(bonuses_to_remove[i-1]);
		}
	}

	void Update () {

		if (!out_of_bounds && !time_out) {
			if (Vector3.Distance (new Vector3 (0, 3.5f, 0), transform.position) > 126) {
				out_of_bounds = true;
				Pause ();
				DestroyLevel ();
			}

			if (Input.GetButtonDown("Jump")){

				if (!paused) {
					Pause ();
				} else {
					Unpause ();
				}

			}


			game_timer -= Time.deltaTime;
			if (game_timer > 60) {
				game_timer = 60;
			}
			if (game_timer < 0) {
				time_out = true;
				Pause ();
				DestroyLevel ();
			}

			pivot_timer += Time.deltaTime;
			if (pivot_timer > pivot_duration) {
				pivot_timer -= pivot_duration;
			}

			//pivot_object.transform.LookAt(new Vector3(100,0,0));

			pivot_object.transform.LookAt (new Vector3(100, 100.0f * Mathf.Cos (pivot_timer * Mathf.PI * 2.0f / pivot_duration), 100.0f * Mathf.Sin (pivot_timer * Mathf.PI * 2.0f / pivot_duration)));

			spawn_timer += Time.deltaTime;
			if (spawn_timer > 2) {
				spawn_timer -= 2;
				Spawn ();
			}

		}


		if (!paused) {
			float x = Input.mousePosition.x - mousePos.x;
			camera_angle_x += x;

			camera_object.transform.localEulerAngles = new Vector3(30,camera_angle_x,0);

			float camera_pos_x = transform.localPosition.x -20 * Mathf.Sin (Mathf.Deg2Rad * camera_angle_x);
			float camera_pos_y = 20;
			float camera_pos_z = transform.localPosition.z -20 * Mathf.Cos (Mathf.Deg2Rad  * camera_angle_x);

			camera_object.transform.localPosition = new Vector3 (camera_pos_x, camera_pos_y, camera_pos_z);
			ref_object.transform.localPosition = new Vector3(camera_pos_x, 3.5f, camera_pos_z);

			Vector3 direction = transform.position - ref_object.transform.position;
			direction.Normalize ();

			if (Input.GetButtonDown("Fire1")){

				ball_rb.AddForce (direction * 250);

			}
		}

		mousePos = Input.mousePosition;



		Vector3 grounded = transform.localPosition;
		grounded.y = 3.5f;
		transform.localPosition = grounded;

	}

	void OnTriggerEnter(Collider other){ //Called by child Trigger

		if (other.tag == "Bonus") {

			Destroy(other.gameObject);
			game_timer += 5;

		}else if (other.tag == "Point") {

			Destroy(other.gameObject);
			game_score ++;

		}

	}

	void Spawn(){

		float radius = 50.0f;
		float x = Random.Range(-1.0f,1.0f);
		float z = Random.Range (-Mathf.Sin (Mathf.Acos (x)), Mathf.Sin (Mathf.Acos (x)));

		x *= radius;
		z *= radius;

		if (Random.Range (1, 10) > 7) {
			GameObject inst = (GameObject)Instantiate (bonus_prefab); 
			inst.transform.position = new Vector3 (x, 100, z);
		} else {
			GameObject inst = (GameObject)Instantiate (point_prefab); 
			inst.transform.position = new Vector3 (x, 100, z);
		}

	}

	void OnGUI(){
		GUI_Constant ();

		if (paused) {
			GUI_PauseScreen ();
		}
	}

	void GUI_Constant(){
		GUI.backgroundColor = Color.clear;

		GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height/16));

		GUI.Label(new Rect(0,0,Screen.width / 2,Screen.height / 16), "TIMER "+game_timer.ToString("00"), guistyle_banner_text);
		GUI.Label(new Rect(Screen.width / 2,0,Screen.width / 2,Screen.height / 16), "SCORE "+game_score.ToString("00"), guistyle_banner_text);

		GUI.EndGroup();

	}

	void GUI_PauseScreen(){

		GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));

		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), gui_textures[0]);
		GUI.Label(new Rect(Screen.width / 4,Screen.height / 16,Screen.width / 2,Screen.height / 8), "Speed Ball", guistyle_pause_text);
		GUI.Label(new Rect(Screen.width / 4,Screen.height * 3 / 16,Screen.width / 2,Screen.height / 8), "Score : "+game_score, guistyle_pause_text);
		GUI.DrawTexture(new Rect(Screen.width / 4,Screen.height * 9 / 16,Screen.width / 2,Screen.height / 8), gui_textures[1]);

		if (out_of_bounds) {
			GUI.Label (new Rect (Screen.width / 4, Screen.height * 6 / 16, Screen.width / 2, Screen.height / 8), "You fell off the map!", guistyle_pause_text);
		} else if (time_out) {
			GUI.Label (new Rect (Screen.width / 4, Screen.height * 6 / 16, Screen.width / 2, Screen.height / 8), "Time's up!", guistyle_pause_text);
		}

		if (!out_of_bounds && !time_out) {
			GUI.Label (new Rect (Screen.width / 4, Screen.height * 9 / 16, Screen.width / 2, Screen.height / 8), "RESUME", guistyle_pause_text);
		} else {
			GUI.Label (new Rect (Screen.width / 4, Screen.height * 9 / 16, Screen.width / 2, Screen.height / 8), "RETRY", guistyle_pause_text);
		}

		GUI.DrawTexture(new Rect(Screen.width / 4,Screen.height * 12 / 16,Screen.width / 2,Screen.height / 8), gui_textures[1]);
		GUI.Label(new Rect(Screen.width / 4,Screen.height * 12 / 16,Screen.width / 2,Screen.height / 8), "MAIN MENU", guistyle_pause_text);

		GUI.EndGroup();

		if (!out_of_bounds && !time_out) {
			if (GUI.Button(new Rect(Screen.width / 4,Screen.height * 9 / 16,Screen.width / 2,Screen.height / 8), "")){
				Unpause ();
			}
		} else {
			if (GUI.Button(new Rect(Screen.width / 4,Screen.height * 9 / 16,Screen.width / 2,Screen.height / 8), "")){
				Reset ();
			}
		}


		if (GUI.Button(new Rect(Screen.width / 4,Screen.height * 12 / 16,Screen.width / 2,Screen.height / 8), "")){
			SceneManager.LoadScene(0,LoadSceneMode.Single);
		}

	}

}
