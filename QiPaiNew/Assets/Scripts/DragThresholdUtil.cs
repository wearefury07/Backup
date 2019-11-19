using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragThresholdUtil : MonoBehaviour {

	void Start () {
		int defaultValue = EventSystem.current.pixelDragThreshold;		
		EventSystem.current.pixelDragThreshold = 
				Mathf.Max(
					defaultValue , 
					(int) (defaultValue * Screen.dpi / 160f));

		Debug.Log ("DEFAULT VALUE:  " + defaultValue);
		Debug.Log ("SCREEN DPI:     " + Screen.dpi);
		Debug.Log ("SCREEN DPI 160: " + Screen.dpi/160f);
		Debug.Log ("DRAG THRESHOLD: " + EventSystem.current.pixelDragThreshold);
	}

}
