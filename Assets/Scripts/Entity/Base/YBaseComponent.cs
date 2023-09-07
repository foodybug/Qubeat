using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class YBaseComponent : MonoBehaviour {
	
	//#region - member
	YMessageProcessor m_MsgProcessor = null;
	//#endregion

	//#region - message -
	Dictionary<System.Type, Dlt_Message> m_dicDlt = new Dictionary<System.Type, Dlt_Message>();
	
	protected void RegisterReceiver(System.Type _type, Dlt_Message _delegate)
	{
		m_dicDlt.Add(_type, _delegate);
	}
	
	public void HandleMessage(YMessage _msg)
	{
		m_MsgProcessor.HandleMessage(_msg);
	}
	//#endregion
	
	public virtual void Init()
	{
//		if(msgProcessor == null)
//			msgProcessor = gameObject.AddComponent<YMessageProcessor>();
//		else
			
		if(m_MsgProcessor == null)
		{
			YMessageProcessor msgProcessor = GetComponent<YMessageProcessor>();
			
			if(msgProcessor != null)
				m_MsgProcessor = msgProcessor;
			else
				m_MsgProcessor = gameObject.AddComponent<YMessageProcessor>();
		}
		
		if(m_MsgProcessor != null)
		{
			m_MsgProcessor.RegisterReceiver(this, m_dicDlt);
		}
		else
		{
			Debug.LogError("YBaseComponent::Init: component<YMessageProcessor> is not found. registering message process is skip.");
		}
	}

	protected void ReleaseReceiver()
	{
		m_dicDlt.Clear();
	}
}
