#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
#endregion

/************************************************************************
 *	Copyright © 2013 Matthew Hoyes. All rights reserved					*
 *																		*
 *	No part of this content may be used for any purpose other than		*
 *	personal use. Therefore, reproduction or modification, and resale	*
 *	is strictly prohibited without prior written permission.			*
 ************************************************************************/

public static class ColoredDebug 
{
	#region Variables
	private static List<ColorObj> colors;

	public static List<ColorObj> ColorsList
	{
		get
		{
			if (colors == null) { colors = new List<ColorObj>(); }
			return colors;
		}
		private set
		{
			if (colors == null) { colors = new List<ColorObj>(); }
			colors = value;
		}
	}

	private static string fileName;

	// Where we want to save and load to and from 
	private static string fileLocation;

	private static string data;

	//private static int enumSize = 0;
	private static string str = string.Empty;
	#endregion
	
	/// <summary>
	/// If it's not initialized, initialize the color file with primary colors
	/// </summary>
    private static void InitializeFile()
    {
        fileName = "\\ColorFile.xml";

        // Where we want to save and load to and from 
        fileLocation = Application.dataPath + fileName;
		
		FileInfo t = new FileInfo(fileLocation);

        if (!t.Exists)
		{
			#region Set up Default (Primary) colors
			if(ColorsList.Count < 7)
			{
				Color color = Color.black;
				string name = string.Empty;
				string hex = string.Empty;
				for(int i = 0; i < 7; i++)
				{
					switch(i)
					{
					case 0:
                        color = new Color(1.0f, 0f, 0f, 1.0f);		// Red
						name = "Red";
						break;

					case 1:
						color = new Color(1.0f, .65f, 0f, 1.0f);	// Orange
						name = "Orange";
						break;
					
					case 2:
                        color = new Color(1.0f, 1.0f, 0f, 1.0f);	// Yellow
						name = "Yellow";
						break;

					case 3:
                        color = new Color(0f, 1.0f, 0, 1.0f);		// Green
						name = "Green";
						break;

					case 4:
						color = new Color(0f, 0f, 1.0f, 1.0f);		// Blue
						name = "Blue";
						break;

					case 5:
						color = new Color(0.5f, 0f, 0.5f, 1.0f);	// Purple
						name = "Purple";
						break;
						
					case 6:
						color = new Color(1.0f, 0f, 1.0f, 1.0f);	// Magenta
						name = "Magenta";
						break;
					}
					
					hex = ColoredDebugHelper.ColorToHex(color);
					ColorsList.Add(new ColorObj(name, hex, color));
				}
				
				Save();
			}
			#endregion
		}
    }
	
    #region Debug Log/Error/Warning

    #region Logs
    /// <summary>
    /// Logs the whole message in a specific style.
    /// If style is FontStyle.Normal, it uses Debug.Log call and defaults.
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="style">The style (Bold, Italic, Etc)</param>
    public static void Log(string message, FontStyle style)
    {
        if (style == FontStyle.Normal) { Debug.Log(message); }
        else
        {
            CalcMessage(LogType.Log, message, string.Empty, "default", style);
        }
    }

    /// <summary>
    /// Logs the whole message in a color (if specified).
    /// Can be used to call:
    /// Log(message),
    /// Log(message, color),
    /// or Log(message, color, style)
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="color">(Optional) The color</param>
    /// <param name="style">(Optional) The style</param>
	public static void Log(string message, string color = "default", FontStyle style = FontStyle.Normal)
	{
		if(color == "default" && style == FontStyle.Normal) { Debug.Log(message); }
		else
		{
			Load();
			CalcMessage(LogType.Log, message, string.Empty, color, style);
		}
	}

    /// <summary>
    /// Logs the whole message in the specified color, and style (if specified).
    /// Can be used to call:
    /// Log(message, color)
    /// or Log(message, color, style)
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="color">The color</param>
    /// <param name="style">(Optional) The style</param>
    public static void Log(string message, Color color, FontStyle style = FontStyle.Normal)
    {
        if (color == Color.black && style == FontStyle.Normal) { Debug.Log(message); }
        else
        {
            Load();
            CalcMessage(LogType.Log, message, string.Empty, color, style);
        }
    }

