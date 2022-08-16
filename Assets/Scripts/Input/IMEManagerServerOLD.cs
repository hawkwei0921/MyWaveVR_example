// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Text;
using UnityEngine;
using Wave.Essence;
using Wave.Native;
using UnityEngine.UI;
#if VUPLEX
using Vuplex.WebView;
#endif
namespace Hubble.Launcher.Input
{
	public class IMEManagerServerOLD : MonoBehaviour
	{
		static private bool bDebug = true;
		static public IMEManagerServerOLD s_IMEManagerServer;
		private InputField m_InputField;
#if VUPLEX
		private CanvasWebViewPrefab m_Webview;
#endif
		private static string LOG_TAG = "IMEManagerServerOLD";
		private IMEManagerWrapper2 mIMEWrapper;
		private string mInputContent = null;
		private StringBuilder onInputClickedSB;
		private string focusGOParentGOname;

		private bool mIsShowKeyboardInputPanel = true;
		void Start()
		{
			s_IMEManagerServer = gameObject.GetComponent<IMEManagerServerOLD>();
			mIMEWrapper = IMEManagerWrapper2.GetInstance();
		}
		public void setCallbacks()
		{
			Log.i(LOG_TAG, "setCallbacks for WV");
			mIMEWrapper.SetDoneCallback(InputDoneCallbackImpl);
		}

		public void setCallbacks(GameObject GO)
		{
			IMEManagerClientOLD inputClient;
			inputClient = GetIMEClient(GO);

			if (inputClient != null)
			{
				if (bDebug) Log.d(LOG_TAG, "setCallbacks for TMP Input_Field");
				// IMECallbackIF inputClickCallback = (IMECallbackIF)m_InputFocusGameObject;
				mIMEWrapper.SetDoneCallback(InputDoneCallbackImpl);
				mIMEWrapper.SetClickedCallback(InputClickCallbackImpl);
			}
			else
			{
				Debug.LogError("inputClient == null ");
			}
		}

		private InputField GetInputField(GameObject GO)
		{
			if (bDebug) Log.d(LOG_TAG, "GetInputField() +++");
			if (GO != null)
			{
				InputField inputObj = GO.GetComponent<InputField>();

				return inputObj;
			}
			else
			{
				Debug.LogError("GO == null");
				throw new System.NullReferenceException();
			}
			if (bDebug) Log.d(LOG_TAG, "GetInputField() ---");
			return null;
		}

		private InputField GetInputField(string name)
		{
			InputField inputObj = GameObject.Find(name).GetComponent<InputField>();
			return inputObj;
		}

		private IMEManagerClientOLD GetIMEClient(GameObject GO)
		{
			if (bDebug) Log.d(LOG_TAG, "GetIMEScript() +++");
			IMEManagerClientOLD inputClient = GO.GetComponent<IMEManagerClientOLD>();
			return inputClient;
		}


		public void showKeyboard(GameObject GO)
		{
			if (bDebug) Log.i(LOG_TAG, "ShowKeyboard");

			if (GO == null)
			{
				if (bDebug) Log.e(LOG_TAG, "ShowKeyboard(null) from null GO");
				new System.NullReferenceException();
			}
			m_InputField = GetInputField(GO);
			focusGOParentGOname = Utils.GameObjectUtils.getParentGOname(GO);
			if (bDebug) Log.i(LOG_TAG, "focusGOParentGOname = " + focusGOParentGOname);
			if (m_InputField != null)
			{
				m_InputField.shouldHideMobileInput = true;
				setCallbacks(GO);
				if (bDebug) Log.i(LOG_TAG, "InputField.text = " + m_InputField.text);
				onInputClickedSB = new StringBuilder(m_InputField.text);
				mIMEWrapper.SetText(m_InputField.text);
			}
			else
			{
				if (bDebug) Log.e(LOG_TAG, "s_InputGameObject=" + GO + " m_InputField == " + m_InputField);
			}
			mIMEWrapper.SetTitle("Input...");
			if (m_InputField.contentType == InputField.ContentType.Password)
				mIMEWrapper.SetLocale(IMEManagerWrapper2.Locale.Password);
			else if (m_InputField.contentType == InputField.ContentType.IntegerNumber)
			{
				mIMEWrapper.SetLocale(IMEManagerWrapper2.Locale.IntegerNumber);
				mIMEWrapper.SetClickedCallback(InputClickCallbackImpl);
			}
			else
				mIMEWrapper.SetLocale(IMEManagerWrapper2.Locale.en_US);
			mIMEWrapper.SetAction(IMEManagerWrapper2.Action.Enter);
			mIMEWrapper.Show(mIsShowKeyboardInputPanel);
		}

#if VUPLEX
		public void showKeyboardOnWebview(CanvasWebViewPrefab webview)
		{
			if (bDebug) Log.i(LOG_TAG, "showKeyboardOnWebview ++");

			if (webview == null)
			{
				if (bDebug) Log.e(LOG_TAG, "showKeyboardOnWebview(null) from null GO");
				new System.NullReferenceException();
				return;
			}

			onInputClickedSB = new StringBuilder(""); // Vuplex not support API to  get the existing text of InputField web page.
													  // setCallbacks();
			mIMEWrapper.SetText("");
			mIMEWrapper.SetTitle("WvInput...");
			mIMEWrapper.SetLocale(IMEManagerWrapper2.Locale.en_US);
			mIMEWrapper.SetAction(IMEManagerWrapper2.Action.Enter);
			mIMEWrapper.SetDoneCallback(InputDoneCallbackImpl);
			// mIMEWrapper.SetClickedCallback(InputClickCallbackImpl);
			m_Webview = webview;
			m_Webview.WebView.SelectAll();
			m_Webview.WebView.HandleKeyboardInput("Delete");
			mIMEWrapper.Show(mIsShowKeyboardInputPanel);
			if (bDebug) Log.i(LOG_TAG, "showKeyboardOnWebview --");
		}
#endif
		public void hideKeyboard()
		{
			if (bDebug) Log.i(LOG_TAG, "hideKeyboard");
			mIMEWrapper.Hide();
			onInputClickedSB = null;
#if VUPLEX
			m_Webview = null;
#endif
		}

