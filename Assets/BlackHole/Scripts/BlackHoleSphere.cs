using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BlackHoleSphere : MonoBehaviour
{
	private Material mat;
	private Transform m_transform;

	void Start()
	{
		mat = GetComponent<MeshRenderer>().sharedMaterial;
		m_transform = transform;
	}

	void Update()
	{
		mat.SetVector("_Center", new Vector4(m_transform.position.x, m_transform.position.y, m_transform.position.z, 1f));
	}
}
