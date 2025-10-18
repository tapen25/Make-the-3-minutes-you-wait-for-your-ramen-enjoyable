using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTomatoSpawner : MonoBehaviour
{
    [Header("AR")]
    public ARTrackedImageManager imageManager;

    [Header("Snow")]
    public GameObject snowballPrefab;
    [Tooltip("画像の中心からどれだけ上で出現させるか（m）")]
    public float spawnHeight = 0.25f;
    [Tooltip("1枚の画像あたり 1秒間に何個スポーンするか")]
    public float spawnRatePerImage = 20f;
    [Tooltip("画像サイズの内側にどれだけ余白を設けるか（m）")]
    public Vector2 padding = new Vector2(0.02f, 0.02f);
    [Tooltip("同時に保持する最大雪玉（プール）")]
    public int maxPool = 250;
    [Tooltip("Yがこの値より低く落ちたらリサイクル")]
    public float recycleY = -1.5f;
    [Tooltip("スポーンされてから自動で消す秒数（0で無効）")]
    public float lifetime = 6f;

    // 内部
    private Dictionary<TrackableId, Coroutine> _spawnRoutines = new();
    private readonly Queue<GameObject> _pool = new();
    private int _activeCount = 0;

    void OnEnable()
    {
        imageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        foreach (var kv in _spawnRoutines) StopCoroutine(kv.Value);
        _spawnRoutines.Clear();
    }

    void Update()
    {
        // 画面外へ落ちた雪玉を回収
        // （軽さ重視で簡易チェック）
        if (_activeCount == 0) return;
        foreach (var go in FindObjectsOfType<Rigidbody>())
        {
            if (!go || !go.gameObject.activeSelf) continue;
            if (go.transform.position.y < recycleY && go.gameObject.CompareTag("Untagged"))
            {
                // 何もタグ指定していない前提。必要なら "Snow" タグを作って使ってください。
                Recycle(go.gameObject);
            }
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        // 追加
        foreach (var img in args.added)
        {
            TryStartSpawn(img);
        }
        // 更新（トラッキングが切れたら止める／戻ったら再開）
        foreach (var img in args.updated)
        {
            if (img.trackingState == TrackingState.Tracking)
                TryStartSpawn(img);
            else
                TryStopSpawn(img);
        }
        // 削除
        foreach (var img in args.removed)
        {
            TryStopSpawn(img);
        }
    }

    void TryStartSpawn(ARTrackedImage img)
    {
        if (snowballPrefab == null || spawnRatePerImage <= 0f) return;

        var id = img.trackableId;
        if (_spawnRoutines.ContainsKey(id)) return;

        _spawnRoutines[id] = StartCoroutine(SpawnRoutine(img));
    }

    void TryStopSpawn(ARTrackedImage img)
    {
        var id = img.trackableId;
        if (_spawnRoutines.TryGetValue(id, out var co))
        {
            StopCoroutine(co);
            _spawnRoutines.Remove(id);
        }
    }

    IEnumerator SpawnRoutine(ARTrackedImage img)
    {
        var wait = new WaitForSeconds(1f / spawnRatePerImage);

        while (img != null && img.trackingState == TrackingState.Tracking)
        {
            // 画像の座標系内でランダムに出現
            Vector2 size = img.size; // m
            float halfX = size.x * 0.5f - padding.x;
            float halfZ = size.y * 0.5f - padding.y;

            float localX = Random.Range(-halfX, halfX);
            float localZ = Random.Range(-halfZ, halfZ);

            Vector3 localPos = new Vector3(localX, spawnHeight, localZ);
            Vector3 worldPos = img.transform.TransformPoint(localPos);

            var go = GetFromPool();
            go.transform.position = worldPos;
            go.transform.rotation = Quaternion.identity;

            // 初速を少しランダムに（横風っぽさ）
            if (go.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                Vector3 wind = new Vector3(Random.Range(-0.05f, 0.05f), 0f, Random.Range(-0.05f, 0.05f));
                rb.AddForce(wind, ForceMode.VelocityChange);
            }

            // 一定時間で自動リサイクル
            if (lifetime > 0f) StartCoroutine(AutoRecycle(go, lifetime));

            yield return wait;
        }
    }

    GameObject GetFromPool()
    {
        GameObject go;
        if (_pool.Count > 0)
        {
            go = _pool.Dequeue();
            go.SetActive(true);
        }
        else
        {
            if (_activeCount >= maxPool)
            {
                // 上限に達していたら最も古いものを再利用する設計も可
                // ここでは単に何もしない（生成しない）
                // 実用では古い雪玉を探して Recycle すると良い
                go = null;
            }
            else
            {
                go = Instantiate(snowballPrefab);
            }
        }

        if (go != null) _activeCount++;
        return go;
    }

    IEnumerator AutoRecycle(GameObject go, float t)
    {
        yield return new WaitForSeconds(t);
        if (go && go.activeSelf) Recycle(go);
    }

    void Recycle(GameObject go)
    {
        if (!go) return;
        go.SetActive(false);
        _pool.Enqueue(go);
        _activeCount = Mathf.Max(0, _activeCount - 1);
    }
}
