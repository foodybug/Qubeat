using UnityEngine;
using System.Collections;

public class GhostState_Idle : YBaseState<Ghost>
{
	public GhostState_Idle(YStateMachine<Ghost> _sm) : base (_sm)
	{
	}
	
	public override void Enter(YMessage _msg)
	{
		Owner.CoroutineProc("Idle_Roaming", true, _msg);
	}
	
	public override void Update()
	{
	}
	
	public override void Exit(YMessage _msg)
	{
		Owner.CoroutineProc("Idle_Roaming", false);
	}
}

