using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void Dlt_Message(YMessage _msg); 

//public class MessageRegistry
//{
//	Dictionary<System.Type, Dlt_Message> m_dicDelegate = new Dictionary<System.Type, Dlt_Message>();
//	
//	public void RegisterDelegate(System.Type _type, Dlt_Message _delegate)
//	{
//		m_dicDelegate.Add(_type, _delegate);
//	}
//	
//	public void HandleMessage(YMessage _msg)
//	{
//		if(m_dicDelegate.ContainsKey(_msg.GetType()) == true)
//			m_dicDelegate[_msg.GetType()](_msg);
//	}
//}

public class YMessageProcessor : MonoBehaviour
{
	//#region - message -
	Dictionary<YBaseComponent, Dictionary<System.Type, Dlt_Message>> m_ddicDlt =
		new Dictionary<YBaseComponent, Dictionary<System.Type, Dlt_Message>>();
	
	public void RegisterReceiver(YBaseComponent _component, Dictionary<System.Type, Dlt_Message> _receiver)
	{
		m_ddicDlt.Add(_component, _receiver);
	}
	
	public void HandleMessage(YMessage _msg)
	{
		foreach(KeyValuePair<YBaseComponent, Dictionary<System.Type, Dlt_Message>> pair in m_ddicDlt)
		{
			if(pair.Value.ContainsKey(_msg.GetType()) == true)
				pair.Value[_msg.GetType()](_msg);
		}
	}
	//#endregion
}

