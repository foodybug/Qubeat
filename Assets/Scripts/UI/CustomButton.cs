using UnityEngine;
using System.Collections;

public delegate void CustomButton_dlt();

public class CustomButton : MonoBehaviour {
	
//	static System.Collections.Generic.List<CustomButton> listButton = null;
	
	CustomButton_dlt m_Del;

	// Use this for initialization
	void Start () {
	
	}
	
	public void Init( CustomButton_dlt _del)
	{
		m_Del = _del;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseUpAsButton()
	{
		if( m_Del != null)
			m_Del();
		
//		if(listButton == null)
//			listButton = new System.Collections.Generic.List<CustomButton>();
//		
//		listButton.Add(_btn);
	}
	
//	void OnDestroy()
//	{
//		if(listButton != null)
//		{
//			listButton.Clear();
//			listButton = null;
//		}
//	}
//	
//	public static void ButtonSelected_Blink( CustomButton _btn)
//	{
//		foreach( CustomButton node in listButton)
//		{
//			node.guiText.enabled = false;
//		}
//	}
}
