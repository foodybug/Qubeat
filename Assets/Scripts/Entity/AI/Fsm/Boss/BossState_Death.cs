using UnityEngine;
using System.Collections;

public class BossState_Death : YBaseState<Boss>
{
	public BossState_Death(YStateMachine<Boss> _sm) : base (_sm)
	{
//		m_dicStateMessageDelegate.Add(typeof(Msg_DetectEnemy), OnDetectEnemy);
	}
	
	public override void Enter(YMessage _msg)
	{
		Owner.HandleMessage(new Msg_CollisionActive(false));
		Owner.CoroutineProc("Death", true, _msg);
	}
	
	public override void Update()
	{
	}
	
	public override void Exit(YMessage _msg)
	{
	}
}

