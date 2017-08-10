using UnityEngine;
using System.Collections;

public class BatteryScript : PowerObject
{
	public float eu,maxEU;
	public Sprite[] sprites;
	public Sprite[] emptySprites;
	public float[] lightIntens;
	public int tempTick;
	public float blinkTick,blinkTickSpeed;
	public SpriteRenderer sr;
	public Light flashLight;
	public float uses;
	public bool autoMaxes = true;
	public float[] maxEUFromDif;

	void Start()
	{
		this.maxEU = maxEUFromDif[MainMenu.curDif];
	}

	public override void Highlight()
	{
		sr.color = new Color(0.5f, 1.0f, 0.0f);
	}

	public override void Unhighlight()
	{
		sr.color = new Color(1.0f, 1.0f, 1.0f);
	}

	public override bool GetPower(float amount)
	{
		if(eu<=maxEU)
		{
			eu+=amount;
			return true;
		}
		else return false;
	}

	public override bool HasPower(float amount)
	{
		if(eu>=amount)
		{
			return true;
		}
		else return false;
	}

	public override bool UsePower(float amount)
	{
		if(eu>=amount)
		{
			eu-=amount;
			return true;
		}
		else
		{
			eu=0;
			return false;
		}
	}

	public override float GetPowerUse()
	{
		return uses;
	}

	void Update()
	{
		//DO THINGS TO SYNC THINGS;
		if(eu>maxEU && autoMaxes)
		{
			eu=maxEU;
		}
		float euM = eu/maxEU;
		int thing = (int)(euM*sprites.Length);
		if(thing>=sprites.Length)
			thing=sprites.Length-1;
		if(thing==0)
		{
			blinkTick-=Time.deltaTime;
			if(blinkTick<0)
			{
				blinkTick=blinkTickSpeed;
				tempTick+=1;
				if(tempTick>=emptySprites.Length)
					tempTick=0;
			}
			sr.sprite=emptySprites[tempTick];
			flashLight.intensity=lightIntens[tempTick];
		}
		else
		{
			flashLight.intensity=0;
			sr.sprite=sprites[thing];
		}
	}
}