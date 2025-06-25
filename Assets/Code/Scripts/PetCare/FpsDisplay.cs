using UnityEngine;

public class FpsDisplay : MonoBehaviour
{
    float _deltaTime = 0.0f;

    void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();
        
        Rect rect = new Rect(200, 200, w, h * 2 / 100);
        style.alignment = TextAnchor.LowerLeft;
        style.fontSize = h * 5 / 100;
        style.normal.textColor = Color.red;

        float msec = _deltaTime * 1000.0f;
        float fps = 1.0f / _deltaTime;
        string text = $"{msec:0.0} ms ({fps:0.} fps)";

        GUI.Label(rect, text, style);
    }
}
