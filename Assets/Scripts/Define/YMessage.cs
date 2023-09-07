using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class YMessage{}

public class Msg_Blank : YMessage {}

#region - user control -
public class Msg_Input_Move : YMessage
{
	public Vector3 pos_;
	
	public Msg_Input_Move(Vector3 _pos)
	{
		pos_ = _pos;
	}
}
#endregion

#region - move & spin -
public class Msg_Movement_Start : YMessage
{
	public Vector3 dest_;
	public Vector3 cur_;
	
	public Msg_Movement_Start(Vector3 _dest)
	{
		dest_ = _dest;
	}
	
	public Msg_Movement_Start(Vector3 _dest, Vector3 _cur)
	{
		dest_ = _dest;
		cur_ = _cur;
	}
}

public class Msg_Movement_Stop : YMessage
{
}

public class Msg_Movement_SpeedModify : YMessage
{
	public float speed_;
	
	public Msg_Movement_SpeedModify(float _speed)
	{
		speed_ = _speed;
	}
}

public class Msg_Spin_Start : YMessage
{
}

public class Msg_Spin_Stop : YMessage
{
}

public class Msg_Spin_SpeedModify : YMessage
{
	public float speed_;
	
	public Msg_Spin_SpeedModify(float _speed)
	{
		speed_ = _speed;
	}
}

public class Msg_Spin_AngleModify : YMessage
{
}

public class Msg_GhostMemory : YMessage
{
	public Vector3 pos_;
	public float speed_;
	
	public Msg_GhostMemory(Vector3 _pos, float _speed)
	{
		pos_ = _pos;
		speed_ = _speed;
	}
}
#endregion
#region - collision -
public class Msg_CollisionActive : YMessage
{
	public bool active_;
	
	public Msg_CollisionActive(bool _active)
	{
		active_ = _active;
	}
}

public class Msg_CollisionSize : YMessage
{
	public float adjust_;

	public Msg_CollisionSize()
	{

	}

	public Msg_CollisionSize(float _adjust)
	{
		adjust_ = _adjust;
	}
}

public class Msg_CollisionOccurred : YMessage
{
	public bool levelUp_ = false;

	public YCollider col_;

	public bool explode = true;
	
	public Msg_CollisionOccurred()
	{
	}
	
	public Msg_CollisionOccurred(YCollider _col)
	{
		col_ = _col;
	}
	
	public Msg_CollisionOccurred SetCollision(YCollider _col)
	{
		col_ = _col;
		
		return this;
	}

	public Msg_CollisionOccurred SetByLevelUp()
	{
		levelUp_ = true;

		return this;
	}

	public Msg_CollisionOccurred SetAsNoneExplode()
	{
		explode = false;

		return this;
	}
}

public class Msg_PlayerDeath : YMessage
{
	public Msg_PlayerDeath()
	{
	}
}
#endregion

#region - player action -
public class Msg_PlayerLevelUp : YMessage
{
	public int lv_;
	
	public Msg_PlayerLevelUp(){}
	public Msg_PlayerLevelUp(int _lv)
	{
		lv_ = _lv;
	}
}

public class Msg_PlayerInvincible : YMessage
{
}

public class Msg_BossDefeated : YMessage
{

}

public class Msg_LevelDown : YMessage
{

}

public class Msg_PlayerLevelChanged : YMessage
{

}
#endregion

#region - item -
public class Msg_ItemUsingIndicate : YMessage
{
	public int index_;
	
	public Msg_ItemUsingIndicate(int _idx)
	{
		index_ = _idx;
	}
}

public class Msg_BerserkIndicate : YMessage
{
	public float time_ = 10;
	public float attackSpeedRatio_ = 0.1f;
	
//	public Msg_BerserkIndicate(float _time)
//	{
//		time_ = _time;
//	}
}

public class Msg_UseExaltation : YMessage
{
	public float range_ = 2;
	public int amount_ = 10;
}

public class Msg_ExaltationIndicate : YMessage
{
	public float time_ = 5;
	public float attackIncrease = 20;
}

public class Msg_UseAssembly : YMessage
{
	public float range_ = 2;
	public int amount_ = 10;
}

public class Msg_AssemblyIndicate : YMessage
{
	public YBaseEntity commander_;
	public float time_ = 30;
	
	public Msg_AssemblyIndicate(YBaseEntity _commander)
	{
		commander_ = _commander;
	}
}

public class Msg_UseDissolution : YMessage
{
	public float range_ = 3;
}

public class Msg_DissolutionIndicate : YMessage
{
}

public class Msg_HoneyIndicate : YMessage
{
}

public class Msg_PrincessIndicate :YMessage
{
}
#endregion