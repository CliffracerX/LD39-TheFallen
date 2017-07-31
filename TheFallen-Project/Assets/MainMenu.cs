using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
	public enum Scr {MainScr=0, HelpScr=1}
	public Scr thisScr;
	public GameObject logo;
	public GUIStyle style;
	[TextArea]
	public string helpLabel;

	void OnGUI()
	{
		logo.SetActive(false);
		if(thisScr==Scr.MainScr)
		{
			logo.SetActive(true);
			if(GUI.Button(new Rect(Screen.width/2-196, Screen.height/2-80, 196*2, 64), "Play Game", style))
			{
				Application.LoadLevel(1);
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
	}
}