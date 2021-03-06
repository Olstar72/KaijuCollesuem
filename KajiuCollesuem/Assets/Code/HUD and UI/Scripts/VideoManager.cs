﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;

public class VideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public AudioSource announceSource;
    public GameObject mainMenu;
    public GameObject logoVisual;
    public Animator logo;
    public Image background;
    public bool introEnded;

    void Start()
    {
        mainMenu.SetActive(false);
        logoVisual.SetActive(false);
        videoPlayer.SetDirectAudioVolume(1, 0.5f);
        announceSource.PlayDelayed(5.2f);
        audioSource.PlayDelayed(7.5f);

        if (UIManager.menuProperties == true)
        {
            mainMenu.SetActive(true);
        }

        if (UIManager.menuProperties == false)
        {
            videoPlayer.Play();
            StartCoroutine("IntroTransition");
        }
    }

    IEnumerator IntroTransition()
    {
        yield return new WaitForSeconds(4.5f);
        logoVisual.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        mainMenu.SetActive(true);
        videoPlayer.SetDirectAudioVolume(0, 0);
        yield return new WaitForSeconds(0.5f);
        videoPlayer.gameObject.SetActive(false);
        logo.SetBool("introEnded", true);
        background.DOFade(0.95f, 1.6f);
        background.DOFade(0, 1.6f);
    }
}