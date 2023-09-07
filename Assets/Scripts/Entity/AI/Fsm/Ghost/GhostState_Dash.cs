using UnityEngine;
using System.Collections;

public class GhostState_Dash : YBaseState<Ghost>
{
	public GhostState_Dash(YStateMachine<Ghost> _sm) : base (_sm)
	{
	}
	
	public override void Enter(YMessage _msg)
	{
		Owner.CoroutineProc("Dash_Moving", true,_msg);
	}
	
	public override void Update()
	{
	}
	
	public override void Exit(YMessage _msg)
	{
		Owner.CoroutineProc("Dash_Moving", false);
	}
}

