using UnityEngine;
using System.Collections;

public enum eInputState {NORMAL, FLEETCONFIGURATION, FLEETDECOMPOSITION, DIALOG}

#region - input message -
public class InputMessage
{
	protected eInputState m_InputState;
	public eInputState InputState{get{return m_InputState;}}
}
public class InputDownMessage : InputMessage
{
	RaycastHit m_RaycastHit;
	public RaycastHit raycastHit{get{return m_RaycastHit;}}
	
	public InputDownMessage(eInputState _state, RaycastHit _raycastHit)
	{
		m_InputState = _state;
		m_RaycastHit = _raycastHit;
	}
}
public class InputDragMessage : InputMessage
{
	Vector3 m_PosDelta;
	public Vector3 PosDelta{get{return m_PosDelta;}}
	
	public InputDragMessage(eInputState _state, Vector3 _posDelta)
	{
		m_InputState = _state;
		m_PosDelta = _posDelta;
	}
}
public class InputUpMessage : InputMessage
{
	RaycastHit m_RaycastHit;
	public RaycastHit raycastHit{get{return m_RaycastHit;}}
	bool m_WasClicked = false;
	public bool WasClicked{get{return m_WasClicked;}}
	
	public InputUpMessage(eInputState _state, bool _wasClicked, RaycastHit _raycastHit)
	{
		m_InputState = _state;
		m_WasClicked = _wasClicked;
		m_RaycastHit = _raycastHit;
	}
}
#endregion

public class YInputManager : MonoBehaviour
{
	#region - singleton -
	static YInputManager instance;
	public static YInputManager Instance
	{get{return instance;}}
	#endregion
	
	bool m_Activation = true;
	eInputState m_CurInputState;
	public eInputState InputState{get{return m_CurInputState;}set{m_CurInputState = value;}}
	
	YBaseEntity m_FirstClickEntity = null;
	Vector3 m_FirstClickPos = Vector3.zero;
	Vector3 m_PreDragPos = Vector3.zero;
	float m_ClickedTime = 0;
	
	void Awake()
	{
		instance = this;
	}
	
	void Start ()
	{
		
	}
	
	public void SetActivation(bool _active)
	{
		m_Activation = _active;
	}

	void Update ()
	{
		if(m_Activation == true)
		{
			MouseProcess();
			if(m_CurInputState == eInputState.NORMAL)
				SetDragArea();
		}
	}
	
	RaycastHit hit;
	void MouseProcess(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if(Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.NameToLayer("Distance_Checker")) == true)
		{
			if(Input.GetMouseButtonDown(0) == true){
				m_FirstClickPos = hit.transform.position;
				m_PreDragPos = hit.transform.position;
				m_ClickedTime = Time.time;
				
				YBaseEntity hitEntity = GetPickedEntity(hit);
				m_FirstClickEntity = hitEntity;
			}
		
			if(Input.GetMouseButton(0) == true){
//				Vector3 posDelta = hit.transform.position - m_PreDragPos;
				m_PreDragPos = hit.transform.position;
				
//				YBaseEntity hitEntity = GetPickedEntity(hit);
			}
		
			if(Input.GetMouseButtonUp(0) == true){
				YBaseEntity hitEntity = GetPickedEntity(hit);
				ClickEntityProcess(hitEntity);
				ClickGroundProcess(hit);
				
				m_FirstClickEntity = null;
			}
		}
	}
	
	bool m_bDragging = false;
	Vector3 m_DraggingPos = Vector3.zero;
	
	void SetDragArea()
	{
		if(Input.GetMouseButtonDown(0) == true){
			m_FirstClickPos = Input.mousePosition;
			m_bDragging = true;
		}
	
		if(Input.GetMouseButton(0) == true){
			m_DraggingPos = Input.mousePosition;
		}
	
		if(Input.GetMouseButtonUp(0) == true){
			m_DraggingPos = Input.mousePosition;
//			if((Time.time - m_ClickedTime) > PublicDefine.MouseDragThreshold)
//			{
//				YEntityManager.Instance.AreaSelection(m_FirstClickPos, m_DraggingPos);
//			}
			m_bDragging = false;
		}
	}
	
	#region - input process -
	void ClickEntityProcess(YBaseEntity _entity)
	{
		if(_entity != null)
		{
//			if(_entity.EntityType == eEntityType.Player && YEntityManager.Instance.PlayerEntity == null)
//			{
////				_entity.HandleMessage(new Msg_EntityClick());
//				Debug.Log("ClickProcess: Id[" + _entity.Id + "] is clicked");
//			}
		}
	}
	
	void ClickGroundProcess(RaycastHit _hit)
	{
		if(_hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			Vector3 pos = _hit.point;
//			pos.y = YStageManager.Instance.GroundHeight;
			YEntityManager.Instance.MessageToUserEntity(new Msg_Input_Move(pos));
		}
	}
	#endregion
	
	#region - method -
	YBaseEntity GetPickedEntity(RaycastHit _hit)
	{
		return YEntityManager.Instance.GetEntityByInstanceID(_hit.transform.gameObject.GetInstanceID());
	}
	#endregion
	
	public bool showInput_ = false;	
	void OnGUI()
	{
		if(showInput_ == true)
		{
			Rect rect = new Rect(900, 500, 100, 30);
			
			if(m_FirstClickEntity != null)
			{
				GUI.Label(rect, "First clicked Entity:" + m_FirstClickEntity.GetType());
				rect.y += 30;
				GUI.Label(rect, "First clicked pos:" + m_FirstClickPos);
			}
			
			if(m_bDragging == true)
			{
				rect.y += 30;
				GUI.Label(rect, "First clicked pos:" + m_PreDragPos);
				rect.y += 30;
				GUI.Label(rect, "First clicked pos:" + m_ClickedTime);
				rect.y += 30;
				GUI.Label(rect, "First clicked pos:" + m_DraggingPos);
			}
		}
	}
}