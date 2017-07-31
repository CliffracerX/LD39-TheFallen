using UnityEngine;
using System.Collections;
using System;

public class LaunchpadScript : MonoBehaviour
{
	public BatteryScript[] primaryBats;
	public BatteryScript lifeSupport;
	public float percent,lifePercent;
	public GUIStyle perStyle;
	public float lifeDrainRate;
	public GameObject baddy;
	//public GameObject[] baddies;
	//public float[] chance;
	public int curWave;
	public float waveCooldown;
	public float mapScale;

	void Update()
	{
		float tempEU = 0;
		float tempMax = 0;
		waveCooldown-=Time.deltaTime;
		if(waveCooldown<=0)
		{
			waveCooldown=45f;
			curWave+=1;
			for(int i = 0; i<curWave; i++)
			{
				Vector3 tempting = new Vector3(UnityEngine.Random.Range(-mapScale, mapScale), UnityEngine.Random.Range(-mapScale, mapScale));
				Instantiate(baddy, tempting, Quaternion.identity);
			}
		}
		foreach(BatteryScript bs in primaryBats)
		{
			tempEU+=bs.eu;
			tempMax+=bs.maxEU;
		}
		percent = (tempEU/tempMax)*100;
		lifePercent = (lifeSupport.eu/lifeSupport.maxEU)*100;
		if(percent>=100)
		{
			Application.LoadLevel(2);
		}
		lifeSupport.UsePower(Time.deltaTime*lifeDrainRate);
		if(lifeSupport.eu<=0)
		{
			Application.LoadLevel(3);
		}
	}

	void OnGUI()
	{
		string perCut = Math.Round(percent, 3, MidpointRounding.AwayFromZero).ToString();
		GUI.Label(new Rect(Screen.width-300, 25, 275, 25), "POD CHARGE: "+perCut+"%", perStyle);
		string perCut2 = Math.Round(lifePercent, 3, MidpointRounding.AwayFromZero).ToString();
		GUI.Label(new Rect(Screen.width-300, 25+50, 275, 25), "STERILIZER CHARGE: "+perCut2+"%", perStyle);
	}
}