    /// <summary>
    /// Logs the whole message, with a specific key phrase in color (if key phrase exists within the message). Also adds a style, if specified.
    /// Can be used to call:
    /// Log(message, keyPhrase, color)
    /// Log(message, keyPhrase, color, style)
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="keyPhrase">The key phrase</param>
    /// <param name="color">The color</param>
    /// <param name="style">(Optional) The style</param>
    public static void Log(string message, string keyPhrase, string color, FontStyle style = FontStyle.Normal)
    {
        if (!message.Contains(keyPhrase) || (color == "default" && style == FontStyle.Normal)) { Debug.Log(message); }
        else
        {
            Load();
            CalcMessage(LogType.Log, message, keyPhrase, color, style);
        }
    }

    /// <summary>
    /// Logs the whole message, with a specific key phrase in color (if key phrase exists within the message). Also adds a style, if specified.
    /// Can be used to call:
    /// Log(message, keyPhrase, color)
    /// Log(message, keyPhrase, color, style)
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="keyPhrase">The key phrase</param>
    /// <param name="color">The color</param>
    /// <param name="style">(Optional) The style</param>
    public static void Log(string message, string keyPhrase, Color color, FontStyle style = FontStyle.Normal)
    {
        if ( !message.Contains(keyPhrase) || color == Color.black) { Debug.Log(message); }
        else
        {
            Load();
            CalcMessage(LogType.Log, message, keyPhrase, color, style);
        }
    }

    #endregion Logs

    #region LogErrors

    /// <summary>
    /// Logs the whole message as an error in a specific style.
    /// If style is FontStyle.Normal, it uses Debug.Log call and defaults.
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="style">The style (Bold, Italic, Etc)</param>
    public static void LogError(string message, FontStyle style)
    {
        if (style == FontStyle.Normal) { Debug.LogError(message); }
        else
        {
            CalcMessage(LogType.Error, message, string.Empty, "default", style);
        }
    }

    /// <summary>
    /// Logs the whole message as an error in a specific color (if specified).
    /// Can be used to call:
    /// Log(message),
    /// Log(message, color),
    /// or Log(message, color, style)
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="color">(Optional) The color</param>
    /// <param name="style">(Optional) The style</param>
    public static void LogError(string message, string color = "default", FontStyle style = FontStyle.Normal)
    {
        if (color == "default" && style == FontStyle.Normal) { Debug.LogError(message); }
        else
        {
            Load();
            CalcMessage(LogType.Error, message, string.Empty, color, style);
        }
    }

    /// <summary>
    /// Logs the whole message as an error, in the specified color, and style (if specified).
    /// Can be used to call:
    /// Log(message, color)
    /// or Log(message, color, style)
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="color">The color</param>
    /// <param name="style">(Optional) The style</param>
    public static void LogError(string message, Color color, FontStyle style = FontStyle.Normal)
    {
        if (color == Color.black && style == FontStyle.Normal) { Debug.LogError(message); }
        else
        {
            Load();
            CalcMessage(LogType.Error, message, string.Empty, color, style);
        }
    }

    /// <summary>
    /// Logs the whole message as an error, with a specific key phrase in color (if key phrase exists within the message). Also adds a style, if specified.
    /// Can be used to call:
    /// Log(message, keyPhrase, color)
    /// Log(message, keyPhrase, color, style)
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="keyPhrase">The key phrase</param>
    /// <param name="color">The color</param>
    /// <param name="style">(Optional) The style</param>
    public static void LogError(string message, string keyPhrase, string color, FontStyle style = FontStyle.Normal)
    {
        if (!message.Contains(keyPhrase) || (color == "default" && style == FontStyle.Normal)) { Debug.LogError(message); }
        else
        {
            Load();
            CalcMessage(LogType.Error, message, keyPhrase, color, style);
        }
    }

