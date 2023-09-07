using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void StateMessageDelegate(YMessage _msg);

public abstract class YBaseState<T> where T : YBaseEntity {
	
//	public abstract eStateType StateType{get;}
	protected YStateMachine<T> m_StateMachine;
	protected T Owner{get{return m_StateMachine.Owner;}}
	
	protected Dictionary<System.Type, StateMessageDelegate> m_dicStateMessageDelegate =
		new Dictionary<System.Type, StateMessageDelegate>();
	
	protected YBaseState(YStateMachine<T> _sm)
	{
		m_StateMachine = _sm;
	}
	
	public virtual void Enter(YMessage _msg)
	{
	}
	
	public virtual void Update()
	{
		
	}
	
	public virtual void Exit(YMessage _msg)
	{
	}
	
	public void MessageProcess(YMessage _msg)
	{
		if(m_dicStateMessageDelegate.ContainsKey(_msg.GetType()) == true)
		{
			m_dicStateMessageDelegate[_msg.GetType()](_msg);
		}
	}
}