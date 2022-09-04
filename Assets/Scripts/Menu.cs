using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Menu : SerializedMonoBehaviour
{
    public Dictionary<int, Dictionary<int, Sprite>> SpriteDic;

    public GameObject Classfy0;
    public GameObject Classfy1;
    public GameObject Classfy2;
    public GameObject Classfy3;

    public GameObject Style0;
    public GameObject Style1;
    public GameObject Style2;

    public void SetClassify(int classify)
    {
        Classfy0.SetActive(true);
        Classfy1.SetActive(true);
        Classfy2.SetActive(true);
        Classfy3.SetActive(true);
        Style0.SetActive(true);
        Style1.SetActive(true);
        Style2.SetActive(true);

        if (SpriteDic[classify].ContainsKey(0))
        {
            Style0.GetComponent<Image>().sprite = SpriteDic[classify][0];
        }

        if (SpriteDic[classify].ContainsKey(1))
        {
            Style1.GetComponent<Image>().sprite = SpriteDic[classify][1];
        }

        if (SpriteDic[classify].ContainsKey(2))
        {
            Style2.GetComponent<Image>().sprite = SpriteDic[classify][2];
        }

        switch (classify)
        {
            case 0:
                Classfy0.SetActive(false);
                Style2.SetActive(false);
                break;
            case 1:
                Classfy1.SetActive(false);
                break;
            case 2:
                Classfy2.SetActive(false);
                break;
            case 3:
                Classfy3.SetActive(false);
                break;
            default:
                break;
        }
    }
}