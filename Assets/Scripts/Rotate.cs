using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

    public float rotateSpeed = 10.0f;
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.right * Time.deltaTime * rotateSpeed);
        transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed, Space.World);	
	}
}
