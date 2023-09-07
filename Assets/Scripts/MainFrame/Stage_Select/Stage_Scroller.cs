using UnityEngine;
using System.Collections;

public class Stage_Scroller : MonoBehaviour {
	
	[SerializeField] float m_FrameRate = 0.02f;
	
	[SerializeField] ScrollerBeacon m_StartBeacon;
	[SerializeField] ScrollerBeacon m_EndBeacon;
	
	[SerializeField] float m_DragSpeed = 0.1f;
	
	[SerializeField] Vector3 m_StartPos;
	[SerializeField] Vector3 m_EndPos;
	
	Rect m_Area;

	void Start()
	{
		m_StartPos = m_StartBeacon.transform.position;
		m_EndPos = m_EndBeacon.transform.position;

//		m_StartBeacon.transform.position = m_StartPos;
//		m_EndBeacon.transform.position = m_EndPos;
		
		SetDragArea();
	}
	
	void OnDisable()
	{
		RefreshPos();
	}
	
	void Update()
	{
//		if(Input.GetMouseButtonDown(0) == true && StageBtn.s_Moving == false)
//		{
//			StartCoroutine(Drag_CR());
//		}
	}
	
	void LateUpdate()
	{
		if(Input.GetMouseButtonDown(0) == true && StageBtn.s_Moving == false &&
			(MainFlow.Instance == null || MainFlow.Instance.SceneChanging == false))
		{
			StartCoroutine(Drag_CR());
		}
	}
	
	IEnumerator Drag_CR()
	{
//		Vector3 beginPos = Input.mousePosition;
		Vector3 prevPos = Input.mousePosition;
		
		while(Input.GetMouseButton(0))// && Stage_Select.EditMode == false)
		{
			yield return new WaitForSeconds(m_FrameRate);
//			yield return null;
			
			Vector3 curPos = Input.mousePosition;
	        Vector3 dir = curPos - prevPos;
	
	        Vector3 move = new Vector3(-dir.x * m_DragSpeed, -dir.y * m_DragSpeed, 0);
			move *= Time.deltaTime;
//	        transform.Translate(move, Space.World);
			
			Vector3 result = transform.position + move;
			if(CheckValidPos(result) == true)
			{
				transform.position += move;
				prevPos = curPos;
			}
		}
	}
	
	void SetDragArea()
	{
		m_Area = new Rect(Mathf.Min(m_StartPos.x, m_EndPos.x),
			Mathf.Min(m_StartPos.y, m_EndPos.y),
			Mathf.Abs(m_StartPos.x - m_EndPos.x),
			Mathf.Abs(m_StartPos.y - m_EndPos.y));
	}
	
	bool CheckValidPos(Vector3 _pos)
	{
		if(m_Area.Contains(_pos) == true)
			return true;
		else
			return false;
	}
	
	void RefreshPos()
	{
//		m_StartPos = m_StartBeacon.transform.position;
//		m_EndPos = m_EndBeacon.transform.position;
	}
	
	#region - msg -
	void SetDragArea_Stage_Scroller()
	{
		RefreshPos();
		SetDragArea();
	}
	#endregion
	
//	void OnDrawGizmos()
//	{
//		Gizmos.DrawWireCube(m_Area.center, new Vector3(m_Area.width, m_Area.height, 0));
//	}
}
