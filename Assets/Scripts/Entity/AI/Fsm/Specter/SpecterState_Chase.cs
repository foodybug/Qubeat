using UnityEngine;
using System.Collections;

public class SpecterState_Chase : YBaseState<Specter>
{
	public SpecterState_Chase(YStateMachine<Specter> _sm) : base (_sm)
	{
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
