using UnityEngine;
using System.Collections;

public class BossState_Idle : YBaseState<Boss>
{
	public BossState_Idle(YStateMachine<Boss> _sm) : base (_sm)
	{
	}
	
	public override void Enter(YMessage _msg)
	{
		Owner.CoroutineProc("Idle_Roaming", true);
        Owner.CoroutineProc("Idle_Spinning", true);
        //		Owner.Idle_CheckPlayer");
    }
	
	public override void Update()
	{
	}
	
	public override void Exit(YMessage _msg)
	{
		Owner.CoroutineProc("Idle_Roaming", false);
        Owner.CoroutineProc("Idle_Spinning", false);
        //		Owner.StopCoroutine("Idle_CheckPlayer");
    }
}

