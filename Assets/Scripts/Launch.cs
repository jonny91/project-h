/*************************************************************************************
 *
 * 文 件 名:   Launch.cs
 * 描    述: 
 * 
 * 创 建 者：  洪金敏 
 * 创建时间：  2022-06-26 14:20:01
*************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Launch : MonoBehaviour
{
    private ApplicationState State;

    [SerializeField]
    private Image ImgLogo;

    [SerializeField]
    private GameObject[] ProductModelGroup;

    [SerializeField]
    private Button BtnBack;

    public static Launch Instance;

    private void Awake()
    {
        Instance = this;
        State = ApplicationState.Welcome;
    }

    public void OnClickModel()
    {
        if (State == ApplicationState.Welcome)
        {
            StartShow();
        }
    }

    public void Start()
    {
        BtnBack.gameObject.SetActive(false);
    }

    public void StartShow()
    {
        BtnBack.gameObject.SetActive(true);
        State = ApplicationState.Show;
    }

    public void Back()
    {
        BtnBack.gameObject.SetActive(false);
        State = ApplicationState.Welcome;
    }
}