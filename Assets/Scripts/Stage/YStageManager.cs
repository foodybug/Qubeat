using UnityEngine;
using System.Collections;
	
public class YStageManager : MonoBehaviour
{
	#region - singleton -
	static YStageManager instance;
	public static YStageManager Instance
	{get{return instance;}}
	#endregion
	#region - member -
	public static readonly float s_StdStageSize = 400f;
	
	[SerializeField] Vector2 m_StageSize = new Vector2(20, 20);public Vector2 StageSize{get{return m_StageSize;}}
	float m_GroundHeight = 0;public float GroundHeight{get{return m_GroundHeight;}}
	
	[SerializeField] Vector3 m_StdCameraPos = new Vector3(0, 5, 0);public Vector3 StdCameraPos{get{return m_StdCameraPos;}}
	public static readonly eRegion userRegion_ = eRegion.Blue;

	//[SerializeField] float m_ForeGroundDepth = -2f;
	//[SerializeField] float m_BackGroundDepth = -5f;

	//GameObject m_ForeGround;
	//GameObject m_BackGround;

    ParticleSystem m_BG_Particle;
    [SerializeField] BG_ParticleInfo m_BG_ParticleInfo;
	#endregion

	#region - init & update -
	void Awake()
	{
		instance = this;
	}
	
	public void SetStageInfo(float _size)
	{
		float realSize = s_StdStageSize * _size * _size;
		float sqrt = Mathf.Sqrt(realSize);
		m_StageSize = new Vector2(sqrt, sqrt);
		
		CreateStage(m_StageSize);
	}
	
	void Start ()
	{
		InvokeRepeating("CheckPlayerLevel", 0, 0.1f);
		//StartCoroutine("ChangingGridColor");
        //StartCoroutine("ChangingEdgeColor");
        StartCoroutine("ChangingBG_Particle_CR");	
	}
	
	public void CreateStage(Vector2 _size)
	{
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
		obj.AddComponent<YGround>();
		
		GameObject objLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
		objLeft.layer = LayerMask.NameToLayer("Wall");
		objLeft.AddComponent<Rigidbody>().useGravity = false;
		objLeft.GetComponent<Rigidbody>().isKinematic = true;
//		Destroy(objLeft.collider);
		objLeft.transform.localScale = new Vector3(m_StageSize.x * 2, 1f, m_StageSize.y * 2);
		objLeft.transform.position = new Vector3(-m_StageSize.x * 2, 0, 0);
		
		GameObject objRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
		objRight.layer = LayerMask.NameToLayer("Wall");
		objRight.AddComponent<Rigidbody>().useGravity = false;
		objRight.GetComponent<Rigidbody>().isKinematic = true;
//		Destroy(objRight.collider);
		objRight.transform.localScale = new Vector3(m_StageSize.x * 2, 1f, m_StageSize.y * 2);
		objRight.transform.position = new Vector3(m_StageSize.x * 2, 0, 0);
		
		GameObject objUp = GameObject.CreatePrimitive(PrimitiveType.Cube);
		objUp.layer = LayerMask.NameToLayer("Wall");
		objUp.AddComponent<Rigidbody>().useGravity = false;
		objUp.GetComponent<Rigidbody>().isKinematic = true;
//		Destroy(objUp.collider);
		objUp.transform.localScale = new Vector3(m_StageSize.x * 6, 1f, m_StageSize.y * 6);
		objUp.transform.position = new Vector3(0, 0, m_StageSize.y * 4);
		
		GameObject objDown = GameObject.CreatePrimitive(PrimitiveType.Cube);
		objDown.layer = LayerMask.NameToLayer("Wall");
		objDown.AddComponent<Rigidbody>().useGravity = false;
		objDown.GetComponent<Rigidbody>().isKinematic = true;
//		Destroy(objDown.collider);
		objDown.transform.localScale = new Vector3(m_StageSize.x * 6, 1f, m_StageSize.y * 6);
		objDown.transform.position = new Vector3(0, 0, -m_StageSize.y * 4);

        //GameObject bgParticleObj = Instantiate(Resources.Load("Background/BG_Particle")) as GameObject;
        //m_BG_Particle = bgParticleObj.GetComponent<ParticleSystem>();

		//m_ForeGround = Instantiate(Resources.Load("Background/ForeGround")) as GameObject;
		//m_ForeGround.transform.localScale = new Vector3(m_StageSize.x * 6f, m_StageSize.x * 6f, m_StageSize.y * 6);
		//m_ForeGround.transform.position = new Vector3(0.5f, m_ForeGroundDepth, 0.5f);

		//m_BackGround = Instantiate(Resources.Load("Background/BackGround")) as GameObject;
		//m_BackGround.transform.localScale = new Vector3(m_StageSize.x * 6f, m_StageSize.x * 6f, m_StageSize.y * 6);
		//m_BackGround.transform.position = new Vector3(0f, m_BackGroundDepth, 0f);
	}
	
