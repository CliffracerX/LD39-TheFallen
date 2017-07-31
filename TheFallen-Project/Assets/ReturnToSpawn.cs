using UnityEngine;
using System.Collections;

public class ReturnToSpawn : MonoBehaviour
{
	void Update()
	{
		if(Input.anyKeyDown)
		{
			Application.LoadLevel(0);
		}
	}
}