		public void InputDoneCallbackImpl(IMEManagerWrapper2.InputResult results)
		{
			if (bDebug) Log.d(LOG_TAG, "inputDoneCallbackImpl: " + results.GetContent());
			if (m_InputField != null && m_InputField.contentType == InputField.ContentType.IntegerNumber) return;
			mInputContent = results.GetContent();
		}
		public void InputClickCallbackImpl(IMEManagerWrapper2.InputResult results)
		{
			if (bDebug) Log.d(LOG_TAG, "InputClickCallbackImpl:  clickedKeyChar=" + results.GetContent());

			// Note: directly update input field text in UI thread will exception
			// use LastUpdate to update Input field text
			if (onInputClickedSB != null)
			{
				if (results.GetKeyCode() == IMEManager.InputResult.Key.BACKSPACE)
				{
					if (bDebug) Log.d(LOG_TAG, "on clicked BACKSPACE key");
					if (onInputClickedSB.Length > 1)
					{
						//mInputContent = (onInputClickedSB.Length - 1).ToString();
						onInputClickedSB.Remove(onInputClickedSB.Length - 1, 1);
						mInputContent = onInputClickedSB.ToString();
					}
					else if (onInputClickedSB.Length == 1)
					{
						mInputContent = "";
					}

				}
				else if (results.GetKeyCode() == IMEManager.InputResult.Key.ENTER)
				{
					if (bDebug) Log.d(LOG_TAG, "on clicked ENTER key");
					hideKeyboard();
				}
				else if (results.GetKeyCode() == IMEManager.InputResult.Key.CLOSE)
				{
					if (bDebug) Log.d(LOG_TAG, "on clicked CLOSE key");
					hideKeyboard();
				}
				else if (m_InputField.characterLimit == 0 || m_InputField.characterLimit > onInputClickedSB.Length)
				{
					onInputClickedSB.Append(results.GetContent());
					mInputContent = onInputClickedSB.ToString();
					if (bDebug) Log.d(LOG_TAG, "InputClickCallbackImpl   mInputContent=" + mInputContent + "  m_InputField=" + m_InputField + "  m_InputField.text=" + m_InputField.text + "  m_InputField.textComponent.text=" + m_InputField.textComponent.text);
				}
			}
			else
			{
				if (bDebug) Log.e(LOG_TAG, "m_InputField == null !!");
			}
		}

		private void UpdateInputField(string str)
		{
			if (bDebug) Log.d(LOG_TAG, "UpdateInputField()   m_InputField=" + m_InputField + "  str=" + str);
			if (m_InputField != null && str != null)
			{
				m_InputField.text = str;
			}
			else
			{
				if (bDebug) Log.d(LOG_TAG, "m_InputField=" + m_InputField + "   str=" + str);
			}
		}
#if VUPLEX
		private void UpdateWaveViewInputField(string str)
		{
			if (bDebug) Log.d(LOG_TAG, "UpdateWaveViewInputField()   m_Webview=" + m_Webview + "  str=" + str);
			if (m_Webview != null && str != null)
			{
				m_Webview.WebView.HandleKeyboardInput(str);
				m_Webview.WebView.SetFocused(false); // keyboard will not show during focusing, so I set no focuse every input done for showing keyboard next time.
			}
			else
			{
				if (bDebug) Log.d(LOG_TAG, "m_Webview=" + m_Webview + "   str=" + str);
			}
		}
#endif

		static public bool findChildGO(GameObject currentGO, string childGO)
		{
			var childCount = currentGO.transform.childCount;
			if (childCount != 0)
				for (int i = 0; i < childCount; i++)
					if (currentGO.transform.GetChild(i).gameObject.transform.name == childGO)
					{
						if (bDebug) Log.d(LOG_TAG, "found child GO=" + childGO);
						return true;
					}

			return false;
		}


		void LateUpdate()
		{
			if (mInputContent != null)
			{
				UpdateInputField(mInputContent);
#if VUPLEX
				UpdateWaveViewInputField(mInputContent);
#endif
				mInputContent = null;
			}
		}

		public int getKeyboardStatus()
		{
			int state = mIMEWrapper.getKeyboardState();
			if (bDebug) Log.d(LOG_TAG, "getKeyboardState()= " + state);
			return state;
		}

	}
}
