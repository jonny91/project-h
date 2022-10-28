/*************************************************************************************
 *
 * 文 件 名:   Menu.cs
 * 描    述: 
 * 
 * 创 建 者：  洪金敏 
 * 创建时间：  2022-10-29 01:36:43
*************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Menu : SerializedMonoBehaviour
{
    [LabelText("外包装按钮")]
    public Button BtnStyle0;

    [LabelText("内包装按钮")]
    public Button BtnStyle1;

    public Launch Launch;

    public void SetClassify(int currentSelectedClassify)
    {
        BtnStyle0.gameObject.SetActive(
            Launch.ModelDic[currentSelectedClassify + "_" + BtnStyle0.gameObject.name] != null);
        BtnStyle1.gameObject.SetActive(
            Launch.ModelDic[currentSelectedClassify + "_" + BtnStyle1.gameObject.name] != null);
    }
}