using UnityEngine;
using System.Collections;

public class AndroidCommunicator : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
//	void OnGUI()
//    {
//        if(GUI.Button(new Rect(100,100,100,100) , "launch"))
//        {
//            if( Application.platform != RuntimePlatform.Android )
//                return;
//
//            // Load package
//            AndroidJavaClass pluginClass = new AndroidJavaClass( "com.dts.DtsPlugin" );
//			Debug.Log(pluginClass);
//
//            // Get class Instance
//            AndroidJavaObject _plugin = pluginClass.CallStatic<AndroidJavaObject>( "Instance" );
//			Debug.Log(_plugin);
//
//            // call Java Method.
////            _plugin.Call( "ShowToast", "Test Tost 2", true );
//			_plugin.Call( "ShowAlert", "1", "This is Alert Box", "Yeeees", "Canceeeel" );
//        }
//    }
}
