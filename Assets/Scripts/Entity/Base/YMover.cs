using UnityEngine;
using System.Collections;

public class YMover : YBaseComponent
{
	CharacterController m_Controller;
	
	bool m_Moving;
	Vector3 m_Destination;
	[SerializeField] float m_MoveSpeed = 0;
	float m_MoveSpeedRatio = 1f;
	
//	bool m_Player = false;
	
	void Awake()
	{
		m_Controller = GetComponent<CharacterController>();
		
		RegisterReceiver(typeof(Msg_Movement_Start), OnMovementStart);
		RegisterReceiver(typeof(Msg_Movement_Stop), OnMovementStop);
		RegisterReceiver(typeof(Msg_Movement_SpeedModify), OnMovementSpeedModify);
		
//		YBaseEntity entity = GetComponent<YBaseEntity>();
//		if(entity.GetType() == typeof(Player))
//			m_Player = true;
	}
	
	public override void Init()
	{
		base.Init();
	}
	
	void Update()
	{
		if(m_Moving == true)
		{
			Vector3 direction;
			direction = m_Destination - transform.position;
				
//			switch(m_DestinationType)
//			{
//			case eDestinationType.Position:
//				direction = m_Destination - transform.position;
//				break;
//			case eDestinationType.Target:
//				direction = m_Target.transform.position - transform.position;
//				break;
//			default:
//				direction = m_Destination - transform.position;
//				break;
//			}
			
			if(direction.magnitude > 0.1f)
			{
				Vector3 delta = direction.normalized * m_MoveSpeed * m_MoveSpeedRatio * Time.deltaTime;
//				if(m_Player == true)
//					delta /= Time.timeScale;
				
//				m_Controller.Move(delta);
				
				if(m_Controller != null)
					m_Controller.Move(delta);
				else
					transform.position += delta;
			}
			else
			{
				m_Moving = false;
			}
		}
	}
	
	#region - msg -
	void OnMovementStart(YMessage _msg)
	{
		Msg_Movement_Start move = _msg as Msg_Movement_Start;
		
		m_Destination = move.dest_;
		m_Moving = true;
	}
	
	void OnMovementStop(YMessage _msg)
	{
		m_Moving = false;
	}
	
	void OnMovementSpeedModify(YMessage _msg)
	{
		Msg_Movement_SpeedModify modify = _msg as Msg_Movement_SpeedModify;
		
		m_MoveSpeed = modify.speed_;
	}
	#endregion
	
	#region - public -
	public void SpeedModified(float _ratio)
	{
		m_MoveSpeedRatio = _ratio;
//		m_MoveSpeedRatio = _ratio * 0.5f;
//		if(m_MoveSpeedRatio > 1f)
//			m_MoveSpeedRatio = 1f;
		
//		Debug.Log("YMover::SpeedModified: _ratio = " + _ratio);
	}
	#endregion
}

