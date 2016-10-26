using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class InputManager {

	public static bool PickUpPutDownPressed() {
		List<Func<bool>> buttons = new List<Func<bool>>();
		buttons.Add( () => Input.GetKeyDown(KeyCode.Space) );
		buttons.Add( () => Input.GetButtonDown("Jump") );
		switch( Application.platform ) {
			case RuntimePlatform.OSXPlayer:
				buttons.Add( () => Input.GetButtonDown("joystick button 16") );
				break;
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.LinuxPlayer:
				buttons.Add( () => Input.GetButtonDown("joystick button 0") );
				break;
			default:
				break;
		}
		return buttons.Any( f => f() == true );
	}

	public static bool ResetPressed() {
		List<Func<bool>> buttons = new List<Func<bool>>();
		buttons.Add( () => Input.GetKeyDown(KeyCode.R) );
		buttons.Add( () => Input.GetButtonDown("Cancel") );
		switch( Application.platform ) {
			case RuntimePlatform.OSXPlayer:
				buttons.Add( () => Input.GetButtonDown("joystick button 10") );
				break;
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.LinuxPlayer:
				buttons.Add( () => Input.GetButtonDown("joystick button 6") );
				break;
			default:
				break;
		}
		return buttons.Any( f => f() == true );
	}

	public static bool ChangeSelectionPressed() {
		List<Func<bool>> buttons = new List<Func<bool>>();
		buttons.Add( () => Input.GetKeyDown(KeyCode.E) );
		buttons.Add( () => Input.GetButtonDown("Fire1") );
		switch( Application.platform ) {
			case RuntimePlatform.OSXPlayer:
				buttons.Add( () => Input.GetButtonDown("joystick button 18") );
				break;
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.LinuxPlayer:
				buttons.Add( () => Input.GetButtonDown("joystick button 2") );
				break;
			default:
				break;
		}
		return buttons.Any( f => f() == true );
	}

	public static bool StartGamePressed() {
		List<Func<bool>> buttons = new List<Func<bool>>();
		buttons.Add( () => Input.GetKeyDown(KeyCode.Return) );
		buttons.Add( () => Input.GetButtonDown("Submit") );
		switch( Application.platform ) {
			case RuntimePlatform.OSXPlayer:
				buttons.Add( () => Input.GetButtonDown("joystick button 9") );
				break;
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.LinuxPlayer:
				buttons.Add( () => Input.GetButtonDown("joystick button 7") );
				break;
			default:
				break;
		}
		return buttons.Any( f => f() == true );
	}
}
