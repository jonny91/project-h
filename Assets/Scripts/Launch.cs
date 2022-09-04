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
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Launch : SerializedMonoBehaviour
{
    private ApplicationState State;

    public float MaxScaleSize = 2.0f;

    [SerializeField]
    private Menu MenuGorup;

    /// <summary>
    /// 当前显示的对象
    /// </summary>
    [SerializeField]
    private GameObject _currentShowModel;

    [SerializeField]
    private Transform ShowPos;

    [SerializeField]
    private Animator LogoAnimator;

    [SerializeField]
    private Button BtnBack;

    [SerializeField]
    private GameObject[] HomeModelGroup;

    [SerializeField]
    private Transform[] HomePosArr;

    public Dictionary<string, GameObject> ModelDic;

    private float ShowTime = 0.2f;

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
                HideHomeModel(clickedObject);

                CurrentSelectedClassify = modelData.Classify;
                MenuGorup.SetClassify(CurrentSelectedClassify);
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
                    HideHomeModel(clickedObject);
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
            _currentShowModel.transform.localScale = Vector3.one *
                                                     Math.Max(Math.Min(
                                                         _currentShowModel.transform.localScale.x + scaleValue,
                                                         MaxScaleSize), 1f);
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


    private void HideHomeModel(GameObject clickedObject)
    {
        for (int i = 0; i < HomeModelGroup.Length; i++)
        {
            var m = HomeModelGroup[i];
            //不点击的这个 依次去菜单中
            if (m != clickedObject)
            {
                m.transform.localScale = Vector3.zero;
                m.GetComponent<Animator>().SetBool("show", false);
                m.GetComponent<Animator>().SetTrigger("idle");
            }
        }
    }

    private void ModelBackToHome()
    {
        if (_currentShowModel != null)
        {
            _currentShowModel.SetActive(false);
            _currentShowModel.GetComponent<Animator>().SetBool("show", false);
        }

        for (int i = 0; i < HomeModelGroup.Length; i++)
        {
            var m = HomeModelGroup[i];
            var modelIndex = m.GetComponent<HomeModelIndex>();
            var targetPosTrans = HomePosArr[modelIndex.Index];
            m.SetActive(true);
            m.transform.SetParent(targetPosTrans);
            m.transform.localPosition = Vector3.zero;
            m.transform.DOScale(Vector3.one, ShowTime);
            m.transform.localScale = Vector3.one;
            m.transform.localRotation = Quaternion.identity;
            m.GetComponent<Animator>().SetBool("show", true);
        }

        CurrentSelectedClassify = -1;
        _currentShowModel = null;
    }

    private bool isPlaying = false;

    public void SelectClassify(int classify)
    {
        if (CurrentSelectedClassify == classify || isPlaying)
        {
            return;
        }

        isPlaying = true;

        var newModel = ModelDic[classify + "_0"];
        SwitchModel(newModel, _currentShowModel, () =>
        {
            CurrentSelectedClassify = classify;
            _currentShowModel = newModel;
            isPlaying = false;
        });
    }

    public void SelectStyle(int style)
    {
        var newModel = ModelDic[CurrentSelectedClassify + "_" + style];
        if (newModel == _currentShowModel || isPlaying)
        {
            return;
        }

        isPlaying = true;
        SwitchModel(newModel, _currentShowModel, () =>
        {
            _currentShowModel = newModel;
            isPlaying = false;
        });
    }

    private void SwitchModel(GameObject n, GameObject o, Action cb)
    {
        n.SetActive(true);
        n.transform.parent = ShowPos;
        n.transform.localPosition = Vector3.zero;
        n.transform.DOScale(Vector3.one, ShowTime);

        o.transform.parent = null;
        o.transform.DOScale(Vector3.zero, ShowTime).OnComplete(() =>
        {
            n.GetComponent<Animator>().SetBool("show", true);
            o.GetComponent<Animator>().SetBool("show", false);

            cb?.Invoke();
        });
    }

    public void SetMenuVisible(bool v)
    {
        MenuGorup.gameObject.SetActive(v);
    }

    public void Start()
    {
        SetMenuVisible(false);
        BtnBack.onClick.AddListener(Back);
        GoHome();
    }

    public void StartShow()
    {
        SetMenuVisible(true);
        State = ApplicationState.Show;
        LogoAnimator.SetTrigger("show");
    }

    private void GoHome()
    {
        Debug.Log("返回主界面");
        SetMenuVisible(false);
        LogoAnimator.SetTrigger("home");
        ModelBackToHome();
        _currentShowModel = null;
    }

    public void Back()
    {
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