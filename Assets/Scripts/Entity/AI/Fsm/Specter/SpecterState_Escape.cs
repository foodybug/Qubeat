using UnityEngine;
using System.Collections;

public class SpecterState_Escape : YBaseState<Specter>
{
	public SpecterState_Escape(YStateMachine<Specter> _sm) : base (_sm)
	{
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
