using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClickInvoker : MonoBehaviour {

	public Button button;

	void Update(){
		if(Input.GetKeyDown("joystick button 9")){
			button.onClick.Invoke ();
		}
	}
}
