using UnityEngine;
using System.Collections;

public class EntityStatus : ScriptableObject {
	
	public PlayerStatus playerStatus_;
	public RoamerStatus roamerStatus_;
	public StoneStatus stoneStatus_;
	public StalkerStatus stalkerStatus_;
	public BulletStatus bulletStatus_;
    public DancerStatus dancerStatus_;
    public SpecterStatus specterStatus_;

    public int clearedStageIdx_;
	
	public Vector3[] stageBtnPos_;
	public int LastStageIndex{get{return stageBtnPos_.Length;}}
	
	[SerializeField] Texture2D m_ColorPallete;
	
	public float[] difficulty_;
	
	public static readonly float s_UnitUV = 8;
	public Color GetColorByUv(Vector2 _uv)
	{
		return m_ColorPallete.GetPixel((int)(_uv.x * s_UnitUV), (int)(_uv.y * s_UnitUV));
	}
	
	public bool tutorialFinished_
	{
		get{
//			if(clearedStageIdx_ == 0)
			if(PlayerPrefs.GetInt("ClearedStageIdx", 0) == 0)
				return false;
			else
				return true;
		}
	}
}

[System.Serializable]
public class PlayerStatus
{
	public float hp_ = 100;
	public float hpMax_ = 100;
	public float attack_ = 10;
	public float moveSpeed_ = 1;
}

[System.Serializable]
public class RoamerStatus
{
	public float hp_ = 100;
	public float hpMax_ = 100;
	public float attack_ = 10;
	public float moveSpeed_ = 1;
}

[System.Serializable]
public class StoneStatus
{
	public float hp_ = 100;
	public float hpMax_ = 100;
	public float attack_ = 10;
	public float moveSpeed_ = 1;
}

[System.Serializable]
public class StalkerStatus
{
	public float hp_ = 100;
	public float hpMax_ = 100;
	public float attack_ = 10;
	public float moveSpeed_ = 1;
}

[System.Serializable]
public class BulletStatus
{
	public float hp_ = 100;
	public float hpMax_ = 100;
	public float attack_ = 10;
	public float moveSpeed_ = 1;
}

[System.Serializable]
public class DancerStatus
{
    public float hp_ = 100;
    public float hpMax_ = 100;
    public float attack_ = 10;
    public float moveSpeed_ = 3.5f;
}

[System.Serializable]
public class SpecterStatus
{
    public float hp_ = 100;
    public float hpMax_ = 100;
    public float attack_ = 10;
    public float moveSpeed_ = 1;
}