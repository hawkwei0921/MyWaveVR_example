using UnityEngine;

using System;
using UnityEngine;
using UnityEngine.UI;
using Wave.Essence;
using Wave.Native;
using Hubble.Launcher.Input;

#if VUPLEX
using HTC.UnityPlugin.CommonEventVariable;
using Vuplex.WebView;
using Vuplex.WebView;
#endif

public class WifiWebviewHandler : MonoBehaviour
{
    private const string LOG_TAG = "WifiWebviewHandler";
#if VUPLEX
    [SerializeField] private CanvasWebViewPrefab webView;
    [SerializeField] private CommonVariableAssetString urlString;

    private bool isWebInited = false;
    protected IMEManager mIMEManager;
    protected IMEManager.IMEParameter IMEPara;
#if CUSTOM_KEYBOARD
    private bool isKeyboardOpen = false;
    private bool isToKeyboardOpen = false;
#endif
    private bool isPageLoading = false;

    private void Awake()
    {
        CommonVariable.Get<string>("SettingsUI_WiFiWebviewURLString").SetValue("https://sso.htc.com/sso/login");
    }

    private void OnEnable()
    {
        Log.d(LOG_TAG, "OnEnable isWebInited ++   isWebInited="+ isWebInited);
        if (isWebInited)
        {
            if (!string.IsNullOrEmpty(urlString.Handler.CurrentValue))
            {
                Debug.LogWarning($"[WifiWebviewHandler]OnEnable() load url = {urlString.Handler.CurrentValue}");
                webView.WebView.LoadUrl(urlString.Handler.CurrentValue);
            }
        }
        else
        {
            Debug.LogWarning("[WifiWebviewHandler]OnEnable() call webview init");
            webView.Initialized += WebViewPrefab_Initialized;
            // webView.Init();  // new Vuplex deprecated Init() API
        }
        Log.d(LOG_TAG, "OnEnable isWebInited --");
    }
    protected void Start()
    {
        Debug.LogWarning("[WifiWebviewHandler] Start()++");
        mIMEManager = IMEManager.instance;
#if CUSTOM_KEYBOARD
      SetIMEPara();
#endif
        Debug.LogWarning("[WifiWebviewHandler] Start()--");
    }
    private void WebViewPrefab_Initialized(object sender, System.EventArgs e)
    {
        Debug.LogWarning("[WifiWebviewHandler] WebViewPrefab_Initialized");
        webView.WebView.LoadProgressChanged += WebView_LoadProgressChanged;
        webView.WebView.MessageEmitted += webview_MessageEmitted;
        webView.WebView.FocusedInputFieldChanged += WebView_FocusedInputFieldChanged;
        webView.WebView.ConsoleMessageLogged += WebView_ConsoleMessageLogged;
        webView.WebView.TitleChanged += WebView_TitleChanged;
        webView.WebView.UrlChanged += WebView_UrlChanged;
        // webView.WebView.PageLoadFailed += (sender, eventArgs) => {
        //      Debug.Log("[WifiWebviewHandler] Page load failed");
        // };

        if (!string.IsNullOrEmpty(urlString.Handler.CurrentValue))
        {
            Debug.Log($"[WifiWebviewHandler] load url = {urlString.Handler.CurrentValue}");
            webView.WebView.LoadUrl(urlString.Handler.CurrentValue);
        }
        webView.Initialized -= WebViewPrefab_Initialized;
        isWebInited = true;
    }

    void WebViewPrefab_UrlChanged(object sender, UrlChangedEventArgs eventArgs)
    {
        Log.i(LOG_TAG, "[WifiWebviewHandler] UrlChanged " + eventArgs.Url);
    }

