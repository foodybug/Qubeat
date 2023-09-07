using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Retry_Click : Button_GameOver {
	
	void Awake()
	{
		AddButtons(this);
	}
	
	// Use this for initialization
	void Start () {
		
//		InvokeRepeating("Blink", 0.5f, 0.5f);
		StartCoroutine(Blink());
	}
	
	void Selected()
	{
		GetComponent<Text>().enabled = !GetComponent<Text>().enabled;
	}
	
	IEnumerator Blink()
	{
//		int size = guiText.fontSize;
		
		while(true)
		{
			yield return new WaitForSeconds(0.9f);
			
			GetComponent<Text>().enabled = false;
//			guiText.fontSize = (int)((float)size * 0.7f);
			
			yield return new WaitForSeconds(0.1f);
			
			GetComponent<Text>().enabled = true;
//			guiText.fontSize = size;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
//		if(TouchOrButtonUp() == true)
//		{
////			RaycastHit hit;
//			RaycastHit[] hits = Physics.RaycastAll(Camera.mainCamera.ScreenPointToRay(Input.mousePosition));
//			
//			foreach(RaycastHit node in hits)
//			{
//				Debug.Log(node.transform.name);
//				
//				if(node.transform.gameObject == transform.gameObject)
//				{
//					MainFlow.Instance.Click_Retry();
//					
////					InvokeRepeating("TurnOn", 0f, 0.2f);
////					InvokeRepeating("TurnOff", 0.2f, 0.2f);
//				}
//			}
//			
////			if(Physics.Raycast(Camera.mainCamera.ScreenPointToRay(Input.mousePosition), out hit) == true)
////			{
////				if(hit.transform.gameObject == transform.gameObject)
////				{
////					MainFlow.Instance.Click_Retry();
////					
////					InvokeRepeating("TurnOn", 0f, 0.2f);
////					InvokeRepeating("TurnOff", 0.2f, 0.2f);
////				}
////			}
//		}
	}
	
	bool TouchOrButtonUp()
	{
		if(Input.GetMouseButtonUp(0) == true ||
			(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended))
			return true;
		else
			return false;
	}
	
	void OnMouseUpAsButton()
	{
		//MainFlow.Instance.Click_Retry();
		
		TurnOffButtons();
		InvokeRepeating("Selected", 0f, 0.1f);
					
//		InvokeRepeating("TurnOn", 0f, 0.2f);
//		InvokeRepeating("TurnOff", 0.2f, 0.2f);
	}
	
	void OnDestroy()
	{
		ReleaseButtons();
	}
}
