using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialDisplay : MonoBehaviour {
	
	TutorialManager.eDisplayType m_Type = TutorialManager.eDisplayType.NONE;
	Text m_GuiText;
	
	public void Init(string _str, int _size, TutorialManager.eDisplayType _type)
	{
		m_Type = _type;
		
		GameObject obj = new GameObject("'" + _str + "'");
		//m_GuiText = obj.AddComponent<GUIText>();
		m_GuiText.text = _str;
		m_GuiText.fontSize = _size;
		//m_GuiText.anchor = TextAnchor.LowerCenter;
	}
	
	public void SetDestroyTime(float _time)
	{
		Destroy(gameObject, _time);
	}
	
	public void SetReleaseTime(float _time)
	{
		Destroy(this, _time);
	}
	
	public void SetAnchor(TextAnchor _anchor)
	{
		//m_GuiText.anchor = _anchor;
	}
	
	public void SetColor(Color _color)
	{
		m_GuiText.material.color = _color;
	}
	
	// Use this for initialization
	void Start () {
	
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
	
	void EntityDeath_TutorialDisplay()
	{
		switch(m_Type)
		{
		case TutorialManager.eDisplayType.NONE:
			break;
		case TutorialManager.eDisplayType.Smaller:
			TutorialManager.Instance.SmallerDeath_TutorialManager(this);
			break;
		case TutorialManager.eDisplayType.Bigger:
			break;
		case TutorialManager.eDisplayType.Boss:
			TutorialManager.Instance.BossDeath_TutorialManager(this);
			break;
		}
		
		if(m_GuiText != null)
			Destroy(m_GuiText.gameObject);
		
		Destroy(this);
	}
}
