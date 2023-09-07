using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplosionRoot : MonoBehaviour {

	int m_ActivatedParticleCount = 0;

	List<ExplosionParticle> m_listParticle = new List<ExplosionParticle>();

	void Awake()
	{
		m_listParticle.AddRange(GetComponentsInChildren<ExplosionParticle>());
	}

	void OnEnable()
	{
	}

	void OnDisable()
	{
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Init(Transform _start, Transform _target, Color _color, float _size, eScatterType _type = eScatterType.Assemble)
	{
		gameObject.SetActive(true);

		m_ActivatedParticleCount = m_listParticle.Count;

		transform.position = _start.position;

		foreach(ExplosionParticle node in m_listParticle)
		{
			node.transform.localPosition = Vector3.zero;
			node.Init(_target, Dlt_ParticleEnd, _color, _size, _type);
		}
	}

	void Dlt_ParticleEnd()
	{
		--m_ActivatedParticleCount;
        //Debug.Log("m_ActivatedParticleCount = " + m_ActivatedParticleCount);

		if(m_ActivatedParticleCount <= 0)
		{
			gameObject.SetActive(false);
		}
	}
}
