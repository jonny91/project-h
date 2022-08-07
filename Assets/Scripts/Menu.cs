using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
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
        switch (classify)
        {
            case 0:
                Classfy0.SetActive(false);
                Style1.SetActive(false);
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