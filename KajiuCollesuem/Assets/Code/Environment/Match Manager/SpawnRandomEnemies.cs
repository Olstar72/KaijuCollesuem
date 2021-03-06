﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Programmer: Kavian Kermani
Additional Programmers: Oliver Loescher
Description: Spawns a random number of enemies, in preset locations randomly.
*/

public class SpawnRandomEnemies : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _goldEnemyPrefab;

    [SerializeField] [Range(0, 1)] float _oddsOffGoldSpawn = 0.02f; 

    [SerializeField] private int enemySpawnCount = 12;
    
    private List<Transform> _enemySpawnPoints = new List<Transform>();
    private int _enemySpawnPointIndex;

    private void Awake()
    {
        foreach (Transform children in GetComponentInChildren<Transform>())
        {
            _enemySpawnPoints.Add(children);
        }
    }

    public void SpawnEnemies()
    {
        if (enemySpawnCount > _enemySpawnPoints.Count - 1)
            enemySpawnCount = _enemySpawnPoints.Count - 1;

        for (int i = 0; i < enemySpawnCount; i++)
        {
            // Randomly Choose between normal and gold Hellhound
            GameObject prefab;
            if (Random.value < _oddsOffGoldSpawn)
                prefab = _goldEnemyPrefab;
            else
                prefab = _enemyPrefab;

            _enemySpawnPointIndex = Random.Range(0, _enemySpawnPoints.Count);
            Instantiate(prefab, _enemySpawnPoints[_enemySpawnPointIndex].position, _enemySpawnPoints[_enemySpawnPointIndex].rotation);
            _enemySpawnPoints.Remove(_enemySpawnPoints[_enemySpawnPointIndex]);
        }
    }
}
