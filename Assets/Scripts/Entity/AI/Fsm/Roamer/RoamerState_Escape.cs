using UnityEngine;
using System.Collections;

public class RoamerState_Escape : YBaseState<Roamer>
{
	public RoamerState_Escape(YStateMachine<Roamer> _sm) : base (_sm)
	{
//		m_dicStateMessageDelegate.Add(typeof(Msg_DetectEnemy), OnDetectEnemy);
	}
	
	public override void Enter(YMessage _msg)
	{
		Owner.CoroutineProc("Escape_EscapePlayer", true);
		Owner.CoroutineProc("Escape_CheckPlayer", true);
	}
	
	public override void Update()
	{
	}
	
	public override void Exit(YMessage _msg)
	{
		Owner.CoroutineProc("Escape_EscapePlayer", false);
		Owner.CoroutineProc("Escape_CheckPlayer", false);
	}
}

