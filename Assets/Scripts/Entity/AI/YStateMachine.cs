using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class YStateMachine<T> where T : YBaseEntity
{
	T m_Owner;
	public T Owner{get{return m_Owner;}}
//	public eStateType StateType{get{return m_CurState.StateType;}}
	
	YBaseState<T> m_CurState;public YBaseState<T> CurState{get{return m_CurState;}}
	YBaseState<T> m_PreState;public YBaseState<T> PreState{get{return m_PreState;}}
	
//	Dictionary<eStateType, YBaseState<T>> m_dicState = new Dictionary<eStateType, YBaseState<T>>();
	Dictionary<System.Type, YBaseState<T>> m_dicState = new Dictionary<System.Type, YBaseState<T>>();
	
	public YStateMachine(T _entity)
	{
		m_Owner = _entity;
	}
	
	public void RegisterState(YBaseState<T> _state)
	{
		m_dicState.Add(_state.GetType(), _state);
	}
	
	public void ChangeState(System.Type _type, YMessage _msg)
	{
		if(m_CurState != null)
		{
			m_PreState = m_CurState;
			m_CurState.Exit(_msg);
		}
		
		SetState(_type);
		
		if(m_CurState != null)
			m_CurState.Enter(_msg);
	}
	
	public void ChangeState(System.Type _state)
	{
		ChangeState(_state, null);
	}
	
	public void Update()
	{
		if(m_CurState != null)
			m_CurState.Update();
	}
	
//	void SetGlobalState(eStateType _state)
//	{
//		if(m_dicState.ContainsKey(_state) == true)
//		{
//			m_GlobalState = m_dicState[_state];
//		}
//		else
//			Debug.Log("[YStateMachine]SetState:There is no enum state");
//	}
	
	void SetState(System.Type _state)
	{
		if(m_dicState.ContainsKey(_state) == true)
		{
			m_CurState = m_dicState[_state];
		}
		else
			Debug.LogError("[YStateMachine]SetState:There is no enum state");
	}
	
	public void MessageProcess(YMessage _msg)
	{
		if(m_CurState != null)
			m_CurState.MessageProcess(_msg);
	}
}

