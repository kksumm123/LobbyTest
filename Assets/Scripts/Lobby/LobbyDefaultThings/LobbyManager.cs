using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : Singleton<LobbyManager>
{
    [SerializeField] private LobbyFrameScrollViewController lobbyFrameScrollViewController;

    private void Start()
    {
        lobbyFrameScrollViewController.Initialize();
    }
}