using UnityEngine;
using System.Collections;

public class ReturnToSpawn : MonoBehaviour
{
	public int level = 0;
	public float timeTill = 1f;

	void Update()
	{
		timeTill -= Time.deltaTime;
		if(Input.anyKeyDown && timeTill<=0)
		{
			Application.LoadLevel(level);
		}
	}
}