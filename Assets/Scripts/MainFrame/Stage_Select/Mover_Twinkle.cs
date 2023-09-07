using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class Mover_Twinkle : MonoBehaviour {
	
	[SerializeField] float m_BoundarySize = 0f;
	[SerializeField] float m_MoveSpeedRandomRange = 0.5f;
	[SerializeField] float m_AngleSpeedRandomRange = 45f;
	
	Vector3 m_StartingPos;
//	Vector3 m_Boundary;
	
	Vector3 m_CurAngleSpeed;
//	Vector3 m_CurAngleAccel;
	Vector3 m_CurMoveSpeed;
	Vector3 m_CurMoveAccel;
	
	void Start () {
		
//		Mover_Twinkle_Init();
		
		InvokeRepeating("ChangeAngleSpeed", 0f, 5f);
	}
	
	void ChangeAngleSpeed()
	{
		m_CurAngleSpeed = new Vector3(
			Random.Range(-m_AngleSpeedRandomRange, m_AngleSpeedRandomRange),
			Random.Range(-m_AngleSpeedRandomRange, m_AngleSpeedRandomRange),
			Random.Range(-m_AngleSpeedRandomRange, m_AngleSpeedRandomRange));
	}
	
	void Update () {
		
//		Vector3 prevAccel = m_CurMoveAccel;
		transform.position += m_CurMoveSpeed * Time.deltaTime;
		transform.Rotate(m_CurAngleSpeed * Time.deltaTime);
		
		if(m_StartingPos.x + m_BoundarySize < transform.position.x)
			m_CurMoveAccel.x = -Mathf.Abs(m_CurMoveAccel.x);
		if(m_StartingPos.y + m_BoundarySize < transform.position.y)
			m_CurMoveAccel.y = -Mathf.Abs(m_CurMoveAccel.y);
		if(m_StartingPos.z + m_BoundarySize < transform.position.z)
			m_CurMoveAccel.z = -Mathf.Abs(m_CurMoveAccel.z);
		
		if(m_StartingPos.x - m_BoundarySize > transform.position.x)
			m_CurMoveAccel.x = Mathf.Abs(m_CurMoveAccel.x);
		if(m_StartingPos.y - m_BoundarySize > transform.position.y)
			m_CurMoveAccel.y = Mathf.Abs(m_CurMoveAccel.y);
		if(m_StartingPos.z - m_BoundarySize > transform.position.z)
			m_CurMoveAccel.z = Mathf.Abs(m_CurMoveAccel.z);
		
		m_CurMoveSpeed += m_CurMoveAccel * Time.deltaTime;
	}
	
//	void OnDrawGizmos()
//	{
////		Gizmos.DrawWireCube(m_StartingPos, new Vector3(m_BoundarySize, m_BoundarySize, m_BoundarySize));
//		Gizmos.DrawWireSphere(m_StartingPos, 0.1f);
//	}
	
	void Init_Mover_Twinkle()
	{
		m_CurMoveSpeed = new Vector3(
			Random.Range(-m_MoveSpeedRandomRange, m_MoveSpeedRandomRange),
			Random.Range(-m_MoveSpeedRandomRange, m_MoveSpeedRandomRange),
			Random.Range(-m_MoveSpeedRandomRange, m_MoveSpeedRandomRange));
		m_CurMoveSpeed *= 0.2f;
		
		m_StartingPos = transform.position;
//		m_Boundary = new Vector3(
//			m_StartingPos.x + m_BoundarySize * 0.5f,
//			m_StartingPos.y + m_BoundarySize * 0.5f,
//			m_StartingPos.z + m_BoundarySize * 0.5f);
		
		transform.position += new Vector3(
			Random.Range(-(m_BoundarySize * 0.5f), (m_BoundarySize * 0.5f)),
			Random.Range(-(m_BoundarySize * 0.5f), (m_BoundarySize * 0.5f)),
			Random.Range(-(m_BoundarySize * 0.5f), (m_BoundarySize * 0.5f)));
		
		m_CurMoveAccel = new Vector3(
			Random.Range(-m_MoveSpeedRandomRange, m_MoveSpeedRandomRange),
			Random.Range(-m_MoveSpeedRandomRange, m_MoveSpeedRandomRange),
			Random.Range(-m_MoveSpeedRandomRange, m_MoveSpeedRandomRange));
	}
	
	void ChangeAngleSpeed_Mover_Twinkle(float _speed)
	{
		m_AngleSpeedRandomRange = _speed;
		
		m_CurAngleSpeed = new Vector3(
			Random.Range(-_speed, _speed),
			Random.Range(-_speed, _speed),
			Random.Range(-_speed, _speed));
	}
	
	void OnDisable()
	{
		transform.position = m_StartingPos;
	}
}
