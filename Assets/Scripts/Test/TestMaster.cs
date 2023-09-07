using UnityEngine;
using System.Collections;

public class TestMaster : MonoBehaviour {
	
	[SerializeField] CollisionTest testObject_;
	[SerializeField] CollisionTest_Player testPlayer_;

	// Use this for initialization
	void Start () {
		for(int i=0; i<200; ++i)
		{
			Instantiate(testObject_.gameObject, new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f)), Quaternion.identity);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0) == true)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 1 << LayerMask.NameToLayer("Ground")) == true)
			{
				testPlayer_.SetDestination(hit.point);
			}
		}
	}
	
	void OnMouseDown()
	{
		
	}
}
