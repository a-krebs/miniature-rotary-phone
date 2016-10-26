using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClickInvoker : MonoBehaviour {

	public Button button;

	void Update(){
		if(InputManager.StartGamePressed()){
			button.onClick.Invoke ();
		}
	}
}
