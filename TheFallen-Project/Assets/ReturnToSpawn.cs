using UnityEngine;
using System.Collections;

[System.Serializable]
public class Unlocker
{
	public int unlockCharInt,unlockDifInt,difficulty,reqChar;
	public bool dlock = true,clock = true;

	public void Unlock()
	{
		if((MainMenu.curDif==difficulty || difficulty==-1) && (MainMenu.curChar==reqChar || reqChar==-1))
		{
			if(unlockDifInt!=-1)
			{
				MainMenu.DifState(unlockDifInt, dlock);
			}
			if(unlockCharInt!=-1)
			{
				MainMenu.CharState(unlockCharInt, clock);
			}
		}
	}
}

public class ReturnToSpawn : MonoBehaviour
{
	public int level = 0;
	public float timeTill = 1f;
	public bool unlocks;
	public Unlocker[] unlock;

	void Start()
	{
		if(unlocks)
		{
			foreach(Unlocker u in unlock)
			{
				u.Unlock();
			}
		}
	}

	void Update()
	{
		timeTill -= Time.deltaTime;
		if(Input.anyKeyDown && timeTill<=0)
		{
			Application.LoadLevel(level);
		}
	}
}