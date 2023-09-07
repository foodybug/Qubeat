using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TouchScreenBtn : Button_GameOver {
	
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
		
//		if( Input.GetMouseButtonUp(0) == true)
//			ClickProcess();
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
		ClickProcess();
	}
	
	void ClickProcess()
	{
		if( Title.Instance.ChangeEnable == true)
		{
			MainFlow.Instance.SetInput(eInputType.Click);
			
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
