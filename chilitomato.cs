using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Linq;

public class ChiliTomatoDetector : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added.Concat(args.updated))
        {
            if (trackedImage.referenceImage.name == "ChiliTomatoLogo" &&
                trackedImage.trackingState == TrackingState.Tracking)
            {
                Debug.Log("✅ ロゴ画像を検出！ 色チェックに進みます...");

                // テクスチャを取得
                Texture2D tex = trackedImage.referenceImage.texture;
                if (tex != null)
                {
                    if (CheckRedGreen(tex))
                    {
                        Debug.Log("🍜 チリトマトを検出しました！");
                    }
                    else
                    {
                        Debug.Log("⚠️ ロゴはあるが、色条件を満たさず");
                    }
                }
            }
        }
    }

    private bool CheckRedGreen(Texture2D tex)
    {
        Color32[] pixels = tex.GetPixels32();
        int total = pixels.Length;
        int redCount = 0, greenCount = 0;

        foreach (var p in pixels)
        {
            Color.RGBToHSV(p, out float h, out float s, out float v);

            // 赤 (0〜10度 or 350〜360度)
            if ((h < 0.05f || h > 0.95f) && s > 0.4f && v > 0.3f)
                redCount++;

            // 緑 (100〜140度付近)
            if (h > 0.25f && h < 0.4f && s > 0.4f && v > 0.3f)
                greenCount++;
        }

        float redRatio = (float)redCount / total;
        float greenRatio = (float)greenCount / total;

        Debug.Log($"[色判定] 赤:{redRatio:F2}, 緑:{greenRatio:F2}");

        // ゆるめの条件
        return (redRatio > 0.05f && greenRatio > 0.02f);
    }
}
