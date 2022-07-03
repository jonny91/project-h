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
using DG.Tweening;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.UI;

public class Launch : MonoBehaviour
{
    private ApplicationState State;

    [SerializeField]
    private Transform ShowPos;

    [SerializeField]
    private Animator LogoAnimator;

    [SerializeField]
    private Button BtnBack;

    [SerializeField]
    private GameObject[] HomeModelGroup;

    [SerializeField]
    private Transform[] MenuPosArr;

    [SerializeField]
    private Transform[] HomePosArr;

    public float ShowTime = 0.5f;

    public static Launch Instance;

    private void Awake()
    {
        Instance = this;
        State = ApplicationState.Welcome;
    }

    public void OnTap(GestureRecognizer gesture)
    {
        var ray = Camera.main.ScreenPointToRay(new Vector3(gesture.FocusX, gesture.FocusY, 0));
        var hitList = Physics.RaycastAll(ray);
        foreach (var raycastHit in hitList)
        {
            print(raycastHit.collider.name);
            OnClickModel(raycastHit.collider.gameObject);
        }
    }

    private void OnClickModel(GameObject clickedObject)
    {
        if (State == ApplicationState.Welcome)
        {
            var modelData = clickedObject.GetComponent<ModelData>();
            if (modelData)
            {
                StartShow();
                MoveToCenter(clickedObject);
                MoveToMenu(clickedObject);
            }
        }
    }

    /// <summary>
    /// 要展示的物品到屏幕中间
    /// </summary>
    /// <param name="clickedObject"></param>
    private void MoveToCenter(GameObject clickedObject)
    {
        clickedObject.transform.parent = ShowPos;
        clickedObject.transform.DOLocalMove(Vector3.zero, ShowTime);

        clickedObject.GetComponent<Animator>().SetBool("show", true);
    }

    private void MoveToMenu(GameObject clickedObject)
    {
        var menuIndex = 0;
        for (int i = 0; i < HomeModelGroup.Length; i++)
        {
            var m = HomeModelGroup[i];
            //不点击的这个 依次去菜单中
            if (m != clickedObject)
            {
                var menuPos = MenuPosArr[menuIndex];
                m.transform.parent = menuPos;

                m.transform.DOLocalMove(Vector3.zero, ShowTime);
                m.GetComponent<Animator>().SetBool("show", false);
                m.GetComponent<Animator>().SetTrigger("idle");
                menuIndex++;
            }
        }
    }

    private void ModelBackToHome()
    {
        for (int i = 0; i < HomeModelGroup.Length; i++)
        {
            var m = HomeModelGroup[i];
            var modelIndex = m.GetComponent<HomeModelIndex>();
            var targetPosTrans = HomePosArr[modelIndex.Index];
            m.transform.SetParent(targetPosTrans);
            m.transform.DOLocalMove(Vector3.zero, ShowTime);

            m.GetComponent<Animator>().SetBool("show", true);
        }
    }

    public void Start()
    {
        BtnBack.gameObject.SetActive(false);
        BtnBack.onClick.AddListener(Back);
        GoHome();
    }

    public void StartShow()
    {
        BtnBack.gameObject.SetActive(true);
        State = ApplicationState.Show;
        LogoAnimator.SetTrigger("show");
    }

    private void GoHome()
    {
        Debug.Log("返回主界面");
        LogoAnimator.SetTrigger("home");
        ModelBackToHome();
    }

    public void Back()
    {
        BtnBack.gameObject.SetActive(false);
        State = ApplicationState.Welcome;
        GoHome();
    }
}