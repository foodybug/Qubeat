using UnityEngine;
using System.Collections;

public class RoamerState_Chase : YBaseState<Roamer>
{
	public RoamerState_Chase(YStateMachine<Roamer> _sm) : base (_sm)
	{
//		m_dicStateMessageDelegate.Add(typeof(Msg_DetectEnemy), OnDetectEnemy);
	}
	
	public override void Enter(YMessage _msg)
	{
		Owner.CoroutineProc("Chase_ChasePlayer", true);
		Owner.CoroutineProc("Chase_CheckPlayer", true);
	}
	
	public override void Update()
	{
	}
	
	public override void Exit(YMessage _msg)
	{
		Owner.CoroutineProc("Chase_ChasePlayer", false);
		Owner.CoroutineProc("Chase_CheckPlayer", false);
	}
}

