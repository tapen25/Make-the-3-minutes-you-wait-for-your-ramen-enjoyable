using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SimpleTomatoDrop : MonoBehaviour
{
    public GameObject tomatoPrefab;   // 落としたいトマトPrefab
    public float heightAboveImage = 0.3f; // 画像の上にどのくらいの高さから出すか

    private ARTrackedImageManager _imageManager;
    private bool spawned = false;

    void Awake()
    {
        _imageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        _imageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        _imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var img in args.added)
        {
            SpawnTomato(img);
        }
        foreach (var img in args.updated)
        {
            if (img.trackingState == TrackingState.Tracking)
                SpawnTomato(img);
        }
    }

    void SpawnTomato(ARTrackedImage img)
    {
        if (spawned) return; // 一回だけ落とす

        Vector3 spawnPos = img.transform.position + Vector3.up * heightAboveImage;
        Instantiate(tomatoPrefab, spawnPos, Quaternion.identity);
        spawned = true;
    }
}