	void Update ()
	{
	}
	#endregion
	#region - private -
	float m_MaxIntensity = 0;
	float m_Intensity = 0;
	float m_Speed = 1f;
	void CheckPlayerLevel()
	{
		Player player = YEntityManager.Instance.PlayerEntity;	
		m_MaxIntensity = player.Exp / Player.s_MaxExp;
		m_Speed = player.Exp * player.Exp * 0.0012f;
	}
	
	IEnumerator ChangingGridColor()
	{
		while(true)
		{
			Color color1 = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
			Color color2 = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
			Color tempColor1 = color1;
			Color tempColor2 = color2;

			while(true)
			{
				yield return null;

				m_Intensity += Time.deltaTime * m_Speed;
				float intensity = m_Intensity;// * m_Intensity;

				if(intensity > m_MaxIntensity)
					break;

				tempColor1 = color1 * intensity; tempColor1.a = 1f;
				//m_ForeGround.GetComponent<Renderer>().material.color = tempColor1;

				tempColor2 = color2 * intensity; tempColor2.a = 1f;
				//m_BackGround.GetComponent<Renderer>().material.color = tempColor2;
			}

			while(true)
			{
				yield return null;
				
				m_Intensity -= Time.deltaTime * m_Speed;
				float intensity = m_Intensity;// * m_Intensity;
				
				if(intensity < 0f)
					break;

				tempColor1 = color1 * intensity; tempColor1.a = 1f;
				//m_ForeGround.GetComponent<Renderer>().material.color = tempColor1;
				
				tempColor2 = color2 * intensity; tempColor2.a = 1f;
				//m_BackGround.GetComponent<Renderer>().material.color = tempColor2;
			}
		}
	}

    //	Color m_CurEdgeColor; public Color CurEdgeColor{get{return m_CurEdgeColor;}}
    //	IEnumerator ChangingEdgeColor()
    //	{
    //		float curSpeed = 0f;
    //
    //		while(true)
    //		{
    //			Color color1 = Color.black;
    //			Color color2 = Color.white;
    //			Color tempColor = color1;
    //			
    //			while(true)
    //			{
    //				yield return null;
    //				
    //				m_Intensity += Time.deltaTime * m_Speed;
    //				float intensity = m_Intensity;// * m_Intensity;
    //				
    //				if(intensity > m_MaxIntensity)
    //					break;
    //				
    //				tempColor = color1 * intensity; tempColor1.a = 1f;
    //				m_ForeGround.GetComponent<Renderer>().material.color = tempColor1;
    //			}
    //			
    //			while(true)
    //			{
    //				yield return null;
    //				
    //				m_Intensity -= Time.deltaTime * m_Speed;
    //				float intensity = m_Intensity;// * m_Intensity;
    //				
    //				if(intensity < 0f)
    //					break;
    //				
    //				tempColor1 = color1 * intensity; tempColor1.a = 1f;
    //				m_ForeGround.GetComponent<Renderer>().material.color = tempColor1;
    //				
    //				tempColor2 = color2 * intensity; tempColor2.a = 1f;
    //				m_BackGround.GetComponent<Renderer>().material.color = tempColor2;
    //			}
    //		}
    //	}