    /// <summary>
    /// Logs the whole message as an error, with a specific key phrase in color (if key phrase exists within the message). Also adds a style, if specified.
    /// Can be used to call:
    /// Log(message, keyPhrase, color)
    /// Log(message, keyPhrase, color, style)
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="keyPhrase">The key phrase</param>
    /// <param name="color">The color</param>
    /// <param name="style">(Optional) The style</param>
    public static void LogError(string message, string keyPhrase, Color color, FontStyle style = FontStyle.Normal)
    {
        if (!message.Contains(keyPhrase) || color == Color.black) { Debug.LogError(message); }
        else
        {
            Load();
            CalcMessage(LogType.Error, message, keyPhrase, color, style);
        }
    }

    #endregion LogErrors

    #region LogWarnings

    /// <summary>
    /// Logs the whole message as a warning in a specific style.
    /// If style is FontStyle.Normal, it uses Debug.Log call and defaults.
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="style">The style (Bold, Italic, Etc)</param>
    public static void LogWarning(string message, FontStyle style)
    {
        if (style == FontStyle.Normal) { Debug.LogWarning(message); }
        else
        {
            CalcMessage(LogType.Warning, message, string.Empty, "default", style);
        }
    }

    /// <summary>
    /// Logs the whole message as a warning in a specific color (if specified).
    /// Can be used to call:
    /// Log(message),
    /// Log(message, color),
    /// or Log(message, color, style)
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="color">(Optional) The color</param>
    /// <param name="style">(Optional) The style</param>
    public static void LogWarning(string message, string color = "default", FontStyle style = FontStyle.Normal)
    {
        if (color == "default" && style == FontStyle.Normal) { Debug.LogWarning(message); }
        else
        {
            Load();
            CalcMessage(LogType.Warning, message, string.Empty, color, style);
        }
    }

    /// <summary>
    /// Logs the whole message as a warning, in the specified color, and style (if specified).
    /// Can be used to call:
    /// Log(message, color)
    /// or Log(message, color, style)
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="color">The color</param>
    /// <param name="style">(Optional) The style</param>
    public static void LogWarning(string message, Color color, FontStyle style = FontStyle.Normal)
    {
        if (color == Color.black && style == FontStyle.Normal) { Debug.LogWarning(message); }
        else
        {
            Load();
            CalcMessage(LogType.Warning, message, string.Empty, color, style);
        }
    }

    /// <summary>
    /// Logs the whole message as a warning, with a specific key phrase in color (if key phrase exists within the message). Also adds a style, if specified.
    /// Can be used to call:
    /// Log(message, keyPhrase, color)
    /// Log(message, keyPhrase, color, style)
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="keyPhrase">The key phrase</param>
    /// <param name="color">The color</param>
    /// <param name="style">(Optional) The style</param>
    public static void LogWarning(string message, string keyPhrase, string color, FontStyle style = FontStyle.Normal)
    {
        if (!message.Contains(keyPhrase) || (color == "default" && style == FontStyle.Normal)) { Debug.LogWarning(message); }
        else
        {
            Load();
            CalcMessage(LogType.Warning, message, keyPhrase, color, style);
        }
    }

    /// <summary>
    /// Logs the whole message as a warning, with a specific key phrase in color (if key phrase exists within the message). Also adds a style, if specified.
    /// Can be used to call:
    /// Log(message, keyPhrase, color)
    /// Log(message, keyPhrase, color, style)
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="keyPhrase">The key phrase</param>
    /// <param name="color">The color</param>
    /// <param name="style">(Optional) The style</param>
    public static void LogWarning(string message, string keyPhrase, Color color, FontStyle style = FontStyle.Normal)
    {
        if (!message.Contains(keyPhrase) || color == Color.black) { Debug.LogWarning(message); }
        else
        {
            Load();
            CalcMessage(LogType.Warning, message, keyPhrase, color, style);
        }
    }

    #endregion LogWarnings

    #endregion Debug Log/Error/Warning
	
