using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Dont let the user type some characters
 * 
 * dNesvera 
 */

public class InputFieldChecker : MonoBehaviour {

	private InputField input_field;

	// Use this for initialization
	void Start () {
		// get the input field
		input_field = GetComponent<InputField> ();

		// Sets the MyValidate method to invoke after the input field's default input validation invoke (default validation happens every time a character is entered into the text field.)
		input_field.onValidateInput += delegate(string input, int charIndex, char addedChar){ return MyValidate(addedChar); };
	}

	// Check for characters that windows allows for save files
	private char MyValidate(char charToValidate){
		if (charToValidate == '\\' ||
			charToValidate == '/' ||
			charToValidate == ':' ||
			charToValidate == '*' ||
			charToValidate == '?' ||
			charToValidate == '"' ||
			charToValidate == '\'' ||
			charToValidate == '<' ||
			charToValidate == '>' ||
			charToValidate == '|'
		){
			// ... if it is change it to an empty character.
			charToValidate = '\0';
		}
		return charToValidate;
	}
}
