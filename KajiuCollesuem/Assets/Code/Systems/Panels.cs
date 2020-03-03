﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Panels : MonoBehaviour
{
    [SerializeField] int playerNumber;
    [SerializeField] private connectedPlayers _AddPlayer;
    public Text playerPanels;
    private int stateValue = 0;
    [SerializeField] private MenuLizzy _myLizzy;

    [HideInInspector] public DeviceHandler myDevice = null;
    public UIManager uiMan;

    public Sprite[] abilityIcons;
    public SpriteRenderer[] ability = new SpriteRenderer[2];
    public RectTransform abilityOneRect, abilityTwoRect;
    public RectTransform dPadLeftRect, dPadRightRect;
    private int presetNumber = 0;
    public bool setOne;
    public bool setTwo;
    public bool abilityLocked;
    public bool animBool;
    public Animator animShield;
    public Animator animMask;

    public AudioClip[] lockedIn = new AudioClip[4];
    public AudioSource sfxSource;

    private void Start()
    {
        ability[0].GetComponent<SpriteRenderer>().sprite = abilityIcons[2];
        ability[1].GetComponent<SpriteRenderer>().sprite = abilityIcons[3];
    }

    private void Update()
    {
        if (animShield.GetCurrentAnimatorStateInfo(0).IsName("Has Left"))
        {
            abilityOneRect.DOAnchorPos(new Vector2(0, 34), 0.4f);
            dPadLeftRect.DOAnchorPos(new Vector2(-157, -201), 0.4f);
            abilityTwoRect.DOAnchorPos(new Vector2(0, -145), 0.4f);
            dPadRightRect.DOAnchorPos(new Vector2(157, -201), 0.4f);
        }

        if (uiMan.panelCheck == true)
        {
            abilityOneRect.DOAnchorPos(new Vector2(0, 34), 0.01f);
            dPadLeftRect.DOAnchorPos(new Vector2(-157, -201), 0.01f);
            abilityTwoRect.DOAnchorPos(new Vector2(0, -145), 0.01f);
            dPadRightRect.DOAnchorPos(new Vector2(157, -201), 0.01f);
        }
        animShield.SetBool("hasJoined", animBool);
        animMask.SetBool("maskJoined", animBool);
    }

    public void OnJoining()
    {
        switch (stateValue)
        {
            case 0:
                if(stateValue == 0)
                {
                    sfxSource.clip = lockedIn[Random.Range(0, 3)];
                    sfxSource.volume = Random.Range(0.6f, 0.8f);
                    sfxSource.PlayDelayed(0.25f);
                    playerPanels.text = "READY!";
                    animBool = true;
                    abilityOneRect.DOAnchorPos(new Vector2(0, -1930), 1.6f);
                    dPadLeftRect.DOAnchorPos(new Vector2(-157, -2131), 1.6f);
                    abilityTwoRect.DOAnchorPos(new Vector2(0, -2110), 1.6f);
                    dPadRightRect.DOAnchorPos(new Vector2(157, -2131), 1.6f);
                    abilityLocked = true;
                    stateValue = 1;
                    _myLizzy.ChangeWeights(MenuLizzy.menuLizzyStates.LockedIn);
                }
                break;
            
            case 1:
                if (connectedPlayers.playersConnected >= 2 && abilityLocked)
                {
                    _AddPlayer.SetPlayerOrder();
                    SceneManager.LoadSceneAsync(1);
                }
                break;
        }
    }

    public void OnLeft()
    {
        if(stateValue == 0)
        {
            ChangeIcons(-1);
        }
    }

    public void OnRight()
    {
        if (stateValue == 0)
        {
            ChangeIcons(1);
        }
    }

    public void ChangeIcons(int pDirection)
    {
        presetNumber += pDirection;

        // Check if outside bounds
        if(presetNumber < 0)
        {
            presetNumber = abilityIcons.Length / 2 - 1;
        }
        else if(presetNumber >= abilityIcons.Length / 2)
        {
            presetNumber = 0;
        }

        ability[0].GetComponent<SpriteRenderer>().sprite = abilityIcons[presetNumber * 2];
        ability[1].GetComponent<SpriteRenderer>().sprite = abilityIcons[presetNumber * 2 + 1];
    }

    public void PlayerJoined(DeviceHandler pDevice)
    {
        myDevice = pDevice;

        playerPanels.text = " ";
        presetNumber = 0;
        abilityOneRect.DOAnchorPos(new Vector2(0, 34), 0.4f);
        dPadLeftRect.DOAnchorPos(new Vector2(-157, -201), 0.4f);
        abilityTwoRect.DOAnchorPos(new Vector2(0, -145), 0.4f);
        dPadRightRect.DOAnchorPos(new Vector2(157, -201), 0.4f);
        ability[0].GetComponent<SpriteRenderer>().sprite = abilityIcons[2];
        ability[1].GetComponent<SpriteRenderer>().sprite = abilityIcons[3];

        _myLizzy.ChangeWeights(MenuLizzy.menuLizzyStates.Joined);
    }

    public int PlayerLeft()
    {
        myDevice = null;

        playerPanels.text = "Press 'Space'\nor\n'Start' to Join";
        animBool = false;
        stateValue = 0;

        _myLizzy.ChangeWeights(MenuLizzy.menuLizzyStates.NotJoined);

        return playerNumber - 1;
    }
}