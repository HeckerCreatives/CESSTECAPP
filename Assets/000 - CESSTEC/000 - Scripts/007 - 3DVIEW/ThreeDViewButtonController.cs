using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDViewButtonController : MonoBehaviour
{
    [SerializeField] private GameObject exteriorBtnObj;
    [SerializeField] private GameObject interiorBtnObj;
    [SerializeField] private bool exterior;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private CinemachineFreeLook freeVcam;

    public void ChangeView()
    {
        if (exterior)
        {
            freeVcam.m_Priority = 10;
            vcam.m_Priority = 9;
            interiorBtnObj.SetActive(true);
            exteriorBtnObj.SetActive(false);
        }
        else
        {
            vcam.m_Priority = 10;
            freeVcam.m_Priority = 9;
            exteriorBtnObj.SetActive(true);
            interiorBtnObj.SetActive(false);
        }
    }
}
