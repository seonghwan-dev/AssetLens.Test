﻿using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RV
{
	internal sealed class ReferenceSetting : ScriptableObject
	{
		public const uint INDEX_VERSION = 100;

		private static ReferenceSetting instance = default;
		private const string k_editorCustomSettingsRoot = "Assets/Editor Default Resources";
		private const string k_editorCustomSettingsPath = k_editorCustomSettingsRoot + "/Reference Setting.asset";

		[SerializeField] private bool enabled = false;
		[SerializeField] private bool pauseInPlaymode = true;
		[SerializeField] private bool traceSceneObject = false;
		[SerializeField] private bool useEditorUtilityWhenSearchDependencies = false;
		[SerializeField] private bool displayIndexerVersion = false;

		[SerializeField] private string localization = "English";

		public static bool IsEnabled {
			get => GetOrCreateSettings().enabled;
			set
			{
				GetOrCreateSettings().enabled = value;
				EditorUtility.SetDirty(GetOrCreateSettings());
			}
		}

		public static bool PauseInPlaymode => GetOrCreateSettings().pauseInPlaymode;

		public static bool TraceSceneObject => GetOrCreateSettings().traceSceneObject;

		public static bool UseEditorUtilityWhenSearchDependencies =>
			GetOrCreateSettings().useEditorUtilityWhenSearchDependencies;


		public static bool DisplayIndexerVersion => GetOrCreateSettings().displayIndexerVersion;

		public static Localize LoadLocalization {
			get
			{
				string locale = GetOrCreateSettings().localization;
				string fullPath = Path.GetFullPath($"{FileSystem.PackageDirectory}/Languages/{locale}.json");

				string json = File.ReadAllText(fullPath);
				return JsonUtility.FromJson<Localize>(json);
			}
		}

		public static string Localization {
			set
			{
				GetOrCreateSettings().localization = value;
				EditorUtility.SetDirty(GetOrCreateSettings());
			}
		}

		private static ReferenceSetting GetOrCreateSettings()
		{
			if (instance != null)
			{
				return instance;
			}

			instance = EditorGUIUtility.Load(k_editorCustomSettingsPath) as ReferenceSetting;

			if (instance == null)
			{
				instance = CreateInstance<ReferenceSetting>();
				instance.enabled = false;

				if (!Directory.Exists(k_editorCustomSettingsRoot))
				{
					Directory.CreateDirectory(k_editorCustomSettingsRoot);
					AssetDatabase.ImportAsset(k_editorCustomSettingsRoot);
				}

				AssetDatabase.CreateAsset(instance, k_editorCustomSettingsPath);
				AssetDatabase.SaveAssets();
			}

			return instance;
		}

		internal class AssetDataSettingsProviderRegister
		{
			[SettingsProvider]
			public static SettingsProvider CreateFromSettingsObject()
			{
				Object settingsObj = EditorGUIUtility.Load(k_editorCustomSettingsPath);

				AssetSettingsProvider provider =
					AssetSettingsProvider.CreateProviderFromObject("Project/Reference", settingsObj);

				provider.keywords =
					SettingsProvider.GetSearchKeywordsFromSerializedObject(
						new SerializedObject(settingsObj));
				return provider;
			}
		}
	}
}