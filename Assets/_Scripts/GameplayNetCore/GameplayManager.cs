using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance;


    public GameObject _gameManager;
    public NetworkManager networkManager;
    public PlayerManager playerManager;
    public ItemsFactory itemsFactory;
    public ItemSpawner itemSpawner;
    public itemEffectInvoker itemEffectInvoker;
    public scoreManager scoreManager;
    public KartAssetManager kartAssetManager;
    public ObjectPool objectPool;
    public GameplayUIManager gameplayUIManager;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

}
