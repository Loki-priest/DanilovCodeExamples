using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyTutorial : MonoBehaviour
{
    public static StrategyTutorial Instance;

    public bool isTutorial;
    public int currentScreen = 0;
    public GameObject bgFader;
    public GameObject block;
    public GameObject tutorialStarter;
    public GameObject[] screens;

    private void Awake()
    {
        Instance = this;
    }

    public void ActivateMe()
    {
        Start();
    }

    private void Start()
    {
        isTutorial = PlayerPrefs.GetInt("IsTutorial", 1) > 0;
        StartTutorial();//сразу 1ое окно //tutorialStarter.SetActive(isTutorial);
        PlayerPrefs.SetInt("IsTutorial", 0);
    }

    public void StartTutorial()
    {
        tutorialStarter.SetActive(false);
        if (isTutorial)
        {
            currentScreen = 0;
            bgFader.SetActive(true);
            screens[currentScreen].SetActive(true);
        }
        else
        {
            block.SetActive(false);
        }
    }


    public void CloseCurrent()
    {
        bgFader.SetActive(false);
        screens[currentScreen].SetActive(false);
    }

    IEnumerator DelayedScreen(float delay)
    {
        yield return new WaitForSeconds(delay);
        bgFader.SetActive(true);
        screens[currentScreen].SetActive(true);
    }


    public void OpenScreen(int index, bool isDelay = false, float delay = 2.15f)
    {
        CloseCurrent();
        if (isTutorial)
        {
            currentScreen = index;
            if (isDelay)
            {
                StartCoroutine(DelayedScreen(delay));
            }
            else
            {
                bgFader.SetActive(true);
                screens[currentScreen].SetActive(true);
            }
        }
    }

    public void NextScreen(bool isDelay = false)
    {
        CloseCurrent();
        if (isTutorial)
        {
            currentScreen++;
            if (currentScreen >= screens.Length)
            {
                EndTutorial();
            }
            else
            {
                if (isDelay)//с задержкой
                {
                    StartCoroutine(DelayedScreen(2.15f));
                }
                else
                {
                    bgFader.SetActive(true);
                    screens[currentScreen].SetActive(true);
                }
            }
        }
    }

    public void EndTutorial()
    {
        bgFader.SetActive(false);
        block.SetActive(false);
        isTutorial = false;
        PlayerPrefs.SetInt("IsTutorial", 0);
    }


}
