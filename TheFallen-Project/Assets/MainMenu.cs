using UnityEngine;
using System.Collections;

[System.Serializable]
public class EntryThing
{
	public string name;
	[TextArea]
	public string description;
	public Texture2D icon;
}

public class MainMenu : MonoBehaviour
{
	public enum Scr {MainScr=0, HelpScr=1, PlayScr=2}
	public Scr thisScr;
	public GameObject logo,webLogo;
	public GUIStyle style,selStyle;
	[TextArea]
	public string helpLabel;
	public EntryThing[] difs,chars;
	public static int curDif,curChar;

	void Start()
	{
		curDif = PlayerPrefs.GetInt("DifficultyPref");
		curChar = PlayerPrefs.GetInt("CharacterPref");
	}

	void OnGUI()
	{
		logo.SetActive(false);
		webLogo.SetActive(false);
		if(thisScr==Scr.MainScr)
		{
			logo.SetActive(true);
			if(Application.platform==RuntimePlatform.WebGLPlayer)
			{
				webLogo.SetActive(true);
			}
			if(GUI.Button(new Rect(Screen.width/2-196, Screen.height/2-80, 196*2, 64), "Play Game", style))
			{
				thisScr = Scr.PlayScr;
			}
			if(GUI.Button(new Rect(Screen.width/2-196, Screen.height/2, 196*2, 64), "Help", style))
			{
				thisScr = Scr.HelpScr;
			}
			if(GUI.Button(new Rect(Screen.width/2-196, Screen.height/2+80, 196*2, 64), "Quit Game", style))
			{
				Application.Quit();
			}
		}
		else if(thisScr==Scr.HelpScr)
		{
			if(GUI.Button(new Rect(25, 25, 196*2, 64), "Back to Main", style))
			{
				thisScr=Scr.MainScr;
			}
			GUI.Label(new Rect(Screen.width/2-600, 100, 1200, 1000), helpLabel, style);
		}
		else if(thisScr==Scr.PlayScr)
		{
			if(GUI.Button(new Rect(25, Screen.height-64, 196, 64), "Back to Main", style))
			{
				thisScr=Scr.MainScr;
			}
			for(int i = 0; i<difs.Length; i++)
			{
				if(GUI.Button(new Rect((Screen.width/4)-196, 25+(32*i), 196, 32), difs[i].name, curDif==i ? selStyle : style))
				{
					curDif=i;
					PlayerPrefs.SetInt("DifficultyPref", curDif);
				}
			}
			for(int i = 0; i<chars.Length; i++)
			{
				if(GUI.Button(new Rect((Screen.width/2)+(Screen.width/4), 25+(32*i), 196, 32), chars[i].name, curChar==i ? selStyle : style))
				{
					curChar=i;
					PlayerPrefs.SetInt("CharacterPref", curChar);
				}
			}
			GUI.Label(new Rect(25, Screen.height-368, Screen.width/3, 368), difs[curDif].description, style);
			GUI.Label(new Rect(Screen.width-(Screen.width/3)-25, Screen.height-368, Screen.width/3, 368), chars[curChar].description, style);
			if(GUI.Button(new Rect(Screen.width-196-25, Screen.height-64, 196, 64), "PLAY", style))
			{
				Application.LoadLevel(1);
			}
		}
	}
}