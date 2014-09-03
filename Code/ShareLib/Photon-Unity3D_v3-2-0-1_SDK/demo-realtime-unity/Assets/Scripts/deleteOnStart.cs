using UnityEngine;
using System.Collections;

public class deleteOnStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Component.DestroyObject(this.gameObject);
	}
}
