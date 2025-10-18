using UnityEngine;

public class TomatoSpawner : MonoBehaviour
{
    public GameObject tomatoPrefab; // トマトPrefab
    public int tomatoCount = 20;    // 飛ばす数
    public float force = 5f;        // 飛ばす力

    // トマトを飛ばす処理
    public void SpawnTomatoes()
    {
        for (int i = 0; i < tomatoCount; i++)
        {
            // キャラクターの体（このオブジェクト）から生成
            GameObject tomato = Instantiate(
                tomatoPrefab,
                transform.position + Random.insideUnitSphere * 0.5f, // 少しバラける
                Random.rotation // ランダム回転
            );

            // Rigidbodyで飛ばす
            Rigidbody rb = tomato.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDir = Random.onUnitSphere; 
                rb.AddForce(randomDir * force, ForceMode.Impulse);
            }
        }
    }
}
