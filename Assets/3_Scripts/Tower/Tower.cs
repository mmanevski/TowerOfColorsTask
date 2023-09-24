using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Tower Settings For Cylindrical Tower")]
    public float TileHeight = 1.2f;
    public float TileRadius = 0.5f;

    [Header("Parameters For Cylindrical Tower")]
    public int TileCountPerFloor = 15;

    [Header("Parameters For Box Tower")]
    public int TilesPerRow = 5;
    public int TilesPerColumn = 5;

    public int FloorCount = 15;
    public int PlayableFloors = 8;
    public float SpecialTileChance = 0.1f;
    public TowerTile TilePrefab;
    public TowerTile[] SpecialTilePrefabs;
    public bool BuildOnStart = true;


    [Header("Scene")]
    public Transform CameraTarget;

    private List<List<TowerTile>> tilesByFloor;
    private int currentFloor = -1;
    private int maxFloor = 0;

    public System.Action<TowerTile> OnTileDestroyedCallback;

    private void Start()
    {
        if (BuildOnStart) {
            BuildTower();
        }
    }

    public float CaculateTowerRadius(float sideLength, float sideCount)
    {
        return sideLength / (2 * Mathf.Sin(Mathf.Deg2Rad * (180.0f / sideCount)));
    }

    public void BuildTower()
    {

        ResetTower();

        if (RemoteConfig.TOWER_BOX_SHAPE_ENABLED)
        {
            BuildBoxTower();
        }
        else
        {
            BuidCylindricalTower();
        }
        
    }

    private void BuidCylindricalTower()
    {
        tilesByFloor = new List<List<TowerTile>>();
        float towerRadius = CaculateTowerRadius(TileRadius * 2, TileCountPerFloor);
        float angleStep = 360.0f / TileCountPerFloor;
        Quaternion floorRotation = transform.rotation;
        for (int y = 0; y < FloorCount; y++)
        {
            tilesByFloor.Add(new List<TowerTile>());
            for (int i = 0; i < TileCountPerFloor; i++)
            {
                Quaternion direction = Quaternion.AngleAxis(angleStep * i, Vector3.up) * floorRotation;
                Vector3 position = transform.position + Vector3.up * y * TileHeight + direction * Vector3.forward * towerRadius;
                TowerTile tileInstance = Instantiate(Random.value > SpecialTileChance ? TilePrefab : SpecialTilePrefabs[Random.Range(0, SpecialTilePrefabs.Length)], position, direction * TilePrefab.transform.rotation, transform);
                tileInstance.SetColorIndex(Mathf.FloorToInt(Random.value * TileColorManager.Instance.ColorCount));
                tileInstance.SetFreezed(true);
                tileInstance.Floor = y;
                tileInstance.OnTileDestroyed += OnTileDestroyedCallback;
                tileInstance.OnTileDestroyed += OnTileDestroyed;
                tilesByFloor[y].Add(tileInstance);
            }
            floorRotation *= Quaternion.AngleAxis(angleStep / 2.0f, Vector3.up);
        }
        maxFloor = FloorCount - 1;

        SetCurrentFloor(tilesByFloor.Count - PlayableFloors);
        for (int i = 1; i < PlayableFloors; i++)
        {
            SetFloorActive(currentFloor + i, true);
        }
    }

    public int GetTotalNumberOfTiles()
    {
        int totalTiles = tilesByFloor.Count * tilesByFloor[0].Count;

        return totalTiles;
    }

    public void BuildBoxTower()
    {
        tilesByFloor = new List<List<TowerTile>>();
        float angleStep = 360.0f / TileCountPerFloor;
        Quaternion floorRotation = transform.rotation;
        for (int z = 0; z < FloorCount; z++)
        {
            tilesByFloor.Add(new List<TowerTile>());
            for (int x = 0; x < TilesPerRow; x++)
            {
                for (int y = 0; y < TilesPerColumn; y++)
                {
                    if (x > 0 && x < TilesPerRow -1)
                    {
                        if (y > 0 && y < TilesPerColumn-1)
                        {
                            continue;
                        }
                    }

                    float floorOffset = 0f;
                    if (z % 2 == 0)
                    {
                        floorOffset = TileRadius*0.5f;
                    }

                    Vector3 startPos = transform.position - new Vector3((TilesPerRow - 1) * TileRadius + floorOffset, 0f, (TilesPerColumn - 1) * TileRadius + floorOffset);

                    Quaternion direction = Quaternion.AngleAxis(angleStep * y, Vector3.up) * floorRotation;
                    Vector3 position = startPos + (Vector3.up * z * TileHeight) + (Vector3.right * TileRadius*2f * y) + Vector3.forward*TileRadius *2f * x;
                    TowerTile tileInstance = Instantiate(Random.value > SpecialTileChance ? TilePrefab : SpecialTilePrefabs[Random.Range(0, SpecialTilePrefabs.Length)], position, TilePrefab.transform.rotation, transform);
                    tileInstance.SetColorIndex(Mathf.FloorToInt(Random.value * TileColorManager.Instance.ColorCount));
                    tileInstance.SetFreezed(true);
                    tileInstance.Floor = z;
                    tileInstance.OnTileDestroyed += OnTileDestroyedCallback;
                    tileInstance.OnTileDestroyed += OnTileDestroyed;
                    tilesByFloor[z].Add(tileInstance);
                    floorRotation *= Quaternion.AngleAxis(angleStep / 2.0f, Vector3.up);
                   
                }
            }
        }
      
        maxFloor = FloorCount - 1;

        SetCurrentFloor(tilesByFloor.Count - PlayableFloors);
        for (int i = 1; i < PlayableFloors; i++)
        {
            SetFloorActive(currentFloor + i, true);
        }

    }


    public void OnTileDestroyed(TowerTile tile)
    {
        if (maxFloor > PlayableFloors - 1 && tilesByFloor != null) {
            float checkHeight = (maxFloor - 1) * TileHeight + TileHeight * 0.9f;
            float maxHeight = 0;
            foreach (List<TowerTile> floor in tilesByFloor) {
                foreach (TowerTile t in floor) {
                    if (t != null)
                        maxHeight = Mathf.Max(t.transform.position.y, maxHeight);
                }
            }
            if (maxHeight < checkHeight) {
                maxFloor--;
                if (currentFloor > 0) {
                    SetCurrentFloor(currentFloor - 1);
                }
            }
        }
    }

    public void ResetTower()
    {
        if (tilesByFloor != null) {
            foreach (List<TowerTile> tileList in tilesByFloor) {
                foreach (TowerTile tile in tileList) {
                    if (Application.isPlaying)
                        Destroy(tile.gameObject);
                    else
                        DestroyImmediate(tile.gameObject);
                }
                tileList.Clear();
            }
            tilesByFloor.Clear();
        }
    }

    public void StartGame()
    {
        StartCoroutine(StartGameSequence());
    }

    IEnumerator StartGameSequence()
    {
        for (int i = 0; i < tilesByFloor.Count - PlayableFloors; i++) {
            yield return new WaitForSeconds(0.075f * Time.timeScale);
            SetFloorActive(i, false, false);
        }
        yield return null;
    }

    public void SetCurrentFloor(int floor)
    {
        currentFloor = floor;
        CameraTarget.position = transform.position + Vector3.up * floor * TileHeight;
        SetFloorActive(currentFloor, true);
    }

    public void SetFloorActive(int floor, bool value, bool setFreezed = true)
    {
        foreach (TowerTile tile in tilesByFloor[floor]) {
            if (tile && tile.isActiveAndEnabled) {
                tile.SetEnabled(value);
                if (setFreezed)
                    tile.SetFreezed(!value);
            }
        }
    }

}
