using UnityEngine;
using System.Collections;

public class TempSound : MonoBehaviour
{
	public AudioSource aud;

	void Update()
	{
		if(!aud.isPlaying)
		{
			Destroy(this.gameObject);
		}
	}
}