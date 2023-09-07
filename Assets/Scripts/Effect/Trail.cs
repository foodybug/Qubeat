using UnityEngine;
using System.Collections;

public class Trail : MonoBehaviour {

	[SerializeField] float m_Ratio = 1f;

	Transform m_Target;

	public void Init(Transform _trn)
	{
		m_Target = _trn;

		transform.localScale = _trn.localScale * 0.8f;

		Transform parent = transform.parent;
		transform.parent = null;

		if(parent.childCount == 0)
			Destroy(parent.gameObject);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if(m_Target != null)
		{
			transform.position = Vector3.Lerp(transform.position, m_Target.position, m_Ratio * Time.deltaTime);
			transform.rotation = m_Target.rotation;
		}
	}
}
