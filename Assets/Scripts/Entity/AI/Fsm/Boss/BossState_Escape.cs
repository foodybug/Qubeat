using UnityEngine;
using System.Collections;

public class BossState_Escape : YBaseState<Boss>
{
	public BossState_Escape(YStateMachine<Boss> _sm) : base (_sm)
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

