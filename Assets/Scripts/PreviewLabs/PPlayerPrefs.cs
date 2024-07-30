using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace PreviewLabs
{
	public static class PPlayerPrefs
	{
		private static readonly Hashtable playerPrefsHashtable;

		private static bool hashTableChanged;

		private static string serializedOutput;

		private static string serializedInput;

		private const string PARAMETERS_SEPERATOR = ";";

		private const string KEY_VALUE_SEPERATOR = ":";

		private static string[] seperators;

		private static readonly string fileName;

		private static readonly string secureFileName;

		private static byte[] bytes;

		private static bool wasEncrypted;

		private static bool securityModeEnabled;

		static PPlayerPrefs()
		{
			playerPrefsHashtable = new Hashtable();
			hashTableChanged = false;
			serializedOutput = string.Empty;
			serializedInput = string.Empty;
			seperators = new string[2]
			{
				";",
				":"
			};
			fileName = Application.persistentDataPath + "/PlayerPrefs.txt";
			secureFileName = Application.persistentDataPath + "/AdvancedPlayerPrefs.txt";
			bytes = Encoding.ASCII.GetBytes("iw3q" + SystemInfo.deviceUniqueIdentifier.Substring(0, 4));
			wasEncrypted = false;
			securityModeEnabled = false;
			StreamReader streamReader = null;
			if (File.Exists(secureFileName))
			{
				streamReader = new StreamReader(secureFileName);
				wasEncrypted = true;
				serializedInput = Decrypt(streamReader.ReadToEnd());
			}
			else if (File.Exists(fileName))
			{
				streamReader = new StreamReader(fileName);
				serializedInput = streamReader.ReadToEnd();
			}
			if (!string.IsNullOrEmpty(serializedInput))
			{
				if (serializedInput.Length > 0 && serializedInput[serializedInput.Length - 1] == '\n')
				{
					serializedInput = serializedInput.Substring(0, serializedInput.Length - 1);
					if (serializedInput.Length > 0 && serializedInput[serializedInput.Length - 1] == '\r')
					{
						serializedInput = serializedInput.Substring(0, serializedInput.Length - 1);
					}
				}
				Deserialize();
			}
			streamReader?.Close();
		}

		public static bool HasKey(string key)
		{
			return playerPrefsHashtable.ContainsKey(key);
		}

		public static void SetString(string key, string value)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			hashTableChanged = true;
		}

		public static void SetInt(string key, int value)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			hashTableChanged = true;
		}

		public static void SetFloat(string key, float value)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			hashTableChanged = true;
		}

		public static void SetBool(string key, bool value)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			hashTableChanged = true;
		}

		public static void SetLong(string key, long value)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			hashTableChanged = true;
		}

		public static string GetString(string key)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return playerPrefsHashtable[key].ToString();
			}
			return null;
		}

		public static string GetString(string key, string defaultValue)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return playerPrefsHashtable[key].ToString();
			}
			return defaultValue;
		}

		public static int GetInt(string key)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (int)playerPrefsHashtable[key];
			}
			return 0;
		}

		public static int GetInt(string key, int defaultValue)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (int)playerPrefsHashtable[key];
			}
			return defaultValue;
		}

		public static long GetLong(string key)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (long)playerPrefsHashtable[key];
			}
			return 0L;
		}

		public static long GetLong(string key, long defaultValue)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (long)playerPrefsHashtable[key];
			}
			return defaultValue;
		}

		public static float GetFloat(string key)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (float)playerPrefsHashtable[key];
			}
			return 0f;
		}

		public static float GetFloat(string key, float defaultValue)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (float)playerPrefsHashtable[key];
			}
			return defaultValue;
		}

		public static bool GetBool(string key)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (bool)playerPrefsHashtable[key];
			}
			return false;
		}

		public static bool GetBool(string key, bool defaultValue)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (bool)playerPrefsHashtable[key];
			}
			return defaultValue;
		}

		public static void DeleteKey(string key)
		{
			playerPrefsHashtable.Remove(key);
		}

		public static void DeleteAll()
		{
			playerPrefsHashtable.Clear();
		}

		public static bool WasReadPlayerPrefsFileEncrypted()
		{
			return wasEncrypted;
		}

		public static void EnableEncryption(bool enabled)
		{
			securityModeEnabled = enabled;
		}

		public static void Flush()
		{
			if (hashTableChanged)
			{
				Serialize();
				string value = (!securityModeEnabled) ? serializedOutput : Encrypt(serializedOutput);
				StreamWriter streamWriter = null;
				streamWriter = File.CreateText((!securityModeEnabled) ? fileName : secureFileName);
				File.Delete((!securityModeEnabled) ? secureFileName : fileName);
				if (streamWriter == null)
				{
					UnityEngine.Debug.LogWarning("PlayerPrefs::Flush() opening file for writing failed: " + fileName);
					return;
				}
				streamWriter.Write(value);
				streamWriter.Close();
				serializedOutput = string.Empty;
			}
		}

		private static void Serialize()
		{
			IDictionaryEnumerator enumerator = playerPrefsHashtable.GetEnumerator();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			while (enumerator.MoveNext())
			{
				if (!flag)
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(";");
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(EscapeNonSeperators(enumerator.Key.ToString(), seperators));
				stringBuilder.Append(" ");
				stringBuilder.Append(":");
				stringBuilder.Append(" ");
				stringBuilder.Append(EscapeNonSeperators(enumerator.Value.ToString(), seperators));
				stringBuilder.Append(" ");
				stringBuilder.Append(":");
				stringBuilder.Append(" ");
				stringBuilder.Append(enumerator.Value.GetType());
				flag = false;
			}
			serializedOutput = stringBuilder.ToString();
		}

		private static void Deserialize()
		{
			string[] array = serializedInput.Split(new string[1]
			{
				" ; "
			}, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Split(new string[1]
				{
					" : "
				}, StringSplitOptions.None);
				playerPrefsHashtable.Add(DeEscapeNonSeperators(array3[0], seperators), GetTypeValue(array3[2], DeEscapeNonSeperators(array3[1], seperators)));
				if (array3.Length > 3)
				{
					UnityEngine.Debug.LogWarning("PlayerPrefs::Deserialize() parameterContent has " + array3.Length + " elements");
				}
			}
		}

		public static string EscapeNonSeperators(string inputToEscape, string[] seperators)
		{
			inputToEscape = inputToEscape.Replace("\\", "\\\\");
			for (int i = 0; i < seperators.Length; i++)
			{
				inputToEscape = inputToEscape.Replace(seperators[i], "\\" + seperators[i]);
			}
			return inputToEscape;
		}

		public static string DeEscapeNonSeperators(string inputToDeEscape, string[] seperators)
		{
			for (int i = 0; i < seperators.Length; i++)
			{
				inputToDeEscape = inputToDeEscape.Replace("\\" + seperators[i], seperators[i]);
			}
			inputToDeEscape = inputToDeEscape.Replace("\\\\", "\\");
			return inputToDeEscape;
		}

		private static string Encrypt(string originalString)
		{
			if (string.IsNullOrEmpty(originalString))
			{
				return string.Empty;
			}
			DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);
			StreamWriter streamWriter = new StreamWriter(cryptoStream);
			streamWriter.Write(originalString);
			streamWriter.Flush();
			cryptoStream.FlushFinalBlock();
			streamWriter.Flush();
			return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
		}

		private static string Decrypt(string cryptedString)
		{
			if (string.IsNullOrEmpty(cryptedString))
			{
				return string.Empty;
			}
			DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
			MemoryStream stream = new MemoryStream(Convert.FromBase64String(cryptedString));
			CryptoStream stream2 = new CryptoStream(stream, dESCryptoServiceProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
			StreamReader streamReader = new StreamReader(stream2);
			return streamReader.ReadToEnd();
		}

		private static object GetTypeValue(string typeName, string value)
		{
			if (typeName == "System.String")
			{
				return value.ToString();
			}
			if (typeName == "System.Int32")
			{
				return Convert.ToInt32(value);
			}
			if (typeName == "System.Boolean")
			{
				return Convert.ToBoolean(value);
			}
			if (typeName == "System.Single")
			{
				return Convert.ToSingle(value);
			}
			if (typeName == "System.Int64")
			{
				return Convert.ToInt64(value);
			}
			UnityEngine.Debug.LogError("Unsupported type: " + typeName);
			return null;
		}
	}
}
