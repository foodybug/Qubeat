using UnityEngine;
using System.Collections;

public class ScreenSetting : MonoBehaviour 
{
	void Start()
	{
		Screen.SetResolution( 800, 480, true );
	}
	
	void OnApplicationPause(bool _pause)
	{
		if(_pause == false)
			Screen.SetResolution( 800, 480, true );
	}
}
