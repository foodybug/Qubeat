using UnityEngine;
using System.Collections;

public class Ending : MonoBehaviour {
	
	[SerializeField] float waitTime_InputEnable_ = 2f;
	bool m_ChangeEnable = false;
	
	void Awake()
	{
	}
	
	IEnumerator Start()
	{
		yield return new WaitForSeconds(0.5f);
		
		InvokeRepeating("EffectProcess", 1f, 2f);
		InvokeRepeating("LaserProcess", 3f, 2.1f);
		
		yield return new WaitForSeconds(waitTime_InputEnable_);
		
		m_ChangeEnable = true;
	}
	
	void EffectProcess()
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0));
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit) == true)
		{
//			GameObject obj = Instantiate(Resources.Load("Effect/Explode_Ending")) as GameObject;
//			obj.transform.position = hit.point;
//			obj.transform.Rotate(Vector3.right, 90);
//			obj.GetComponent<ParticleSystem>().startColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
//			obj.GetComponent<ParticleSystem>().loop = false;
//			
//			Destroy(obj, 2f);

			if(ExplosionManager.Instance != null)
			{
				ExplosionManager.Instance.SetExplosion(hit.point,
				                                       Random.insideUnitSphere * 9f,
				                                       new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)),
				                                       Random.Range(1f, 9f));
				ExplosionManager.Instance.SetBossExplosion(hit.point,
				                                           Random.insideUnitSphere * 9f,
				                                           new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)),
				                                           Random.Range(1f, 9f));
			}
		}
	}
	
	void LaserProcess()
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0));
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit) == true)
		{
			GameObject laser = GameObject.CreatePrimitive(PrimitiveType.Cube);
			laser.AddComponent<Laser>();
			Destroy(laser.GetComponent<Collider>());
			laser.transform.position = hit.point;
//			laser.transform.Rotate(Vector3.forward, Random.Range(0f, 360f));

			Destroy(laser, 2f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnMouseUpAsButton()
	{
		if(m_ChangeEnable == true)
//			Application.LoadLevel("Stage_Select");
			MainFlow.Instance.SceneConversion("Developer_Mark");
	}
}