    IEnumerator ChangingBG_Particle_CR()
    {
        while (m_BG_Particle == null)
        {
            yield return null;
        }

        float initEmissionRate = m_BG_Particle.emissionRate;

        while (true)
        {
            yield return new WaitForSeconds(m_BG_ParticleInfo.refreshRate);

            m_BG_Particle.startLifetime = m_BG_ParticleInfo.maxLifeTime - Mathf.Pow(m_MaxIntensity, 0.5f) * m_BG_ParticleInfo.maxLifeTime + 0.01f;
            m_BG_Particle.emissionRate = initEmissionRate + m_MaxIntensity * m_BG_ParticleInfo.addEmissionRate;
        }
    }
    #endregion
    #region - public -
    public Vector3 GetRandomPlane2DPosInStage_ExceptPlayerPos()
	{
		float range = 10f;
		
		while(true)
		{
			Vector3 pos = new Vector3(Random.Range(-m_StageSize.x, m_StageSize.x), m_GroundHeight,
			Random.Range(-m_StageSize.y, m_StageSize.y));
		
			float dist = Vector3.SqrMagnitude(pos - YEntityManager.Instance.PlayerEntity.transform.position);
			
			if(dist > range)
				return pos;
		}
	}
	
	public Vector3 GetRandomPlane2DPosInStage_ExceptPlayerPos(float _range)
	{
		while(true)
		{
			Vector3 pos = new Vector3(Random.Range(-m_StageSize.x, m_StageSize.x), m_GroundHeight,
			Random.Range(-m_StageSize.y, m_StageSize.y));
		
			float dist = Vector3.SqrMagnitude(pos - YEntityManager.Instance.PlayerEntity.transform.position);
			
			if(dist > _range)
				return pos;
		}
	}
	
	public Vector3 GetRandomPosOuterPlane(float _range)
	{
		int seed = Random.Range(0, 4);
		
		Vector3 pos = new Vector3(0, m_GroundHeight, 0);
		
		switch(seed)
		{
		case 0:
			pos.x = Random.Range(-m_StageSize.x, m_StageSize.x);
			pos.z = m_StageSize.y;
			break;
		case 1:
			pos.x = Random.Range(-m_StageSize.x, m_StageSize.x);
			pos.z = -m_StageSize.y;
			break;
		case 2:
			pos.x = m_StageSize.x;
			pos.z = Random.Range(-m_StageSize.y, m_StageSize.y);
			break;
		case 3:
			pos.x = -m_StageSize.x;
			pos.z = Random.Range(-m_StageSize.y, m_StageSize.y);
			break;
		}
	
		return pos;
	}
	
	public bool CheckPosInPlane(Vector3 _pos)
	{
		if(-m_StageSize.x < _pos.x && _pos.x < m_StageSize.x &&
			-m_StageSize.y < _pos.z && _pos.z < m_StageSize.y)
			return true;
		else
			return false;
	}
	
	public Vector3 GetRandomPlane2DPosInStage()
	{
		return new Vector3(Random.Range(-m_StageSize.x, m_StageSize.x), m_GroundHeight,
			Random.Range(-m_StageSize.y, m_StageSize.y));
	}
	
	public Vector3 GetRandomRange2DPosInStage(Vector3 _pos, float _range)
	{
		float x = _pos.x;
		float z = _pos.z;
		
		if(x < -m_StageSize.x + _range)
			x = -m_StageSize.x + _range;
		if(m_StageSize.x - _range < x)
			x = m_StageSize.x - _range;
		if(z < -m_StageSize.y + _range)
			z = -m_StageSize.y + _range;
		if(m_StageSize.y - _range < z)
			z = m_StageSize.y - _range;
		 
		return new Vector3(x + Random.Range(-_range, _range), m_GroundHeight, z + Random.Range(-_range, _range));
	}
	
	public float GetStageExtent()
	{
		return m_StageSize.x * m_StageSize.y;
	}
	#endregion
}

[System.Serializable]
public class BG_ParticleInfo
{
    public float refreshRate = 0.1f;
    public float maxLifeTime = 1f;
    public float addEmissionRate = 4800f;
}