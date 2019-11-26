﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Michsky.UI.ModernUIPack;
using System;
using OuterRimStudios.Utilities;

public class ApplicationManager : MonoBehaviour
{
    [Header("Start Page")]
    public GameObject startPage;
    public TMP_InputField testerCountField;

    [Space, Header("Quest Page")]
    public GameObject questPage;
    public TextMeshProUGUI questIDField;
    public TMP_InputField userIDField;
    public SwitchManager groupIDField;

    [Space, Header("End Page")]
    public GameObject endPage;
    public TextMeshProUGUI warning;

    const string MISSING_ID = "One or more userIDs were left empty!";
    const string DUPLICATE_ID = "One or more users have duplicate userIDs!";

    List<PageInfo> pageInfos = new List<PageInfo>();
    int pageIndex;

    public void SetNumberOfTesters()
    {
        if (int.TryParse(testerCountField.text, out int numberOfTester))
        {
            if (numberOfTester <= 0) return;

            if(numberOfTester < pageInfos.Count)
            {
                int count = pageInfos.Count - numberOfTester;

                for (int i = 0; i < count; i++)
                    pageInfos.RemoveAt(pageInfos.Count - 1);
            }
            for (int i = pageInfos.Count; i < numberOfTester; i++)
            {
                pageInfos.Add(new PageInfo());
                pageInfos[i].questID = "Oculus Quest " + (i + 1);
            }

            Next();
        }
    }

    private void Update()
    {
        print(pageIndex);
        print(pageInfos.Count);
    }

    public void Next()
    {
        SaveFields();
        pageIndex = pageIndex.IncrementClamped(pageInfos.Count + 1);
        SetUpPage();
    }

    public void Previous()
    {
        SaveFields();
        pageIndex = pageIndex.DecrementClamped();
        SetUpPage();
    }

    void SaveFields()
    {
        if(pageIndex != 0 && pageIndex != pageInfos.Count + 1)
        {
            pageInfos[pageIndex - 1].userID = userIDField.text;
            pageInfos[pageIndex - 1].groupID = groupIDField.isOn ? "A" : "B";
        }
    }

    void SetUpPage()
    {
        startPage.SetActive(pageIndex == 0);
        endPage.SetActive(pageIndex == pageInfos.Count + 1);

        if (!startPage.activeInHierarchy && !endPage.activeInHierarchy)
        {
            questPage.SetActive(true);

            questIDField.text = pageInfos[pageIndex - 1].questID;
            userIDField.text = pageInfos[pageIndex - 1].userID == "" ? "" : pageInfos[pageIndex - 1].userID;
            groupIDField.SetSwitch(pageInfos[pageIndex - 1].groupID != "B");
        }
        else
            questPage.SetActive(false);

        if(endPage.activeInHierarchy)
        {
            foreach(PageInfo pageInfo in pageInfos)
            {
                if (pageInfo.userID == "")
                {
                    warning.text = MISSING_ID;
                    warning.gameObject.SetActive(true);
                    break;
                }

                foreach(PageInfo _pageInfo in pageInfos)
                {
                    if (pageInfo != _pageInfo && pageInfo.userID == _pageInfo.userID)
                    {
                        warning.text = DUPLICATE_ID;
                        warning.gameObject.SetActive(true);
                    }
                }
            }
        }
        else
            warning.gameObject.SetActive(false);
    }

    public void BeginExperience()
    {
        pageInfos.Clear();
        Debug.Log("Start");
    }
}

class PageInfo
{
    public string questID;
    public string userID;
    public string groupID;
}