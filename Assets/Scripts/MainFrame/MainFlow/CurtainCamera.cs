using UnityEngine;
using System.Collections;

public class CurtainCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
		DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void SM_Destroy()
	{
		Destroy(gameObject);
	}
}