	#region Calculate Message
    
	/// <summary>
	/// Calculates the message by converting the message into xml format with color codes
	/// </summary>
	/// <param name='logType'>
	/// The type of Log which will be used to display the message
	/// </param>
	/// <param name='message'>
	/// The message being displayed in the Log
    /// </param>
    /// <param name='keyPhrase'>
    /// The key phrase within the message to stylize
    /// </param>
	/// <param name='color'>
	/// The color in which the message is drawn in
	/// </param>
	/// <param name='style'>
	/// Should the message be in Bold or Italic?
	/// </param>
    private static void CalcMessage(LogType logType, string message, string keyPhrase = "", string color = "default", FontStyle style = FontStyle.Normal)
	{
		// Needs to be reset so it's not constantly appended to over and over again.
		str = string.Empty;
        bool colorActingAsKeyPhrase = false;
		
		if(color.ToLower() == "rainbow")
		{
			SetupRainbow(logType, message, keyPhrase, style); return;
		}
		else if(color.ToLower() == "default" && style == FontStyle.Normal)
		{
			if(logType == LogType.Log) { Debug.Log(message); }
			else if(logType == LogType.Error) { Debug.LogError(message); }
			else { Debug.LogWarning(message); }
			return;	
		}
		else
		{
			bool found = false;

            if (color != "default")
            {
                foreach (ColorObj clr in ColorsList)
                {
                    if (clr.name.ToLower() == color.ToLower())
                    {
                        // No key phrase, do the whole string
                        if (keyPhrase == string.Empty) { message = message.AddColorTag(clr.colorInHex); }
                        else
                        {
                            if (style == FontStyle.Normal) { message = message.AddColorToKeyPhrase(keyPhrase, color, false, false); }
                            else if (style == FontStyle.Bold) { message = message.AddColorToKeyPhrase(keyPhrase, color, true, false); }
                            else if (style == FontStyle.Italic) { message = message.AddColorToKeyPhrase(keyPhrase, color, false, true); }
                            else if (style == FontStyle.BoldAndItalic) { message = message.AddColorToKeyPhrase(keyPhrase, color, true, true); }
                        }
                        found = true;
                        break;
                    }
                }
            }

			if(!found)
			{
                // If it's not a color that is found, check to make sure the color parameter isn't acting as a keyPhrase instead
                if (!message.Contains(color))
                {
                    // Color parameter is still acting as a color
                    if (color != "default")
                    {
                        Debug.LogWarning("Color: \"" + color + "\" not assigned or found. Defaulting!\n" +
                                         "------------------------------------------------------");
                        if (logType == LogType.Log) { Debug.Log(message); }
                        else if (logType == LogType.Error) { Debug.LogError(message); }
                        else { Debug.LogWarning(message); }
                        return;
                    }
                }
                else
                {
                    colorActingAsKeyPhrase = true;
                    if (style == FontStyle.Normal) { message = message.AddColorToKeyPhrase(color, Color.black, false, false); }
                    else if (style == FontStyle.Bold) { message = message.AddColorToKeyPhrase(color, Color.black, true, false); }
                    else if (style == FontStyle.Italic) { message = message.AddColorToKeyPhrase(color, Color.black, false, true); }
                    else if (style == FontStyle.BoldAndItalic) { message = message.AddColorToKeyPhrase(color, Color.black, true, true); }
                }
			}
		}

        if (colorActingAsKeyPhrase == false && ( keyPhrase == string.Empty || style != FontStyle.Normal))
		{
	        if (style == FontStyle.Bold || style == FontStyle.BoldAndItalic) { message = message.AddTag("b"); }
	        if (style == FontStyle.Italic || style == FontStyle.BoldAndItalic) { message = message.AddTag("i"); }
		}
		str = message;
		
		if(logType == LogType.Log) { Debug.Log(str); }
		else if(logType == LogType.Error) { Debug.LogError(str); }
		else { Debug.LogWarning(str); }
	}

