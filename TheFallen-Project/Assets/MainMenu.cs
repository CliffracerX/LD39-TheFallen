using UnityEngine;
using System.Collections;

[System.Serializable]
public class EntryThing
{
	public string name;
	[TextArea]
	public string description;
	[TextArea]
	public string lockDesc;
	public Texture2D icon;
}

public class MainMenu : MonoBehaviour
{
	public enum Scr {MainScr=0, HelpScr=1, PlayScr=2}
	public Scr thisScr;
	public GameObject logo,webLogo;
	public GUIStyle style,selStyle,lockStyle;
	[TextArea]
	public string helpLabel;
	public EntryThing[] difs,chars;
	public static int curDif,curChar;
	public bool[] difUnlocked,charUnlocked;
	public static MainMenu inst;

	public static void DifState(int dif, bool state)
	{
		inst.difUnlocked[dif]=state;
		PlayerPrefs.SetInt("DifUn:"+dif, state ? 1 : 0);
	}

	public static void CharState(int cha, bool state)
	{
		inst.charUnlocked[cha]=state;
		PlayerPrefs.SetInt("CharUn:"+cha, state ? 1 : 0);
	}

	void Start()
	{
		inst = this;
		curDif = PlayerPrefs.GetInt("DifficultyPref");
		curChar = PlayerPrefs.GetInt("CharacterPref");
		for(int i = 0; i<difUnlocked.Length; i++)
		{
			difUnlocked[i] = PlayerPrefs.GetInt("DifUn:"+i)==1 ? true : false;
		}
		for(int i = 0; i<charUnlocked.Length; i++)
		{
			charUnlocked[i] = PlayerPrefs.GetInt("CharUn:"+i)==1 ? true : false;
		}
		if(difUnlocked[0]==false)
		{
			difUnlocked[0] = true;
			PlayerPrefs.SetInt("DifUn:0", 1);
		}
		if(charUnlocked[0]==false)
		{
			charUnlocked[0] = true;
			PlayerPrefs.SetInt("CharUn:0", 1);
		}
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
			if(GUI.Button(new Rect(25, 25, 196*2, 64), "Back to Main", selStyle))
			{
				thisScr=Scr.MainScr;
			}
			GUI.Label(new Rect(Screen.width/2-600, 100, 1200, 1000), helpLabel, style);
		}
		else if(thisScr==Scr.PlayScr)
		{
			if(GUI.Button(new Rect(25, Screen.height-64, 196, 64), "Back to Main", selStyle))
			{
				thisScr=Scr.MainScr;
			}
			for(int i = 0; i<difs.Length; i++)
			{
				if(GUI.Button(new Rect((Screen.width/4)-196, 48+(32*i), 196, 32), difs[i].name, curDif==i ? selStyle : style))
				{
					curDif=i;
					PlayerPrefs.SetInt("DifficultyPref", curDif);
				}
			}
			GUI.Label(new Rect((Screen.width/4)-196, 0, 196, 32), "DIFFICULTY:", selStyle);
			for(int i = 0; i<chars.Length; i++)
			{
				if(GUI.Button(new Rect((Screen.width/2)+(Screen.width/4), 48+(32*i), 196, 32), chars[i].name, curChar==i ? selStyle : style))
				{
					curChar=i;
					PlayerPrefs.SetInt("CharacterPref", curChar);
				}
			}
			GUI.Label(new Rect((Screen.width/2)+(Screen.width/4), 0, 196, 32), "CHARACTERS:", selStyle);
			GUI.Label(new Rect(25, Screen.height-368, Screen.width/3, 368), difUnlocked[curDif] ? difs[curDif].description : difs[curDif].lockDesc, style);
			GUI.Label(new Rect(Screen.width-(Screen.width/3)-25, Screen.height-368, Screen.width/3, 368), charUnlocked[curChar] ? chars[curChar].description : chars[curChar].lockDesc, style);
			bool playAct = difUnlocked[curDif] && charUnlocked[curChar];
			if(GUI.Button(new Rect(Screen.width-196-25, Screen.height-64, 196, 64), "PLAY", playAct ? selStyle : lockStyle) && playAct)
			{
				Application.LoadLevel(1);
			}
		}
	}
}