using UnityEngine;
using System.Collections;

public class CurtainMover : MonoBehaviour {

	[SerializeField] ParticleSystem m_Particle;

	// Use this for initialization
	void Start () {
		
//		SimplePad.DestroyThis();
		
		m_Particle.GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
//		SendMessage("SM_Color", renderer.material.color);
		
		foreach(Transform trn in transform)
		{
			trn.SendMessage("SM_Color", GetComponent<Renderer>().material.color);
		}
		
		StartCoroutine(SceneConversionProcess());
		
		Invoke("CloseBanner", 0.5f);
	}
	
	void CloseBanner()
	{
		AdvertisementManager.Instance.Close_Banner();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDestroy()
	{
		MainFlow.Instance.SceneChangingFinished();
	}
	
	IEnumerator SceneConversionProcess()
	{
		transform.localPosition = new Vector3(-7, 0, 1);
		
		Vector3 dir = new Vector3(1, 0, 0);
		float speed = 6;
		
		while(true)
		{
			yield return null;
			
			transform.localPosition += dir * speed * Time.deltaTime;
			
			if(transform.localPosition.x > 0)
				break;
		}
		
		MainFlow.Instance.CurtainCoveredFull();
//		m_Particle.Play();

		while(true)
		{
			yield return null;
			
			transform.localPosition += dir * speed * Time.deltaTime;
			
			if(transform.localPosition.x > 7)
				break;
		}
		
		SendMessageUpwards("SM_Destroy");
//		Destroy(gameObject);
	}
	
	void SM_Color(Color _color)
	{
		GetComponent<Renderer>().material.color = _color;
	}
}
