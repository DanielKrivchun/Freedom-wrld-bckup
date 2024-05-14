using UnityEngine;
using System.Collections;
using UnityEditor;

/************************************************************************
 *	Copyright © 2013 Matthew Hoyes. All rights reserved					*
 *																		*
 *	No part of this content may be used for any purpose other than		*
 *	personal use. Therefore, reproduction or modification, and resale	*
 *	is strictly prohibited without prior written permission.			*
 ************************************************************************/


public class ColorWindow : EditorWindow
{
	public Color color;
	public string colorInHex;
	public string colorName = "Enter Color Name";
    public Vector2 scrollPos;
	public string errorMsg = "";
	public bool showError = false;
	public float timer = 1.0f;

	private static GUIStyle style = new GUIStyle();

	void Update()
	{
		colorInHex = ColoredDebugHelper.ColorToHex(color);

		if (showError)
		{
			if (timer > 0.0f) { timer -= .02f; }
			else { ResetError(); }
		}
	}

	void OnGUI()
	{
		if (ColoredDebug.ColorsList.Count > 0)
		{
			style.alignment = TextAnchor.MiddleCenter;
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 12;
			EditorGUILayout.LabelField("Colors: " + ColoredDebug.ColorsList.Count, style);
		}

		color = EditorGUILayout.ColorField("Color: " + colorInHex , color);

		colorName = EditorGUILayout.TextField("Color Name (Max: 25):", colorName);

		// Cap the name to a 25 character limit in order to not mess up layout
		if (colorName.Length > 25) { colorName = colorName.Substring(0, 25); }

		EditorGUILayout.LabelField(errorMsg, style);

		style.alignment = TextAnchor.MiddleLeft;
		style.fontStyle = FontStyle.Normal;
		style.fontSize = 10;

		// Draw a seperation line
		EditorGUILayout.LabelField("__________________________________________________________________________");

		// Begin a scroll view
		// Everything inside here scrolls if the contents exceeds to lower bounds
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		
		// Set up a delete color in case one is set to delete
        ColorObj colorToDelete = null;

		int count = 0;
        foreach (ColorObj col in ColoredDebug.ColorsList)
        {
			count++;
            EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(col.name, style);
			EditorGUILayout.LabelField(col.colorInHex, style);
            EditorGUILayout.ColorField(col.color);
            EditorGUILayout.Separator();

			// The current color being drawn is not one of
			// the auto-generated primary colors
			if (count > 7)
			{
				if (GUILayout.Button("X")) { colorToDelete = col; }
			}
			else
			{
				// Needed to push the "Primary Colors" to the left to align
				// with other colors which have X buttons next to them
				EditorGUILayout.Separator();
				EditorGUILayout.Separator();
				EditorGUILayout.Separator();
				EditorGUILayout.Separator();
				EditorGUILayout.Separator();
			}
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
        }
        EditorGUILayout.EndScrollView();

		// If there is a color to delete, delete it
        if (colorToDelete != null) { ColoredDebug.ColorsList.Remove(colorToDelete); }

		// Space it out a little before the Save/Apply Buttons draw
        for(int i = 0; i < 6; i++) 
            EditorGUILayout.Separator();

		// Save the colors, and close the window
		if (GUI.Button(new Rect((Screen.width / 2) - 90, Screen.height - 25, 60, 25), "Save")) { SaveClose(); }

		// Apply/save the colors to the list
        if (GUI.Button(new Rect((Screen.width / 2)- 30, Screen.height - 25, 60, 25), "Apply")) { Save(); }

        // Apply/save the colors to the list
        if (GUI.Button(new Rect((Screen.width / 2)  + 40, Screen.height - 25, 60, 25), "Cancel")) { Close(); }
	}

	[MenuItem("Window/Colored Debug")]
	public static void AddColor()
	{
		ColorWindow window = (ColorWindow)EditorWindow.GetWindow(typeof(ColorWindow), true, "View Colors");
		window.minSize = window.maxSize = new Vector2(530f, 350f);

		style.alignment = TextAnchor.UpperCenter;
		style.normal.textColor = Color.black;
		style.fontSize = 10;

		ColoredDebug.Load();
	}

	private void Save()
	{
        // Has the user modified the name?
            if (colorName != "Enter Color Name" && !colorName.IsCompletelyEmpty())
            {
				// Create a color structure
                ColorObj clr = new ColorObj(colorName, colorInHex, color);

				// Search through the list and try to find if any attribute matches
                bool found = false;
                foreach (ColorObj col in ColoredDebug.ColorsList)
                {
					// If the 
					if (col.name.ToLower() == clr.name.ToLower() || col.colorInHex == clr.colorInHex || col.color == clr.color)
                    {
                        found = true;
						ShowError("Unable to add color. The name or color already exists."); 
                        break;
                    }
                }

				// Add the color if it hasn't been found
                if (!found)
				{
					// Set the scroll position to it's max (very bottom) in order to show the last added color
					scrollPos.y = 10000000.0f;
                    ColoredDebug.ColorsList.Add(clr);
					ColoredDebug.Save();
					
					colorName = "Enter Color Name";
					colorInHex = ColoredDebugHelper.ColorToHex(Color.black);
                }
            }
			else { ShowError("You must add a name and color!"); ColoredDebug.Save(); }
	}

	private void SaveClose()
	{
		Save(); Close();
	}

	private void ShowError(string msg)
	{
		errorMsg = msg;
		showError = true;
	}
	
	private void ResetError()
	{
		timer = 1.0f;
		showError = false;
		errorMsg = "";
	}
}
