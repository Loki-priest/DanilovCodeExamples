using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyIsland : MonoBehaviour
{
    public enum IslandStatus
    {
        Empty,
        Ally,
        Enemy,
        Choose,
        ToGo
    }

    public int index;
    public UILabel islandNum;

    public IslandStatus myStatus;
    public UI2DSprite sandColor;

    public UI2DSprite mySprite;
    public TweenAlpha myTween;

    [Header("Magnet")]
    public GameObject magnet;

    [Header("Allies")]
    public List<StrategyAlly> myAllies;
    public List<GameObject> allSlots;
    public List<GameObject> freeSlots;
    public List<GameObject> lockSlots;

    [Header("Enemies")]
    public List<StrategyEnemy> myEnemies;
    public List<GameObject> allEnemySlots;
    public List<GameObject> enemySlots;
    public List<GameObject> lockEnemySlots;

    Color tmp;
    public void SetStatus(IslandStatus _status)
    {
        if(_status == IslandStatus.Empty)
        {
            myTween.enabled = false;
            tmp = mySprite.color;
            tmp.a = 0.0f;
            mySprite.color = tmp;
        }

        if(myStatus != _status)
        {
            switch(_status)
            {
                case IslandStatus.Empty:
                    //
                    break;

                case IslandStatus.Ally:
                    myTween.enabled = false;
                    tmp.a = 1.0f;
                    tmp = Color.cyan;
                    mySprite.color = tmp;
                    break;

                case IslandStatus.Enemy:
                    myTween.enabled = false;
                    tmp.a = 1.0f;
                    tmp.r = 1.0f;
                    tmp.g = 0.5f;
                    tmp.b = 0.0f;
                    mySprite.color = tmp;
                    break;

                case IslandStatus.Choose:
                    myTween.enabled = false;
                    tmp.a = 1.0f;
                    //tmp = Color.yellow;
                    mySprite.color = tmp;
                    /*
                    myTween.ResetToBeginning();
                    myTween.PlayForward();
                    tmp.a = 1.0f;
                    tmp = Color.yellow;
                    mySprite.color = tmp;
                    */
                    break;

                case IslandStatus.ToGo:
                    tmp.a = 1.0f;
                    //tmp = Color.yellow;
                    mySprite.color = tmp;
                    myTween.mFactor = 0.0f;
                    myTween.mAmountPerDelta = 1.0f;
                    myTween.ResetToBeginning();
                    myTween.PlayForward();
                    break;

                default:
                    break;
            }

            myStatus = _status;
        }

        CheckMagnet();
    }


    void ChooseMe(bool isSound)
    {
        if (myStatus == IslandStatus.Choose) //сделать пустым
        {
            //ничего не надо тут делать, то, что ниже - временно
            SetStatus(IslandStatus.Empty);
            StrategyManager.Instance.SetStatus(StrategyManager.GameStatus.Free);
            StrategyManager.Instance.islandFrom = null;
            StrategyManager.Instance.currentLevel.MakeAllEmpty();
        }
        else
        if (myStatus == IslandStatus.Empty)
        {
            if (myAllies.Count == 0)
            {
                //Debug.Log("No Allies");
                //StrategyUtilities.Instance.SoundError();
                if (isSound) StrategySoundManager.Instance.soundNot.Play();
                return;//+звук ошибки?
            }
            else
            {
                for (int i = 0; i < myAllies.Count; i++)
                    myAllies[i].SetStatus(StrategyAlly.Status.Select);
            }

            /*
            if(StrategyManager.Instance.gameStatus == StrategyManager.GameStatus.ChooseFrom) //закомментить на случай, если мы выбираем перекрестно союзников через остров
            {
                Debug.Log("WTF tmp");
                return;
            }
            */

            StrategyManager.Instance.SetStatus(StrategyManager.GameStatus.ChooseFrom);
            //все остальные - пустыми?
            SelectNearIslands();

            //tmp
            //StrategyManager.Instance.MoveTo(this);
        }
        else
            if (myStatus == IslandStatus.ToGo)
        {
            StrategyManager.Instance.ChooseMovingType(this);
        }

        if (isSound) StrategySoundManager.Instance.soundIsland.Play();
        StrategyUtilities.Instance.Vibrate();
        
    }


    public void SelectNearIslands()
    {
        StrategyManager.Instance.currentLevel.MakeAllEmpty();
        //StrategyManager.Instance.currentLevel.ShowAllRoads(this, false);

        SetStatus(IslandStatus.Choose);
        StrategyManager.Instance.islandFrom = this;
        List<StrategyIsland> islands = StrategyManager.Instance.currentLevel.FindNearIslands(this);

        for (int i = 0; i < islands.Count; i++)
        {
            if (islands[i].myAllies.Count > 0 && islands[i].myAllies[0].myType != myAllies[0].myType)
            {
                //нельзя сюда ходить
            }
            else
            {
                islands[i].ChooseToGo();
                StrategyManager.Instance.currentLevel.ShowRoadConnect(this, islands[i]);
            }
        }
        //StrategyManager.Instance.currentLevel.ShowAllRoads(this, true);

    }


    public void ChooseToGo()
    {
        SetStatus(IslandStatus.ToGo);
    }

    public void MakeEmptyIfChosen()
    {
        if (myStatus != IslandStatus.Ally && myStatus != IslandStatus.Enemy)
        {
            SetStatus(IslandStatus.Empty);
        }
    }

    public void PublicOnClick()
    {
        OnClick();
    }

    public void SelectAgain()
    {
        //return;
        AfterClick(false);
    }

    void OnClick()
    {
        AfterClick(true);
    }

    void AfterClick(bool isSound)
    {
        if (StrategyManager.Instance.gameStatus != StrategyManager.GameStatus.Free && StrategyManager.Instance.gameStatus != StrategyManager.GameStatus.ChooseFrom) //не можем кликать, если двигаемся, деремся или проиграли-выиграли
        {
            //Debug.Log("Cannot Choose Yet!");
            //StrategyUtilities.Instance.SoundError();
            if (isSound) StrategySoundManager.Instance.soundNot.Play();
            return; //+звук ошибки
        }

        //Debug.Log("Click");
        if (myStatus != IslandStatus.Ally && myStatus != IslandStatus.Enemy)
        {
            //StrategySoundManager.Instance.soundIsland.Play();
            //StrategyUtilities.Instance.Vibrate();
            ChooseMe(isSound);
        }
    }


    public void MoveAllEnemies(StrategyIsland islandTo)
    {
        //освободить все слоты этого острова
        for (int i = 0; i < lockEnemySlots.Count; i++)
        {
            enemySlots.Add(lockEnemySlots[i]);
        }
        lockEnemySlots.Clear();

        for (int i = 0; i < myEnemies.Count; i++)
        {
            //if (myEnemies[i].myStatus != StrategyEnemy.Status.Stun)
            //{
                myEnemies[i].SetWay(islandTo);
                myEnemies[i].SetStatus(StrategyEnemy.Status.Move);
            //}
        }
    }

    public void MoveAllPeople()
    {
        //освободить все слоты этого острова
        for (int i = 0; i < lockSlots.Count; i++)
        {
            freeSlots.Add(lockSlots[i]);
        }
        lockSlots.Clear();

        for (int i = 0; i < myAllies.Count; i++)
        {
            myAllies[i].SetWay();
            myAllies[i].SetStatus(StrategyAlly.Status.Move);
        }
    }


    //событие зажатия
    /*
    void OnPress()
    {

    }

    void Update()
    {

    }
    */

    Vector3 tmpMagnetPos;
    public void CheckMagnet()
    {
        if (
            StrategyManager.Instance.currentLevel.myCfg.levelType == LevelConfig.LevelType.Desert && 
            gameObject.activeInHierarchy &&
            (
            StrategyManager.Instance.gameStatus == StrategyManager.GameStatus.Free
            ||
            StrategyManager.Instance.gameStatus == StrategyManager.GameStatus.ChooseFrom //&&
            //myStatus == IslandStatus.Choose
            ) &&
            myAllies.Count > 0 &&
            myAllies.Count % 2 == 0 &&
            myAllies[0].myType == LevelConfig.MobType.Warrior)
        {
            magnet.transform.parent = transform;
            magnet.transform.localPosition = tmpMagnetPos;
            magnet.transform.parent = StrategyManager.Instance.magnetsContainer.transform;
            magnet.SetActive(true);
        }
        else
        {
            magnet.SetActive(false);
            //magnet.transform.parent = transform;
            //magnet.transform.localPosition = tmpMagnetPos;
        }
    }



    public void UniteWarriors()
    {
        int alliesCount = myAllies.Count;
        StrategyManager.Instance.levelConstructor.RefreshSlotsForIsland(this);
        StrategyManager.Instance.levelConstructor.MakeAllies(StrategyManager.Instance.currentLevel, alliesCount / 2, index, LevelConfig.MobType.Archer);

        StrategyUtilities.Instance.Vibrate();
        StrategySoundManager.Instance.soundUnite.Play();
        //StrategyUtilities.Instance.effectsManager.CallUnite(transform);

        //restart ToGo подсветка
        /*
        if (myStatus == IslandStatus.Choose)
        {
            SelectNearIslands();
        }*/

        /*
        if (StrategyManager.Instance.gameStatus == StrategyManager.GameStatus.ChooseFrom)
        {
            if (StrategyManager.Instance.islandFrom)
                if (StrategyManager.Instance.islandFrom.myStatus == IslandStatus.Choose)
                    StrategyManager.Instance.islandFrom.SelectNearIslands();
        }
        */
        StrategyUtilities.Instance.effectsManager.EndMagnit(true);

        MagnetReact();

        //StrategyManager.Instance.currentLevel.MakeAllEmpty();
        //PublicOnClick();//StrategyManager.Instance.SetStatus(StrategyManager.GameStatus.ChooseFrom);

        //StrategyManager.Instance.islandFrom.SelectNearIslands();
    }

    public void MagnetReact()
    {
        StrategyManager.Instance.currentLevel.MakeAllEmpty();
        PublicOnClick();//StrategyManager.Instance.SetStatus(StrategyManager.GameStatus.ChooseFrom);
    }



    void Awake()
    {
        tmpMagnetPos = new Vector3(43, -30, 0);//new Vector3(0, 90, 0);

        allSlots.AddRange(freeSlots);
        allEnemySlots.AddRange(enemySlots);
    }

    public void Init()
    {
        Start();
    }

    void Start()
    {
        SetStatus(IslandStatus.Empty);

        /*
        if (myAllies.Count > 0)
            SetStatus(IslandStatus.Ally);
        if (myEnemies.Count > 0)
            SetStatus(IslandStatus.Enemy);
          */  
    }

}
