using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject characterPrefab; // キャラのPrefab

    private GameObject spawnedCharacter;

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // 新しく検出された画像に対して
        foreach (var trackedImage in eventArgs.added)
        {
            // キャラを出現させる
            spawnedCharacter = Instantiate(
                characterPrefab,
                trackedImage.transform.position,
                trackedImage.transform.rotation
            );

            // キャラに付いてる TomatoSpawner を探して実行
            TomatoSpawner spawner = spawnedCharacter.GetComponent<TomatoSpawner>();
            if (spawner != null)
            {
                spawner.SpawnTomatoes();
            }
        }
    }
}
