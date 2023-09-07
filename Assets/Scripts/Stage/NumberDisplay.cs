using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NumberDisplay : MonoBehaviour {
	
	Text m_GuiText;
	
	[SerializeField] int fontSize_ = 20;
	
	// Use this for initialization
	void Start () {
		GameObject obj = new GameObject("number");
		m_GuiText = obj.GetComponent<Text>();
		m_GuiText.text = name;
		m_GuiText.fontSize = fontSize_;
		//m_GuiText.rectTransform.anchor anchor = TextAnchor.LowerCenter;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void LateUpdate()
	{
		Vector3 pos = transform.position;
		Vector3 screenPos = Camera.main.WorldToViewportPoint(pos);
		m_GuiText.transform.position = screenPos;
		
		//switch(m_GuiText.anchor)
		//{
		//case TextAnchor.LowerCenter:
		//case TextAnchor.LowerLeft:
		//case TextAnchor.LowerRight:
		//	screenPos.y += 0.05f;
		//	m_GuiText.transform.position = screenPos;
		//	break;
		//case TextAnchor.UpperCenter:
		//case TextAnchor.UpperLeft:
		//case TextAnchor.UpperRight:
		//	screenPos.y -= 0.05f;
		//	m_GuiText.transform.position = screenPos;
		//	break;
		//}
	}
	
	void OnDestroy()
	{
		if(m_GuiText != null)
			Destroy(m_GuiText.gameObject);
	}
}
