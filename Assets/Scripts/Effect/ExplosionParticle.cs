using UnityEngine;
using System.Collections;

public enum eScatterType {Assemble, Scatter}

public class ExplosionParticle : MonoBehaviour {

	eScatterType m_ScatterType = eScatterType.Assemble;

	const float destDist = 0.2f * 0.2f;

	Transform m_Parent;
	Transform m_Target;

	Vector3 m_InitVelocity;
	Vector3 m_Velocity;
	Vector3 m_Rotation;

	voidDlt m_Dlt;

	enum eState{Scatter, Stop, Assemble, Assemble2}
	eState m_State = eState.Scatter;
	float m_AssembleTime = 0f;

	void Awake()
	{
		m_Parent = transform.parent;

		if(m_Parent == null)
			Debug.LogError("ExplosionParticle:: Awake: there is no parent");
	}

	// Use this for initialization
	void Start () {
	
	}
	
	float m_ElapsedTime = 0f;
    float m_ElapsedAssembleTime = 0.01f;
	void Update () {

		if(m_Target == null ||
		   (m_ScatterType == eScatterType.Scatter && m_ElapsedTime > 5f))
		{
			enabled = false;
			GetComponent<Renderer>().enabled = false;
			if(m_Dlt != null) m_Dlt();

            //Debug.Log("effect end by time over");
            return;
		}

		transform.Rotate(m_Rotation * Time.deltaTime);

		m_ElapsedTime += Time.deltaTime;

		switch(m_State)
		{
		case eState.Scatter:
			m_Velocity -= m_InitVelocity * Time.deltaTime * 2f;
			if(m_ElapsedTime > 0.5f && m_ScatterType == eScatterType.Assemble)
			{
				m_ElapsedTime = 0f;
				m_State = eState.Stop;
                    
                return;
			}
			break;
		case eState.Stop:
			m_Velocity = Vector3.zero;
			if(m_ElapsedTime > ExplosionManager.Instance.AssembleWait)
			{
				m_ElapsedTime = 0f;
				m_State = eState.Assemble;
                m_Velocity = Vector3.zero;
				return;
			}
			break;
		case eState.Assemble:
			Vector3 dist = m_Target.position - transform.position;
			if(dist.sqrMagnitude < destDist)
			{
				enabled = false;
				GetComponent<Renderer>().enabled = false;
				m_Dlt();
                m_Dlt = null;
                m_ElapsedAssembleTime = 0f;

                //Debug.Log("effect end properly");

                return;
			}
            
                m_Velocity += dist.normalized;
                if (m_Velocity.sqrMagnitude > ExplosionManager.Instance.sqrAssembleSpeed)
                    m_Velocity = m_Velocity.normalized * ExplosionManager.Instance.AssembleSpeed;
                
                if (m_ElapsedTime > m_AssembleTime)
                {
                    m_ElapsedTime = 0f;
                    m_State = eState.Assemble2;

                    //m_Velocity = Vector3.zero;
                    //Debug.Log("additional end process");
                    return;
                }

                break;
        case eState.Assemble2:
                //transform.position = Vector3.Lerp(transform.position, m_Target.position, 0.15f);

                dist = m_Target.position - transform.position;

                m_Velocity += dist.normalized;
                if (m_Velocity.sqrMagnitude > 99f)
                    m_Velocity *= 0.95f;

                if (dist.sqrMagnitude < destDist)
                {
                    m_ElapsedTime = 0f;
                    m_State = eState.Scatter;

                    enabled = false;
                    GetComponent<Renderer>().enabled = false;
                    m_Dlt();
                    m_Dlt = null;

                    //Debug.Log("additional process end");

                    return;
                }
                break;
        }

		switch(m_ScatterType)
		{
        case eScatterType.Assemble:
            transform.position += m_Velocity * Time.deltaTime;
            break;
		case eScatterType.Scatter:
			transform.Translate(m_Velocity * Time.deltaTime);
			break;
		}
	}

	public void Init(Transform _target, voidDlt _dlt, Color _color, float _size, eScatterType _type)
	{
		m_ScatterType = _type;
		m_ElapsedTime = 0f;
        m_ElapsedAssembleTime = 0.01f;
        m_State = eState.Scatter;

		m_Target = _target;
		m_Dlt = _dlt;

		enabled = true;
		GetComponent<Renderer>().enabled = true;

		m_InitVelocity = m_Velocity = Random.onUnitSphere * ExplosionManager.Instance.ExplosionSpeed;
		m_Rotation = Random.rotation.eulerAngles;

//		GetComponent<Renderer>().material.color = _color;
		GetComponent<Renderer>().sharedMaterial.color = _color;
		Vector3 size = 0.1f * (Vector3.one + Vector3.one * 0.4f * _size);
		size.z *= 0.2f;
		transform.localScale = size;

		m_AssembleTime = ExplosionManager.Instance.AssembleKeep +
			Random.Range(0, ExplosionManager.Instance.AssembleKeepRand);

//		StartCoroutine(Move_Scatter());
	}

	#region - move step -
	IEnumerator Move_Scatter()
	{
//		Debug.Log("Move_Scatter");

//		float elapsedTime = 0f;

		while(true)
		{
			yield return null;

			if(ValidTarget() == false)
				yield break;

			if(m_Velocity.sqrMagnitude < 1f)
			{
				StartCoroutine(Move_Stop());
				yield break;
			}

//			float mag = m_Velocity.sqrMagnitude > 3f ? 3f : m_Velocity.sqrMagnitude;
			m_Velocity -= m_InitVelocity * Time.deltaTime * 3f;// * mag;

			MoveByType();
		}
	}

	IEnumerator Move_Stop()
	{
//		Debug.Log("Move_Stop");

		yield return new WaitForSeconds(ExplosionManager.Instance.AssembleWait);

		StartCoroutine(Move_Assemble());
	}

	IEnumerator Move_Assemble()
	{
//		Debug.Log("Move_Assemble");

		float elapsedTime = 0f;

		while(true)
		{
			yield return null;

			if(ValidTarget() == false)
				yield break;

			elapsedTime += Time.deltaTime;
			if(elapsedTime >
			   ExplosionManager.Instance.AssembleKeep + Random.Range(0, ExplosionManager.Instance.AssembleKeepRand))
				break;

			Vector3 dist = m_Target.position - transform.position;
			if(dist.sqrMagnitude < destDist)
				break;

			m_Velocity += dist.normalized;
			if(m_Velocity.sqrMagnitude > ExplosionManager.Instance.sqrAssembleSpeed)
				m_Velocity = m_Velocity.normalized * ExplosionManager.Instance.AssembleSpeed;
			
			MoveByType();
		}

		enabled = false;
		GetComponent<Renderer>().enabled = false;
		m_Dlt();
	}
	#endregion
	#region - method -
	bool ValidTarget()
	{
		if(m_Target == null)
		{
			enabled = false;
			GetComponent<Renderer>().enabled = false;
			return false;
		}
		else
			return true;
	}

	void MoveByType()
	{
		switch(m_ScatterType)
		{
		case eScatterType.Assemble:
			transform.position += m_Velocity * Time.deltaTime;
			break;
		case eScatterType.Scatter:
			transform.Translate(m_Velocity * Time.deltaTime);
			break;
		}
	}
	#endregion
}
