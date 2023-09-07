using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {
	
	void Awake()
	{
	}
	
	float curScale;
	float speed;
	IEnumerator Start () {
		
//		if(YEntityManager.Instance.PlayerEntity != null)
//		{
			transform.localScale = new Vector3(0, 0, 0);
			
			if(YEntityManager.Instance != null)
				transform.LookAt(YEntityManager.Instance.PlayerEntity.transform);
			else
			{
				transform.eulerAngles = new Vector3(90, 0, 0);
				transform.Rotate(Vector3.up, Random.Range(0f, 360f));
			}
			
			yield return new WaitForSeconds(0.4f);
			
			curScale = 0.5f;
			speed = 0.4f;
			
			while(true)
			{
				yield return null;
				
				transform.localScale = new Vector3(curScale, curScale, 99999f);
				
				curScale -= Time.deltaTime * speed;
				
				if(curScale < 0)
					break;
			}
//		}
		
		Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
