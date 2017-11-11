using UnityEngine;
using System.Collections;

public class PlayerPicker : MonoBehaviour
{
	public GameObject[] plays;

	void Start()
	{
		Instantiate(plays[MainMenu.curChar], Vector3.zero, plays[MainMenu.curChar].transform.rotation);
	}
}