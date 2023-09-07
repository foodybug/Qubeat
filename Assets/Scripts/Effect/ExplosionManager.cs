using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void voidDlt();

public class ExplosionManager : MonoBehaviour {

	#region - singleton -
	static ExplosionManager instance;
	public static ExplosionManager Instance
	{get{return instance;}}
	#endregion

	List<ExplosionRoot> m_listExplosion = new List<ExplosionRoot>();

	[SerializeField] float m_ExplosionSpeed = 3f; public float ExplosionSpeed{get{return m_ExplosionSpeed;}}
	[SerializeField] float m_AssembleBegin = 0.5f; public float AssembleBegin{get{return m_AssembleBegin;}}
	[SerializeField] float m_AssembleSpeed = 10f; public float AssembleSpeed{get{return m_AssembleSpeed;}}
	[SerializeField] float m_AssembleWait = 0.5f; public float AssembleWait{get{return m_AssembleWait;}}
	[SerializeField] float m_AssembleKeep = 0.5f; public float AssembleKeep{get{return m_AssembleKeep;}}
	[SerializeField] float m_AssembleKeepRand = 0.2f; public float AssembleKeepRand{get{return m_AssembleKeepRand;}}
	float m_sqrAssembleSpeed = 10f; public float sqrAssembleSpeed{get{return m_sqrAssembleSpeed;}}

	void Awake()
	{
		if(instance != null)
		{
			Destroy(gameObject);
			return;
		}

		#region - singleton -
		instance = this;
		#endregion

		DontDestroyOnLoad(gameObject);

		m_sqrAssembleSpeed = m_AssembleSpeed * m_AssembleSpeed;

		m_listExplosion.AddRange(GetComponentsInChildren<ExplosionRoot>());
	}

	// Use this for initialization
	void Start () {

	}

	void OnEnable()
	{
		foreach(ExplosionRoot node in m_listExplosion)
		{
			node.gameObject.SetActive(false);
		}
	}

	void OnDisable()
	{
//		foreach(ExplosionRoot node in m_listExplosion)
//		{
//			node.gameObject.SetActive(false);
//		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetExplosion(Vector3 _start, Vector3 _target, Color _color, float _size)
	{
		GameObject start = new GameObject("start pos:" + _start);
		start.transform.position = _start;

		GameObject target = new GameObject("target pos:" + _target);
		target.transform.position = _target;

		GameObject obj = Instantiate(Resources.Load("Effect/CubeExplosion")) as GameObject;
		ExplosionRoot root = obj.GetComponent<ExplosionRoot>();
		root.Init(start.transform, target.transform, _color, _size);
		Destroy(obj, 5f);

//		SetExplosion(start.transform, target.transform, _color, _size);

		Destroy(start, 5f);
		Destroy(target, 5f);
	}
	public void SetExplosion(Transform _start, Transform _target, Color _color, float _size)
	{
		GameObject obj = Instantiate(Resources.Load("Effect/CubeExplosion")) as GameObject;
		ExplosionRoot root = obj.GetComponent<ExplosionRoot>();
		root.Init(_start, _target, _color, _size);
		Destroy(obj, 5f);

//		foreach(ExplosionRoot node in m_listExplosion)
//		{
//			if(node.gameObject.activeInHierarchy == false)
//			{
//				node.Init(_start, _target, _color, _size);
//				break;
//			}
//		}
	}

	public void SetBossExplosion(Vector3 _start, Vector3 _target, Color _color, float _size)
	{
		GameObject start = new GameObject("start pos:" + _start);
		start.transform.position = _start;
		
		GameObject target = new GameObject("target pos:" + _target);
		target.transform.position = _target;

		GameObject obj = Instantiate(Resources.Load("Effect/CubeExplosion")) as GameObject;
		ExplosionRoot root = obj.GetComponent<ExplosionRoot>();
		root.Init(start.transform, target.transform, _color, _size, eScatterType.Scatter);
		Destroy(obj, 5f);

//		SetBossExplosion(start.transform, target.transform, _color, _size);
		
		Destroy(start, 5f);
		Destroy(target, 5f);
	}
	public void SetBossExplosion(Transform _start, Transform _target, Color _color, float _size)
	{
		foreach(ExplosionRoot node in m_listExplosion)
		{
			if(node.gameObject.activeSelf == false)
			{
				node.Init(_start, _target, _color, _size, eScatterType.Scatter);
				break;
			}
		}
	}
}
