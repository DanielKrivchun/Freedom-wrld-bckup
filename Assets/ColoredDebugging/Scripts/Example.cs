using UnityEngine;
using System.Collections;

public class Example : MonoBehaviour {
	
	//public ColoredDebug.Colors logColor;
	//public ColoredDebug.Colors logErrorColor;
	//public ColoredDebug.Colors logWarningColor;
	
	public string logColor;
	public string logErrorColor;
	public string logWarningColor;
	
	// Use this for initialization
	void Start () {

        #region Log Examples
        ColoredDebug.Log("Message only");

        ColoredDebug.Log("Message, Style", FontStyle.Bold);

        ColoredDebug.Log("Message, string color", logColor);

        ColoredDebug.Log("Message, string color, style", logColor, FontStyle.BoldAndItalic);

        ColoredDebug.Log("Message, Unity color", Color.blue);

        ColoredDebug.Log("Message, Unity color, style", Color.blue, FontStyle.Bold);

        ColoredDebug.Log("Message, string keyPhrase, style", "keyPhrase", FontStyle.BoldAndItalic);

        ColoredDebug.Log("Message, string keyPhrase, string color", "keyPhrase", "rainbow");

        ColoredDebug.Log("Message, string keyPhrase, Unity color", "keyPhrase", Color.blue);

        ColoredDebug.Log("Message, string keyPhrase, string color, style", "keyPhrase", "rainbow", FontStyle.Italic);

        ColoredDebug.Log("Message, string keyPhrase, Unity color, style", "keyPhrase", Color.blue, FontStyle.Italic); 
        #endregion

        Debug.Log("----------------------------------------------------------------------------------------\n" +
                  "----------------------------------------------------------------------------------------");

        #region LogError Examples
        ColoredDebug.LogError("Message only");

        ColoredDebug.LogError("Message, Style", FontStyle.Bold);

        ColoredDebug.LogError("Message, string color", logErrorColor);

        ColoredDebug.LogError("Message, string color, style", logErrorColor, FontStyle.BoldAndItalic);

        ColoredDebug.LogError("Message, Unity color", Color.red);

        ColoredDebug.LogError("Message, Unity color, style", Color.red, FontStyle.Bold);

        ColoredDebug.LogError("Message, string keyPhrase, style", "keyPhrase", FontStyle.BoldAndItalic);

        ColoredDebug.LogError("Message, string keyPhrase, string color", "keyPhrase", "rainbow");

        ColoredDebug.LogError("Message, string keyPhrase, Unity color", "keyPhrase", Color.red);

        ColoredDebug.LogError("Message, string keyPhrase, string color, style", "keyPhrase", "rainbow", FontStyle.Italic);

        ColoredDebug.LogError("Message, string keyPhrase, Unity color, style", "keyPhrase", Color.red, FontStyle.Italic);
        #endregion

        Debug.Log("----------------------------------------------------------------------------------------\n" +
                  "----------------------------------------------------------------------------------------");

        #region LogWarning Examples
        ColoredDebug.LogWarning("Message only");

        ColoredDebug.LogWarning("Message, Style", FontStyle.Bold);

        ColoredDebug.LogWarning("Message, string color", logWarningColor);

        ColoredDebug.LogWarning("Message, string color, style", logWarningColor, FontStyle.BoldAndItalic);

        ColoredDebug.LogWarning("Message, Unity color", Color.yellow);

        ColoredDebug.LogWarning("Message, Unity color, style", Color.yellow, FontStyle.Bold);

        ColoredDebug.LogWarning("Message, string keyPhrase, style", "keyPhrase", FontStyle.BoldAndItalic);

        ColoredDebug.LogWarning("Message, string keyPhrase, string color", "keyPhrase", "rainbow");

        ColoredDebug.LogWarning("Message, string keyPhrase, Unity color", "keyPhrase", Color.yellow);

        ColoredDebug.LogWarning("Message, string keyPhrase, string color, style", "keyPhrase", "rainbow", FontStyle.Italic);

        ColoredDebug.LogWarning("Message, string keyPhrase, Unity color, style", "keyPhrase", Color.yellow, FontStyle.Italic);
        #endregion

        Debug.Log("----------------------------------------------------------------------------------------\n" +
                  "----------------------------------------------------------------------------------------");
        
        // Examples of Logs that have invalid color inputs
        ColoredDebug.Log("This Log has an invalid color", "unknownColor");
        ColoredDebug.LogError("This LogError has an invalid color", "unknownColor2");
        ColoredDebug.LogWarning("This LogWarning has an invalid color", "unknownColor3"); 
	}
	
}
