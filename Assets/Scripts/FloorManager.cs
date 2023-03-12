using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] GameObject[] FloorPrefabs;
    public void SpawnFloor()
    {
        // 在不同的地板中隨機選擇一種
        int type = Random.Range(0, this.FloorPrefabs.Length);
        // 生成一個新的地板物件，並生成在FloorManager底下
        GameObject newFloor = Instantiate(FloorPrefabs[type], transform);
        newFloor.transform.position = new Vector3(
            Random.Range(-6f, 8f),
            -6.5f,
            0f
        );
    }
}
