using UnityEngine;
using System.Collections;
using System.ComponentModel;
using Unity.Collections;

public enum eSpinType {Left, Right}

public class YSpinner : YBaseComponent
{
	Vector3 m_Angle;
	[SerializeField] float m_SpinSpeed = 0f;
	float m_SpinRevision = 1f;
	bool m_Spinning = true;
	bool m_Player = false;
	
	Transform m_Transform;
	
	void Awake()
	{
		m_Transform = transform;
		
		RegisterReceiver(typeof(Msg_Spin_Start), OnSpinStart);
		RegisterReceiver(typeof(Msg_Spin_Stop), OnSpinStop);
		RegisterReceiver(typeof(Msg_Spin_SpeedModify), OnSpinSpeedModify);
		RegisterReceiver(typeof(Msg_Spin_AngleModify), OnSpinAngleModify);

		//m_Angle = new Vector3(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
		m_Angle = Random.onUnitSphere * 360f * Random.Range(0.8f, 1.2f);
	}
	
	public override void Init()
	{
		base.Init();
	}
	
	public void SetRevision(float _revision)
	{
		m_SpinRevision = _revision;
	}
	
	void Start()
	{
		YBaseEntity entity = GetComponent<YBaseEntity>();
		if(entity.GetType() == typeof(Player))
			m_Player = true;

		_SetSpinSpeedByPlayerExp();

		StartCoroutine(RefreshSpinSpeed());
	}
	
	IEnumerator RefreshSpinSpeed()
	{
		float refreshRate = 0.5f;
		
		while(true)
		{
			yield return new WaitForSeconds(refreshRate);

			if (m_Player == false)
				_SetSpinSpeedByPlayerExp();
		}
	}

	void _SetSpinSpeedByPlayerExp()
    {
		Player player = YEntityManager.Instance.PlayerEntity;
		if (player != null)
			m_SpinSpeed = (player.Exp * player.Exp * 0.01f * 0.01f) * 2f + 1f;
	}
	
	//Vector3 angle;
	void Update()
	{
		if(m_Spinning == true)
		{
			Vector3 angle = m_Angle * m_SpinSpeed * m_SpinRevision * Time.deltaTime;
			if (m_Player == true)
			{
				if (Time.timeScale == 0)
					angle = Vector3.zero;
				else
					angle /= Time.timeScale;
			}
			else
				angle *= m_SpinSpeed;

			m_Transform.Rotate(angle);
        }
	}
	
	#region - msg -
	void OnSpinStart(YMessage _msg)
	{
		StartCoroutine(RefreshSpinSpeed());
		m_Spinning = true;
	}
	
	void OnSpinStop(YMessage _msg)
	{
		StopAllCoroutines();
		m_Spinning = false;
	}
	
	void OnSpinSpeedModify(YMessage _msg)
	{
		Msg_Spin_SpeedModify modify = _msg as Msg_Spin_SpeedModify;
		
		m_SpinSpeed = modify.speed_;
	}
	
	void OnSpinAngleModify(YMessage _msg)
	{
		//m_Angle = new Vector3(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
		m_Angle = Random.onUnitSphere * 360f * Random.Range(0.5f, 1.5f);
	}
	#endregion
}

