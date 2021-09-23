using System.IO;
using UnityEditor;
using UnityEngine;

namespace RV
{
	internal static class Entrypoint
	{
		[InitializeOnLoadMethod]
		private static async void InitializeOnLoadMethod()
		{
			// when there is no directory for cached object.
			DirectoryInfo rootDirectory = new DirectoryInfo(FileSystem.CacheDirectory);
			if (!rootDirectory.Exists)
			{ 
				rootDirectory.Create();

				// force re-indexing
				if (ReferenceSerializer.HasLocalVersion())
				{
					int version = ReferenceSerializer.GetLocalVersion();
					if (version < 100)
					{
						if (EditorUtility.DisplayDialog(
							Localize.Inst.dialog_titleContent,
							Localize.Inst.dialog_indexedAssetExpired,
							Localize.Inst.dialog_enablePlugin,
							Localize.Inst.dialog_disablePlugin))
						{
							await ReferenceCache.CleanUpAssets();
							await ReferenceCache.IndexAssets();
						
							ReferenceSetting.IsEnabled = true;
						}
						else
						{
							ReferenceSetting.IsEnabled = false;
						}
					}
					
					return;
				}

				// first indexing
				await ReferenceDialog.OpenIndexAllAssetDialog();
			}
		}
	}
}