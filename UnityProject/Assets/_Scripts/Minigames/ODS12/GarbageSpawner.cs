using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.VFX;
using static CloudSpawner;

public class GarbageSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _glass;
    [SerializeField] private GameObject _paper;
    [SerializeField] private GameObject _plastic;

    [SerializeField] private Transform garbageSpawnTransform;

    [SerializeField] private Slider spawnTimerSlider;

    [SerializeField] private VisualEffect _spawnVFX;

    private float _timeToSpawnReference;

    private bool _canSpawn;

    public AudioSource SpawnSound;

    private void Awake()
    {
        
    }

    private void Start()
    {
        ODS12Singleton.Instance.OnGameStartEvent += OnGameStart;
        spawnTimerSlider.maxValue = ODS12Singleton.Instance.currentGarbSpawnTime;
        spawnTimerSlider.value = ODS12Singleton.Instance.currentGarbSpawnTime;
        _timeToSpawnReference = Time.time;
    }

    private void Update()
    {
        if (!ODS12Singleton.Instance.gameIsActive) return;
        if (CanSpawnGarbage())
        {
            SpawnGarbage();
        }
    }

    private void OnGameStart()
    {
        _timeToSpawnReference = Time.time;
        _canSpawn = true;
        SpawnGarbage();
        ODS12Singleton.Instance.OnGameStartEvent -= OnGameStart;
    }

    private bool CanSpawnGarbage()
    {
        if (!_canSpawn)
            return false;
        
        ODS12Singleton.Instance.CheckStageChange();
        spawnTimerSlider.maxValue = ODS12Singleton.Instance.currentGarbSpawnTime;
        spawnTimerSlider.value = ODS12Singleton.Instance.currentGarbSpawnTime;

        if (ODS12Singleton.Instance.maxGarbage <= ODS12Singleton.Instance.currentGarbage)
        {
            _timeToSpawnReference = Time.time;
            return false;
        }
    
        float TimeSpawn = ODS12Singleton.Instance.currentGarbSpawnTime - ((Time.time - _timeToSpawnReference));
        TimeSpawn = Mathf.Clamp(TimeSpawn, 0, ODS12Singleton.Instance.currentGarbSpawnTime);
    
        spawnTimerSlider.value = TimeSpawn;
    
        if (TimeSpawn == 0)
            return true;
    
        return false;
    }

    private void SpawnGarbage()
    {
        int garbageType = UnityEngine.Random.Range(0, 3);
        Vector3 spawnPosition = garbageSpawnTransform.position;
        _spawnVFX.Play();

        SpawnSound.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        SpawnSound.Play();

        switch (garbageType)
        {
            case 0:
                GameObject Plastic = Instantiate(_plastic, garbageSpawnTransform.position, Quaternion.identity);
                break;
            case 1:
                GameObject Paper = Instantiate(_paper, garbageSpawnTransform.position, Quaternion.identity);
                break;
            case 2:
                GameObject Glass = Instantiate(_glass, garbageSpawnTransform.position, Quaternion.identity);
                break;
        }
        _timeToSpawnReference = Time.time;
        ODS12Singleton.Instance.OnGarbageCreated.Invoke();
    }
}
