#if UNITY_EDITOR
using UnityEditor;
using System;

public class ModelImportSettings : AssetPostprocessor {
	void OnPreprocessModel() {
		ModelImporter importer = assetImporter as ModelImporter;
		String name = importer.assetPath.ToLower();
		if (name.Substring(name.Length - 4, 4)==".fbx") {



			//importer.globalScale = 1.0F;
			//importer.generateAnimations = ModelImporterGenerateAnimations.None;
			//importer.animationType = ModelImporterAnimationType.None;
			//importer.importBlendShapes = false;
			importer.importMaterials = false;



		}
	}
}
#endif
