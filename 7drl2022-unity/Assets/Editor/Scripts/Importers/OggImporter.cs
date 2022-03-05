using UnityEngine;
using UnityEditor;

public class OggImporter : AssetPostprocessor {

    private const string TagLoopStart = "LOOPSTART";
    private const string TagLoopEnd = "LOOPEND";

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
        foreach (var assetPath in importedAssets) {
            if (!assetPath.EndsWith(".ogg")) {
                return;
            }

            var fileName = assetPath.Substring(0, assetPath.LastIndexOf('.'));
            var looper = AssetDatabase.LoadAssetAtPath<LoopableAudioClipData>(fileName + ".asset");
            if (looper == null) {
                looper = ScriptableObject.CreateInstance<LoopableAudioClipData>();
                AssetDatabase.CreateAsset(looper, fileName + ".asset");
            }
            fileName = fileName.Substring(0, fileName.LastIndexOf('/'));

            using (var vorbis = new NVorbis.VorbisReader(assetPath)) {
                var loopStartString = vorbis.Tags.GetTagSingle(TagLoopStart);
                if (long.TryParse(loopStartString, out long loopStart)) {
                    looper.loopBeginSample = loopStart;
                }
                var loopEndString = vorbis.Tags.GetTagSingle(TagLoopEnd);
                if (long.TryParse(loopEndString, out long loopEnd)) {
                    looper.loopEndSample = loopEnd;
                } else {
                    looper.loopEndSample = vorbis.TotalSamples;
                }
            }

            looper.name = fileName;
            var clipAsset = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
            if (clipAsset != null) {
                looper.clip = clipAsset;
            }

            EditorUtility.SetDirty(looper);
            AssetDatabase.SaveAssets();
        }
    }
}
