//using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WinnerScreen : MonoBehaviour
{



    [SerializeField] private TMP_Text rankText;

    [SerializeField] private TMP_Text totalRankText;
    [SerializeField] private Color goldenColor;
    [SerializeField] private Color whiteColor;
    [SerializeField] private TMP_Text goldCoinText;

    [SerializeField] private TMP_Text silverCoinText;

    [SerializeField] private TMP_Text bronzeCoinText;

    [SerializeField] private TMP_Text killsText;

    [SerializeField] private Animation      winnerAnimation;
    [SerializeField] private AnimationClip  victoryAnimationClip;
    [SerializeField] private AnimationClip  gameOverAnimationClip;



    #region Monobehaviour callbacks
    private void OnEnable()
    {
        
    }


    private void Start()
    {
        
    }

    #endregion

    #region Public methods

    public void Test(int rank)
    {
        winnerAnimation.Stop();
        UpdateWinnerProperty(rank);
    }

    public void UpdateWinnerProperty(int rank)
    {
        Debug.Log($"{nameof(UpdateWinnerProperty)} \t");
        rankText.text = rank.ToString();
        goldCoinText.text = "11";
        silverCoinText.text = "22";
        bronzeCoinText.text = "33";
        killsText.text = "44";

        

        if (rankText.text == "1")
        {
            winnerAnimation.clip = victoryAnimationClip;
            //rankText.colorGradient = new VertexGradient(whiteColor, goldenColor, goldenColor, whiteColor);
          
        }
        else
        {
            winnerAnimation.clip = gameOverAnimationClip;
        }

        winnerAnimation.Play();
    }

    #endregion


}