	/// <summary>
	/// Calculates the message by converting the message into xml format with color codes
	/// (Note: Using this call does not allow Rainbow display)
	/// </summary>
	/// <param name='logType'>
	/// The type of Log which will be used to display the message
	/// </param>
	/// <param name='message'>
	/// The message being displayed in the Log
	/// </param>
	/// <param name='color'>
	/// The color in which the message is drawn in
    /// </param>
    /// <param name='keyPhrase'>
    /// The key phrase within the message to stylize
    /// </param>
	/// <param name='style'>
	/// Should the message be in Bold or Italic?
	/// </param>
    private static void CalcMessage(LogType logType, string message, string keyPhrase = "", Color? color = null, FontStyle style = FontStyle.Normal)
	{
        // Needs to be reset so it's not constantly appended to over and over again.
		str = string.Empty;
		if(color != null)
		{
	        if (color != Color.clear)
	        {
	            // No key phrase, do the whole string
	            if (keyPhrase == string.Empty) { message = message.AddColorTag(color?? Color.black); }
	            else
	            {
	                if (style == FontStyle.Normal) { message = message.AddColorToKeyPhrase(keyPhrase, color ?? Color.black, false, false); }
	                else if (style == FontStyle.Bold) { message = message.AddColorToKeyPhrase(keyPhrase, color ?? Color.black, true, false); }
	                else if (style == FontStyle.Italic) { message = message.AddColorToKeyPhrase(keyPhrase, color ?? Color.black, false, true); }
	                else if (style == FontStyle.BoldAndItalic) { message = message.AddColorToKeyPhrase(keyPhrase, color ?? Color.black, true, true); }
	            }
			}
	        else { Debug.LogWarning("Sorry, Using Color.clear has to default to the Default color"); return; }
	
	        if (keyPhrase == string.Empty || style != FontStyle.Normal)
	        {
	            if (style == FontStyle.Bold || style == FontStyle.BoldAndItalic) { message = message.AddTag("b"); }
	            if (style == FontStyle.Italic || style == FontStyle.BoldAndItalic) { message = message.AddTag("i"); }
	        }
		}
		str = message;

		if (logType == LogType.Log) { Debug.Log(str); }
		else if (logType == LogType.Error) { Debug.LogError(str); }
		else { Debug.LogWarning(str); }
	}
	
	#endregion
	
	/// <summary>
	/// Sets up the rainbow displayed Log
	/// </summary>
	/// <param name='logType'>
	/// The type of Log which will be used to display the message
	/// </param>
	/// <param name='message'>
	/// The full message to be displayed
	/// </param>
    /// <param name='keyPhrase'>
    /// The key phrase within the message to stylize
    /// </param>
	/// <param name='style'>
	/// Should the message be in Bold or Italic?
	/// </param>
    private static void SetupRainbow(LogType logType, string message, string keyPhrase = "", FontStyle style = FontStyle.Normal)
    {
        // No key phrase, do the whole string
        if (keyPhrase == string.Empty)
        {
            int index = 1;
            foreach (char character in message)
            {
                string letter = character.ToString();

                str += letter.AddColorTag(GetRainbowColor(index));

                if (character != ' ') { index++; }

                if (index > 7) { index = 1; }
            }

            if (style == FontStyle.Bold || style == FontStyle.BoldAndItalic) { str = str.AddTag("b"); }
            if (style == FontStyle.Italic || style == FontStyle.BoldAndItalic) { str = str.AddTag("i"); }
        }
        else
        {
            str = message;

            if (str.ToLower().Contains(keyPhrase.ToLower()))
            {
                int count = keyPhrase.Length;
                string code = "";

                for(int i = 0; i < count; i++) { code += "~"; }
				
                str = str.Replace(keyPhrase, code);

                string newKeyPhrase = "";

                int index = 1;
                foreach (char character in keyPhrase)
                {
                    string letter = character.ToString();
	
					newKeyPhrase += letter.AddColorTag(GetRainbowColor(index));
				
                    if (character != ' ') { index++; }
                    if (index > 7) { index = 1; }
                }

                if (style == FontStyle.Bold || style == FontStyle.BoldAndItalic) { newKeyPhrase = newKeyPhrase.AddTag("b"); }
                if (style == FontStyle.Italic || style == FontStyle.BoldAndItalic) { newKeyPhrase = newKeyPhrase.AddTag("i"); }

                str = str.Replace(code, newKeyPhrase);
            }
        }

		if(logType == LogType.Log) { Debug.Log(str); }
		else if(logType == LogType.Error) { Debug.LogError(str); }
		else { Debug.LogWarning(str); }
	}
	
