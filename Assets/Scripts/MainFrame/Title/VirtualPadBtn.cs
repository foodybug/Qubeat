using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VirtualPadBtn : Button_GameOver {
	
	void Awake()
	{
		AddButtons(this);
	}
	
	// Use this for initialization
	IEnumerator Start () {
		
		GetComponent<Text>().enabled = false;
		
		yield return new WaitForSeconds(1f);
		
		GetComponent<Text>().enabled = true;
		
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
			yield return new WaitForSeconds(0.95f);
			
			GetComponent<Text>().enabled = false;
//			guiText.fontSize = (int)((float)size * 0.7f);
			
			yield return new WaitForSeconds(0.05f);
			
			GetComponent<Text>().enabled = true;
//			guiText.fontSize = size;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
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
		if( Title.Instance.ChangeEnable == true)
		{
			MainFlow.Instance.SetInput(eInputType.Virtual_Pad);
			
			Title.Instance.OnGameStart();
			
			TurnOffButtons();
			InvokeRepeating("Selected", 0f, 0.1f);
		}
	}
	
	void OnDestroy()
	{
		ReleaseButtons();
	}
}

public class Button_Title : MonoBehaviour
{
	static System.Collections.Generic.List<Button_GameOver> listButton = null;
	
	protected void AddButtons(Button_GameOver _btn)
	{
		if(listButton == null)
			listButton = new System.Collections.Generic.List<Button_GameOver>();
		
		listButton.Add(_btn);
	}
	
	protected void ReleaseButtons()
	{
		if(listButton != null)
		{
			listButton.Clear();
			listButton = null;
		}
	}
	
	protected static void TurnOffButtons()
	{
		foreach(Button_GameOver node in listButton)
		{
//			node.CancelInvoke();
			node.StopAllCoroutines();
			node.GetComponent<Text>().enabled = false;
		}
	}
}