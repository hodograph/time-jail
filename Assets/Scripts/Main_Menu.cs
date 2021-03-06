using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Main_Menu : MonoBehaviour {

	public Texture title;
	public Texture start;
	public Texture quit;
	public Texture option;
	public Texture Level1png;
	public Texture Level2png;
	public Texture Level3png;
	public Texture Level4png;
	public Texture Level5png;
	public Texture Level6png;
	private bool mainMenu = true;

	void initialSave(){
		PlayerPrefs.SetInt ("isSaved", 1);
		PlayerPrefs.SetInt ("Level1", 0);
		PlayerPrefs.SetFloat ("Level1Time", 0.0f);
		PlayerPrefs.SetInt ("Level2", 0);
		PlayerPrefs.SetFloat ("Level2Time", 0.0f);
		PlayerPrefs.SetInt ("Level3", 0);
		PlayerPrefs.SetFloat ("Level3Time", 0.0f);
		PlayerPrefs.SetInt ("Level4", 0);
		PlayerPrefs.SetFloat ("Level4Time", 0.0f);
		PlayerPrefs.SetInt ("Level5", 0);
		PlayerPrefs.SetFloat ("Level5Time", 0.0f);
		PlayerPrefs.Save ();

	}

	// Use this for initialization
	void Start () {
		int isSaved = PlayerPrefs.GetInt ("isSaved", 0);
		if(isSaved == 0)
			initialSave ();

	}

	// Update is called once per frame
	void Update () {

	}

	void OnGUI(){
		if (mainMenu) {
			GUI.Label (new Rect (0, 0, Screen.width, Screen.height), title);
			GUIStyle blankStyle = new GUIStyle ();
			blankStyle.imagePosition = ImagePosition.ImageLeft;
			if (GUI.Button (new Rect (Screen.width / 2 - Screen.width / 20, Screen.height / 6 * 5, Screen.width / 5, Screen.height / 10), quit, blankStyle))
				Application.Quit();
			if (GUI.Button (new Rect (Screen.width / 2 - Screen.width / 20, Screen.height / 8 * 5, Screen.width / 5, Screen.height / 10), start, blankStyle))
				SceneManager.LoadScene ("Level One", LoadSceneMode.Single);
			if (GUI.Button (new Rect (Screen.width / 2 - Screen.width / 20, Screen.height / 4 * 3, Screen.width / 5, Screen.height / 10), option, blankStyle))
				mainMenu = false;
		} else {
			GUIStyle whiteStyle = new GUIStyle ("button");
			whiteStyle.normal.textColor = Color.white;
			if (GUI.Button (new Rect (Screen.width / 2 - Screen.width / 20, Screen.height / 6*1, Screen.width / 5, Screen.height / 10), Level1png, whiteStyle))
				SceneManager.LoadScene ("Level One", LoadSceneMode.Single);
			
			if (PlayerPrefs.GetInt ("Level1") == 1) {
				if (GUI.Button (new Rect (Screen.width / 2 - Screen.width / 20, Screen.height / 6*2, Screen.width / 5, Screen.height / 10), Level2png, whiteStyle))
					SceneManager.LoadScene ("Level Two", LoadSceneMode.Single);
			}else
				GUI.Label (new Rect (Screen.width/2 - Screen.width/20, Screen.height/6*2 , Screen.width/5, Screen.height/10), Level2png, whiteStyle);
			
			if (PlayerPrefs.GetInt ("Level2") == 1) {
				if (GUI.Button (new Rect (Screen.width / 2 - Screen.width / 20, Screen.height / 6*3, Screen.width / 5, Screen.height / 10), Level3png, whiteStyle))
					SceneManager.LoadScene ("Level Three", LoadSceneMode.Single);
			}else
				GUI.Label (new Rect (Screen.width/2 - Screen.width/20, Screen.height/6*3 , Screen.width/5, Screen.height/10), Level3png, whiteStyle);
			
			if (PlayerPrefs.GetInt ("Level3") == 1) {
				if (GUI.Button (new Rect (Screen.width / 2 - Screen.width / 20, Screen.height / 6 * 4, Screen.width / 5, Screen.height / 10), Level4png, whiteStyle))
					SceneManager.LoadScene ("Level Four", LoadSceneMode.Single);
			}else
				GUI.Label (new Rect (Screen.width/2 - Screen.width/20, Screen.height/6*4 , Screen.width/5, Screen.height/10), Level4png, whiteStyle);

			if (PlayerPrefs.GetInt ("Level4") == 1) {
				if (GUI.Button (new Rect (Screen.width / 2 - Screen.width / 20, Screen.height / 6 * 5, Screen.width / 5, Screen.height / 10), Level5png, whiteStyle))
					SceneManager.LoadScene ("RandomLevel 1", LoadSceneMode.Single);
			}else
				GUI.Label (new Rect (Screen.width/2 - Screen.width/20, Screen.height/6*5 , Screen.width/5, Screen.height/10), Level5png, whiteStyle);


	

				
		}

	}
}
