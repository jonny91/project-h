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

    /// <summary>
    /// 当前显示的对象
    /// </summary>
    [SerializeField]
    private GameObject _currentShowModel;

    [SerializeField]
    private Transform ShowPos;

    [SerializeField]
    private Transform ShowPosLeft;

    [SerializeField]
    private Transform ShowPosRight;

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

    [SerializeField]
    private ModelData[] ShowList0;

    [SerializeField]
    private ModelData[] ShowList1;

    [SerializeField]
    private ModelData[] ShowList2;

    [SerializeField]
    private ModelData[] ShowList3;

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

    private int CurrentSelectedClassify = -1;

    private void OnClickModel(GameObject clickedObject)
    {
        if (State == ApplicationState.Welcome)
        {
            var modelData = clickedObject.GetComponent<ModelData>();
            if (modelData)
            {
                if (CurrentSelectedClassify == modelData.Classify)
                {
                    return;
                }

                StartShow();
                MoveToCenter(clickedObject);
                MoveToMenu(clickedObject);
                CreateRollList(modelData);

                CurrentSelectedClassify = modelData.Classify;
            }
        }
        else if (State == ApplicationState.Show)
        {
            var modelData = clickedObject.GetComponent<ModelData>();
            if (modelData)
            {
                if (CurrentSelectedClassify != modelData.Classify)
                {
                    MoveToCenter(clickedObject);
                    MoveToMenu(clickedObject);
                    CreateRollList(modelData);
                }
                else
                {
                    Roll(modelData);
                    return;
                }

                CurrentSelectedClassify = modelData.Classify;
            }
        }
    }

    private void Roll(ModelData modelData)
    {
        var originParent = modelData.transform.parent;
        var middle = _currentShowModel.transform.parent;

        _currentShowModel.transform.SetParent(originParent);
        modelData.transform.SetParent(middle);

        modelData.transform.DOLocalMove(Vector3.zero, ShowTime);
        _currentShowModel.transform.DOLocalMove(Vector3.zero, ShowTime);

        _currentShowModel.GetComponent<Animator>().SetBool("show", false);

        _currentShowModel = modelData.gameObject;

        _currentShowModel.GetComponent<Animator>().SetBool("show", true);
    }

    public void HandleRotate(float rotateX, float rotateY)
    {
        if (_currentShowModel != null)
        {
            _currentShowModel.transform.Rotate(rotateY, rotateX, 0);
        }
    }

    public void HandleScale(float scaleValue)
    {
        if (_currentShowModel != null)
        {
            _currentShowModel.transform.localScale += Vector3.one * scaleValue;
        }
    }

    /// <summary>
    /// 要展示的物品到屏幕中间
    /// </summary>
    /// <param name="clickedObject"></param>
    private void MoveToCenter(GameObject clickedObject)
    {
        for (int i = 0; i < ShowPos.childCount; i++)
        {
            var c = ShowPos.GetChild(i);
            //不是第一个展示的模型
            if (c.GetComponent<ModelData>().Index != 0)
            {
                c.transform.SetParent(null);
                c.transform.position = new Vector3(-100, 0, 0);
            }
        }

        clickedObject.transform.parent = ShowPos;
        clickedObject.transform.DOLocalMove(Vector3.zero, ShowTime);
        clickedObject.transform.localScale = Vector3.one;
        clickedObject.GetComponent<Animator>().SetBool("show", true);

        _currentShowModel = clickedObject;
    }

    private void CreateRollList(ModelData modelData)
    {
        var modelDataClassify = modelData.Classify;
        var modelIndex = modelData.Index;
        ModelData[] targetList = null;
        if (modelDataClassify == 0)
        {
            targetList = ShowList0;
        }
        else if (modelDataClassify == 1)
        {
            targetList = ShowList1;
        }
        else if (modelDataClassify == 2)
        {
            targetList = ShowList2;
        }
        else if (modelDataClassify == 3)
        {
            targetList = ShowList3;
        }

        CreateLeft(targetList);
        CreateRight(targetList);
    }

    private void CreateLeft(ModelData[] targetList)
    {
        if (targetList.Length == 3)
        {
            for (int i = 0; i < ShowPosLeft.childCount; i++)
            {
                var c = ShowPosLeft.GetChild(i);
                c.transform.SetParent(null);
                c.transform.position = new Vector3(-100, 0, 0);
            }

            var model = targetList[2];
            model.transform.SetParent(ShowPosLeft, true);
            model.transform.localPosition = Vector3.zero;
            model.GetComponent<Animator>().SetBool("show", false);
            ShowPosLeft.gameObject.SetActive(true);
        }
        else
        {
            ShowPosLeft.gameObject.SetActive(false);
        }
    }

    private void CreateRight(ModelData[] targetList)
    {
        if (targetList.Length >= 2)
        {
            for (int i = 0; i < ShowPosRight.childCount; i++)
            {
                var c = ShowPosRight.GetChild(i);
                c.transform.SetParent(null);
                c.transform.position = new Vector3(-100, 0, 0);
            }

            var model = targetList[1];
            model.transform.SetParent(ShowPosRight, true);
            model.GetComponent<Animator>().SetBool("show", false);
            model.transform.localPosition = Vector3.zero;
            ShowPosRight.gameObject.SetActive(true);
        }
        else
        {
            ShowPosRight.gameObject.SetActive(false);
        }
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
                m.transform.localScale = Vector3.one;
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
            m.transform.localScale = Vector3.one;
            m.GetComponent<Animator>().SetBool("show", true);
        }

        CurrentSelectedClassify = -1;
        _currentShowModel = null;
        ShowPosLeft.gameObject.SetActive(false);
        ShowPosRight.gameObject.SetActive(false);
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

    public void HandlePan(DigitalRubyShared.GestureRecognizer gesture, GestureTouch t)
    {
        Debug.LogFormat("Pan gesture, state: {0}, position: {1},{2} -> {3},{4}",
            gesture.State, t.PreviousX,
            t.PreviousY, t.X, t.Y);

        if (gesture.State == GestureRecognizerState.Began)
        {
            _currentShowModel.GetComponent<ModelData>().StopAutoRotate = true;
        }
        else if (gesture.State == GestureRecognizerState.Executing)
        {
            var deltaX = t.DeltaX;
            var deltaY = t.DeltaY;
            HandleRotate(-deltaX, deltaY);
        }
        else if (gesture.State == GestureRecognizerState.Ended)
        {
            _currentShowModel.GetComponent<ModelData>().StopAutoRotate = false;
        }
    }
}