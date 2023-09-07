using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionMaintainer : MonoBehaviour
{
    [SerializeField] Transform targetTrn;

    Vector3 initRelativePos;

	private void Awake()
	{
		initRelativePos = targetTrn.position - transform.position;
	}

	void Start()
    {
        //initRelativePos = targetTrn.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = targetTrn.position - initRelativePos;
    }
}
