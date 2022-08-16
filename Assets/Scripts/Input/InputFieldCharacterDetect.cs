using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Wave.Native;

namespace Hubble.Launcher.Input
{
    class InputFieldCharacterDetect
    {
		private static string LOG_TAG = "InputFieldCharacterDetect";

		static public int getCharacterLimit(GameObject currentGO) // BUG: use TMP .characterLimit will crash for unknown reason, so I use this detection to limit input char.
		{
			int charLimit = 99;
			if (currentGO.transform.name.Equals("InputField"))
			{
				string parentGOName = Utils.GameObjectUtils.getParentGOname(currentGO);
				switch (parentGOName)
				{
					case "CodeInputPanel": // BT
						charLimit = 16;
						break;
					case "ManualProxyPanel": // Wifi  AdvancedOptionsExtendPanel
					case "Bypass":
					case "AutoConfigProxyPanel":
					case "Address":
					case "Gateway":
					case "DNS 1":
					case "DNS 2":
						charLimit = 15;
						break;
					case "Port":
						charLimit = 5;
						break;
					case "NetworkPrefixLength":
						break;
					case "NameInput": // EditWiFiPage
					case "IdentityInput":
					case "AnonymousIdentityInput":
					case "PasswordInput":
					case "HostName":
					case "SetDeviceNameWindow":
						charLimit = 60;
						break;
					case "PacUrl":
						charLimit = 256;
						break;
					case "NameInputItem": // ReportIssuePage
						charLimit = 80;
						break;
					case "EmailInputItem":
						charLimit = 256;
						break;
					case "DescriptionInputItem":
						charLimit = 2048;
						break;
					case "ServerAddressInputField": // EditVPNProfilePage
						charLimit = 15;
						break;
					case "L2TPSecretInputField":
						charLimit = 65;
						break;
					case "IPSecIdentifierInputField":
						charLimit = 60;
						break;
					case "IPSecPreShareKeyInputField":
						charLimit = 128;
						break;
					case "DNSSearchDomainInputField":
						break;
					case "DNSServersInputField":
						charLimit = 15;
						break;
					case "DNSForwardingRoutesInputField":
						charLimit = 15;
						break;
					case "ProxyHostnameInputField":
						charLimit = 60;
						break;
					case "ProxyPortInputField":
						charLimit = 5;
						break;
					case "UserNameInputField":
						charLimit = 60;
						break;
					case "PasswordInputField":
						charLimit = 60;
						break;
					default:
						break;
				}
				Log.i(LOG_TAG, "InputField parent Name =" + parentGOName + "   charLimit=" + charLimit);
			}
			else // name != "InputField"
			{
				switch (currentGO.transform.name)
				{
					case "InputField_Hour":   // hh
					case "InputField_Minute": // mm
					case "MonthInputField":   // MM
					case "DayInputField":     // DD
						charLimit = 2;
						break;
					case "YearInputField":    // YYYY
						charLimit = 4;
						break;
					default:
						break;
				}
				Log.i(LOG_TAG, "InputField Name =" + currentGO.transform.name + "   charLimit=" + charLimit);
			}
			return charLimit;
		}

	}
}