    protected void WebView_LoadProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        Log.d(LOG_TAG, "WebView_LoadProgressChanged: e=" + e);
        if (e.Type == ProgressChangeType.Started)
        {
            Debug.LogWarning("[WifiWebviewHandler] webview_MessageEmitted isPageLoading Started");
            isPageLoading = true;
        }
        if (e.Type == ProgressChangeType.Finished)
        {
            Debug.LogWarning("[WifiWebviewHandler] webview_MessageEmitted isPageLoading Finished");
            isPageLoading = false;
        }
    }
    protected void webview_MessageEmitted(object sender, EventArgs<string> eventArgs)
    {
        Log.d(LOG_TAG, "webview_MessageEmitted: eventArgs=" + eventArgs);
        var serializedMessage = eventArgs.Value;
        var messageType = BridgeMessage.ParseType(serializedMessage);
        Debug.LogWarning("[WifiWebviewHandler] webview_MessageEmitted messageType=" + messageType + "  isPageLoading=" + isPageLoading);
        if (isPageLoading) return;
        switch (messageType)
        {
            case "ACTIVE_ELEMENT_TYPE_CHANGED":
#if CUSTOM_KEYBOARD
                KeyBoard_Open("");
                isToKeyboardOpen = true;
#endif
                break;
        }
    }
    protected void WebView_UrlChanged(object sender, UrlChangedEventArgs e)
    {
        Log.d(LOG_TAG, "UrlChangedEventArgs: e=" + e);
    }
    protected void WebView_TitleChanged(object sender, EventArgs<string> eventArgs)
    {
        Log.d(LOG_TAG, "WebView_TitleChanged: eventArgs=" + eventArgs);
    }
    protected void WebView_ConsoleMessageLogged(object sender, ConsoleMessageEventArgs e)
    {
        Log.d(LOG_TAG, "WebView_ConsoleMessageLogged: e=" + e);
    }
    protected void WebView_FocusedInputFieldChanged(object sender, FocusedInputFieldChangedEventArgs eventArgs)
    {
        Log.d(LOG_TAG, "WebView_FocusedInputFieldChanged: eventArgs=" + eventArgs + "    eventArgs.Type=" + eventArgs.Type + "   isPageLoading="+ isPageLoading);
        if (isPageLoading) return;
        if (eventArgs.Type == FocusedInputFieldType.Text)
        {
            int status = IMEManagerServer.s_IMEManagerServer.getKeyboardStatus();
            Log.d(LOG_TAG, "WebView_FocusedInputFieldChanged: show keyboard    status=" + status);
            IMEManagerServer.s_IMEManagerServer.showKeyboardOnWebview(webView);
        }
        else
        {
            Log.d(LOG_TAG, "WebView_FocusedInputFieldChanged: Not focus");
        }

    }

#if CUSTOM_KEYBOARD
    protected void SetIMEPara()
    {
        Debug.LogWarning("[WifiWebviewHandler] SetIMEPara()");
        Vector3 posFromHead = new Vector3(0.0f, -0.4f, -1.5f);
        Vector3 rotFromOrigin = new Vector3(12.0f, 0.0f, 0.0f);

        int id = 0;
        int type = 0x02;
        int mode = 2;

        string exist = "";
        int cursor = 0;
        int selectStart = 0;
        int selectEnd = 0;
        double[] pos = new double[] { posFromHead.x, posFromHead.y, posFromHead.z };
        Quaternion q = Quaternion.Euler(rotFromOrigin);
        double[] rot = new double[] { q.w, -1 * q.x, q.y, q.z };

        int width = 800;
        int height = 800;
        int shadow = 100;
        string locale = "";
        string title = "";
        int extraInt = 0;
        string extraString = "";
        int buttonId = 1<<(int)WVR_InputId.WVR_InputId_Alias1_Trigger;
        IMEPara = new IMEManager.IMEParameter(id, type, mode, exist, cursor, selectStart, selectEnd, pos,
            rot, width, height, shadow, locale, title, extraInt, extraString, buttonId);
    }

    void FixedUpdate()
    {
        if(isToKeyboardOpen){
            KeyBoard_Open("");
            isToKeyboardOpen = false;
            Log.i(LOG_TAG, "FixedUpdate  isToKeyboardOpen=" + isToKeyboardOpen);
        }
    }
    protected void KeyBoard_Open(string lastWord)
    {
        Debug.LogWarning("[WifiWebviewHandler] KeyBoard_Open() "+ isKeyboardOpen);
        if(isKeyboardOpen) return;
        //current_input_string = lastWord;
        IMEPara.exist = lastWord;
        mIMEManager.showKeyboard(IMEPara, true, InputDoneCallback, OnKeyPressed);
        isKeyboardOpen = true;
        Log.i(LOG_TAG, "KeyBoard_Open  isKeyboardOpen=" + isKeyboardOpen);
    }

    protected void KeyBoard_Close()
    {
        Debug.LogWarning("[WifiWebviewHandler] KeyBoard_Close");
        mIMEManager.hideKeyboard();
        isKeyboardOpen = false;
        Log.i(LOG_TAG, "KeyBoard_Close  isKeyboardOpen=" + isKeyboardOpen);
    }
    protected void InputDoneCallback(IMEManager.InputResult results)
    {
        string inputString = results.InputContent;
        Debug.LogWarning("[WifiWebviewHandler] InputDoneCallback() " + inputString);
        webView.WebView.HandleKeyboardInput(inputString);
        isKeyboardOpen = false;
        Log.i(LOG_TAG, "InputDoneCallback  isKeyboardOpen=" + isKeyboardOpen);
    }
    protected void OnKeyPressed(IMEManager.InputResult results)
    {
        string inputString = results.InputContent;
        Debug.LogWarning("[WifiWebviewHandler] OnKeyPressed() " + inputString);
    }
#endif
#endif
}