	#region Get Methods
	
	/// <summary>
	/// Retrieves the color code in HEX as a string
	/// </summary>
	/// <param name='_color'>The color</param>
	/// <returns>The color in HEX as a string</returns>
    private static string GetColor(Color _color)
    {
        return ColoredDebugHelper.ColorToHex(_color);
    }

	/// <summary>
	/// Converts the given index into a specific Color of the Rainbow
	/// </summary>
    /// <param name='index'>The index of the specific color</param>
    /// <returns>The rainbow color</returns>
	private static string GetRainbowColor(int index)
	{		
		switch(index)
		{
		case 1:	
			return GetColorHex("red");
			
		case 2:
			return GetColorHex("orange");
			
		case 3:	
			return GetColorHex("yellow");
			
		case 4:
			return GetColorHex("green");
			
		case 5:
			return GetColorHex("blue");
			
		case 6:
			return GetColorHex("purple");
			
		case 7:	
			return GetColorHex("magenta");
			
		default:
			return "red";
		}
	}
	
	/// <summary>
	/// Gets the colors HEX code from the given color name
	/// </summary>
	/// <param name='colorName'>The color name. If the color doesn't exist, it defaults to the first primary color (red)</param>
	/// <returns>Returns the HEX code of the given color</returns>
	private static string GetColorHex(string colorName)
	{
		foreach(ColorObj color in ColorsList)
		{
			if(color.name.ToLower() == colorName) { return color.colorInHex; }
		}
		return "red";
	}
	
	#endregion
	
	#region Load/Save Methods
	
	/// <summary>
	/// Load the colors
	/// </summary>
    public static void Load()
    {
		InitializeFile();
		
        // Load the data
        LoadXML();

        if (data != "")
        {
            colors = (List<ColorObj>)ColoredDebugHelper.DeserializeObject<List<ColorObj>>(data);
        }
    }
	
	/// <summary>
	/// Save the current status of the colors
	/// </summary>
    public static void Save()
    {
		// Time to creat our XML! 
        data = ColoredDebugHelper.SerializeObject<List<ColorObj>>(colors);
  
		InitializeFile();
        
        // This is the final resulting XML from the serialization process 
        CreateXML();
    }

	/// <summary>
	/// Load the XML from the color file
	/// </summary>
    private static void LoadXML()
    {
        FileInfo t = new FileInfo(fileLocation);

        if (t.Exists)
        {
            StreamReader r = File.OpenText(fileLocation);

            string info = r.ReadToEnd();
            r.Close();

            //Debug.Log("info: " + info);
            data = info;
        }
    }

	/// <summary>
	/// Create/Write the color data to a file
	/// </summary>
    private static void CreateXML()
    {
        StreamWriter writer;
        FileInfo t = new FileInfo(fileLocation);

        if (!t.Exists) { writer = t.CreateText(); }
        else
        {
            //t.Delete(); 
            writer = t.CreateText();
        }

        writer.Write(data);
        writer.Close();
    }

	#endregion
}
			
[System.Serializable]
public class ColorObj
{
    [XmlAttribute]
	public string name;
	public string colorInHex;
    public Color color;

    public ColorObj()
    {
        this.name = "Color Name";
        this.colorInHex = "hexCode";
        this.color = Color.black;
    }

	public ColorObj(string name, string hexCode, Color color)
	{
		this.name = name;
		this.colorInHex = hexCode;
        this.color = color;
	}
}
