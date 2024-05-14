#region Using
using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
#endregion

/************************************************************************
 *	Copyright © 2013 Matthew Hoyes. All rights reserved					*
 *																		*
 *	No part of this content may be used for any purpose other than		*
 *	personal use. Therefore, reproduction or modification, and resale	*
 *	is strictly prohibited without prior written permission.			*
 ************************************************************************/

public static class ColoredDebugHelper
{
	/// <summary>
	/// Adds a color tag around the string with a specific HEX code.
	/// </summary>
	/// <param name="str">The string the function is an extension of.</param>
	/// <param name="colorInHex">The HEX code which was converted from a color.</param>
	/// <returns>Returns the new string with the color tag surrounding it</returns>
	public static string AddColorTag(this string str, string colorInHex)
	{
		return "<color=" + colorInHex + ">" + str + "</color>";
	}

	/// <summary>
	/// Adds a color tag around the string with a specific HEX code.
	/// Uses the Unity Color and converts it into HEX code.
	/// </summary>
	/// <param name="str">The string the function is an extension of.</param>
	/// <param name="color">The Unity Color</param>
	/// <returns>Returns the new string with the color tag surrounding it.</returns>
	public static string AddColorTag(this string str, Color color)
	{
        string colorInHex = ColoredDebugHelper.ColorToHex(color);
        return str.AddColorTag(colorInHex);
	}

    /// <summary>
    /// Adds a color tag around the key phraase within the message with a specific HEX code.
    /// Uses the Unity Color and converts it into HEX code.
    /// </summary>
    /// <param name="str">The string the function is an extension of.</param>
    /// <param name="keyPhrase">The key phrase to stylize within the message.</param>
    /// <param name="colorInHex">The HEX code which was converted from a color.</param>
    /// <param name="bold">Make the key phrase bold?</param>
    /// <param name="italic">Make the key phrase italic?</param>
    /// <returns>Returns the new string with the color tag surrounding it.</returns>
    public static string AddColorToKeyPhrase(this string str, string keyPhrase, string colorInHex, bool bold, bool italic)
    {
        if (keyPhrase == string.Empty || !str.ToLower().Contains(keyPhrase.ToLower())) { return str; }

        string message = keyPhrase.AddColorTag(colorInHex);

        if (bold) { message = message.AddTag("b"); }
        if (italic) { message = message.AddTag("i"); }

        message = str.Replace(keyPhrase, message);
        return message;
    }

    /// <summary>
    /// Adds a color tag around the key phraase within the message with a specific HEX code.
    /// Uses the Unity Color and converts it into HEX code.
    /// </summary>
    /// <param name="str">The string the function is an extension of.</param>
    /// <param name="keyPhrase">The key phrase to stylize within the message.</param>
    /// <param name="color">The Unity Color</param>
    /// <param name="bold">Make the key phrase bold?</param>
    /// <param name="italic">Make the key phrase italic?</param>
    /// <returns>Returns the new string with the color tag surrounding it.</returns>
    public static string AddColorToKeyPhrase(this string str, string keyPhrase, Color color, bool bold, bool italic)
    {
        string colorInHex = ColoredDebugHelper.ColorToHex(color);

        return str.AddColorToKeyPhrase(keyPhrase, colorInHex, bold, italic);
    }

	/// <summary>
	/// Adds a tag around the the string
	/// </summary>
	/// <param name="str">The string the function is an extension of.</param>
	/// <param name="tag">The tag (I.E: b = Bold, i = Italics).</param>
	/// <returns>Returns the new string with the tag surrounding it.</returns>
    public static string AddTag(this string str, string tag)
    {
        return "<" + tag + ">" + str + "</" + tag + ">";
    }

	/// <summary>
	/// Converts the Color to HEX code by converting the RBA values.
	/// </summary>
	/// <param name="color">The color to convert to HEX code.</param>
	/// <returns>The color in HEX code</returns>
	public static string ColorToHex(Color32 color)
	{
		return "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
	}

	/// <summary>
	/// Checks whether a string is fully empty or not.
	/// </summary>
	/// <param name="str">The string the function is an extension of.</param>
	/// <returns>Returns whether a string has all spaces or not</returns>
	public static bool IsCompletelyEmpty(this string str)
	{
		foreach(char character in str)
		{
			if (character != ' ') { return false; }
		}
		return true;
	}

    public static string UTF8ByteArrayToString(byte[] characters)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        string constructedString = encoding.GetString(characters);

        return (constructedString);
    }

    public static byte[] StringToUTF8ByteArray(string pXmlString)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] byteArray = encoding.GetBytes(pXmlString);

        return byteArray;
    }

    // Here we serialize our object of type T
    public static string SerializeObject<T>(object pObject) where T : class
    {
        string XmlizedString = null;

        MemoryStream memoryStream = new MemoryStream();

        XmlSerializer xs = new XmlSerializer(typeof(T));

        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

        xs.Serialize(xmlTextWriter, pObject);

        memoryStream = (MemoryStream)xmlTextWriter.BaseStream;

        XmlizedString = ColoredDebugHelper.UTF8ByteArrayToString(memoryStream.ToArray());

        return XmlizedString;
    }

    // Here we deserialize it back into its original form 
    public static object DeserializeObject<T>(string pXmlizedString) where T : class
    {
        XmlSerializer xs = new XmlSerializer(typeof(T));

        MemoryStream memoryStream = new MemoryStream(ColoredDebugHelper.StringToUTF8ByteArray(pXmlizedString));

        return xs.Deserialize(memoryStream);
    } 
}