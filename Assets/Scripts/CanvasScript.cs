#if !UNITY_EDITOR && UNITY_IOS
using System.Runtime.InteropServices;
#endif
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script to handle canvas UI elements and communication with iOS plugin.
/// </summary>
public class CanvasScript : MonoBehaviour
{
    public Text MessageText = null;
    public Text HelloWorldText = null;
    public Text AddText = null;
    public Text ConcatText = null;

    private float _update;
    private bool _called = false;

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void cSendHelloWorldMessage();

    [DllImport("__Internal")]
    private static extern string cHelloWorld();

    [DllImport("__Internal")]
    private static extern int cAdd(int x, int y);

    [DllImport("__Internal")]
    private static extern string cConcatenate(string x, string y);
#endif

    /// <summary>
    /// Initializes the text values on Start.
    /// </summary>
    private void Start()
    {
        InitializeTextValues();
    }

    /// <summary>
    /// Updates the timer and calls the iOS plugin function "cSendHelloWorldMessage" after a 5s.
    /// </summary>
    private void Update()
    {
        _update += Time.deltaTime;
        if (_update > 5.0f && !_called)
        {
            _update = 0.0f;
            _called = true;

#if UNITY_IOS && !UNITY_EDITOR
            cSendHelloWorldMessage();
#endif
        }
    }

    /// <summary>
    /// Called when a message is received from the iOS plugin by calling the "swiftSendHelloWorldMessage" method.
    /// </summary>
    /// <param name="message">The received message.</param>
    private void OnMessageReceived(string message)
    {
        MessageText.text = message;
    }

    /// <summary>
    /// Initializes the text values form the Canvas.
    /// </summary>
    private void InitializeTextValues()
    {
        if (MessageText == null || HelloWorldText == null || AddText == null || ConcatText == null)
        {
            Debug.LogError("One or more Text elements are not assigned in the inspector.");
            return;
        }

#if UNITY_IOS && !UNITY_EDITOR
        MessageText.text = "Waiting for message from iOS...";
        HelloWorldText.text = cHelloWorld();
        AddText.text = cAdd(1, 2).ToString();
        ConcatText.text = cConcatenate("Hello, ", "World!");
#else
        MessageText.text = "Only works on iOS.";
        HelloWorldText.text = "Only works on iOS.";
        AddText.text = "Only works on iOS.";
        ConcatText.text = "Only works on iOS.";
#endif
    }
}
