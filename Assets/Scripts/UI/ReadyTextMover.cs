using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ReadyTextMover : MonoBehaviour {
	
	[SerializeField] float waitTime_ = 1.5f;
	[SerializeField] float lifeTime_ = 1f;
	[SerializeField] float speed_ = 200f;
	[SerializeField] int range_ = 40; 
	
	IEnumerator Start()
	{
		yield return new WaitForSeconds(waitTime_);
		GetComponent<Text>().text = "GO!!";
		
		Destroy(gameObject, lifeTime_);
		
		float floatFontSize = GetComponent<Text>().fontSize;
		int initFontSize = GetComponent<Text>().fontSize;
		
		bool uping = true;
		
		while(true)
		{
			yield return null;
			
			if(floatFontSize > initFontSize + range_)
				uping = false;
			if(floatFontSize < initFontSize)
				uping = true;
			
			float delta = Time.deltaTime / lifeTime_;
			if(uping == false)
				delta *= -1f;
			
			floatFontSize += delta * speed_;
			GetComponent<Text>().fontSize = (int)floatFontSize;
		}
	}
	
	void Update()
	{
	
	}
}
