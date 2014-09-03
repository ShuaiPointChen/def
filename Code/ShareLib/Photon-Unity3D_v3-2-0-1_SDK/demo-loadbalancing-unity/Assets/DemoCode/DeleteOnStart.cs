using UnityEngine;
using System.Collections;

public class DeleteOnStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Component.DestroyObject(this.gameObject);
	}
}
