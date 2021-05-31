using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollsClothManager : MonoBehaviour {

    public enum Gender
    {
        Uni,
        Male,
        Female
    }


    public bool isPlayer = false;




    [System.Serializable]
    public class ClothCategory
    {
        public string name;
        public ClothType clothType;
        public MagicClothInfo[] magicClothInfos;
        public int currentIndex = -1;

        public void UnDressCategory(DollsClothManager Instance)
        {
            if (Instance.currentSlot[(int)clothType])
            {
                Instance.currentSlot[(int)clothType].UndressMe();
                Instance.currentSlot[(int)clothType] = null;
                Instance.ApplyChanges(clothType, -1);
            }

        }

        public void MakeShopInfo(DollsClothManager Instance)
        {
            //тут будет баг, что no мешается, осторожно с индексами, или вообще его отдельно
            //
            Debug.Log("ChooseClothCategory " + clothType);
            int i = 0;
            //Instance.shopClothNo.type = clothType;
            HeroShopThing sci;
            MagicShopManager msm = MagicShopManager.Instance;
            MagicClothInfo mci;

            if (magicClothInfos.Length > msm.shopClothItems.Length)
            {
                Debug.Log("WARNING");
            }

            for (i = 0; i < msm.shopClothItems.Length && i < magicClothInfos.Length; i++)//еще сравнить с количеством шмоток в категории
            {
                mci = magicClothInfos[i];
                sci = msm.shopClothItems[i];
                sci.gameObject.SetActive(true); if (Instance.isMale && mci.gender == Gender.Female || !Instance.isMale && mci.gender == Gender.Male) sci.gameObject.SetActive(false);
                sci.index = i;
                sci.what = clothType;
                sci.genderIcon.sprite2D = msm.genderIcons[(int)mci.gender];
                sci.itemIcon.sprite2D = mci.dollsItem.icon;

                //заполнить локайтемы
                sci.myLockItem.lockType = mci.dollsItem.typeLock;
                //sci.lockItem.itemIndex = clothInfos[i].dollsItem.itemIndex;
                sci.myLockItem.priceCoins = mci.dollsItem.price;
                sci.myLockItem.priceAds = mci.dollsItem.price;
                //sci.lockItem.startCountAds = clothInfos[i].dollsItem.price;

                sci.isUnlocked = false;
                sci.analythicName = mci.dollsItem.analythicName;

                sci.myLockItem.UpdateUI();
                sci.ResetStart();
            }
            //скрыть остальные слоты в магазине
            for (i = magicClothInfos.Length; i < msm.shopClothItems.Length; i++)
            {
                msm.shopClothItems[i].gameObject.SetActive(false);
            }

            //обновить грид
            msm.clothGrid.Reposition();
            msm.clothScroll.ResetPosition();

            //select
            SetSelectTransform();
        }

        public void SetSelectTransform()
        {
            int s = PlayerPrefs.GetInt("ClothOn_" + clothType, -1);
            MagicShopManager msm = MagicShopManager.Instance;
            if (msm == null)
                return;
            if (s < 0)
            {
                msm.shopClothSelector.SetActive(false);
                msm.shopClothSelector.transform.SetParent(msm.shopClothItems[0].selectedPlacer);
                msm.shopClothSelector.transform.localPosition = Vector3.zero;
            }
            else
            {
                msm.shopClothSelector.SetActive(true);
                msm.shopClothSelector.transform.SetParent(msm.shopClothItems[s].selectedPlacer);
                msm.shopClothSelector.transform.localPosition = Vector3.zero;
            }
        }

    }

    public bool isMale = true;

    public ClothCategory[] clothCategories;
    public DollCloth[] currentSlot;

    bool newGame = true;




    public void ForNewGame()
    {
        if (newGame)
        {
            //надеть бесплатное
            PlayerPrefs.SetInt("ClothOn_" + clothCategories[0].clothType, 0); //одежда
            PlayerPrefs.SetInt("ClothOn_" + clothCategories[1].clothType, 0); //волосы
            PlayerPrefs.SetInt("ClothOn_" + clothCategories[2].clothType, 0); //аксес
            PlayerPrefs.SetInt("ClothOn_" + clothCategories[4].clothType, 0); //палка

            PlayerPrefs.SetInt("NewGame", 0);
            newGame = false;

            //RewardedVideoController.Instance.AddCurrency(50000);//УДАЛИТЬ на релизе
        }

    }

    private void Awake()
    {
        //загрузить из префсов: надетая одежда, выбранный цвет этой одежды
        newGame = PlayerPrefs.GetInt("NewGame", 1) == 1;
    }


    private void Start()
    {
        //return;
        if (isPlayer)
        {
            isMale = PlayerPrefs.GetInt("Gender", 1) == 1;

            MagicShopManager msm = MagicShopManager.Instance;
            if (msm)
            {
                msm.selectorGirl.SetActive(!isMale);
                msm.selectorBoy.SetActive(isMale);
                msm.adsGirl.SetActive(isMale);
                msm.adsBoy.SetActive(!isMale);
            }

            ChangeSkin(PlayerPrefs.GetInt("SkinColor", 0));

            ForNewGame();

            int index;
            for (int i = 0; i < clothCategories.Length; i++)
            {
                index = PlayerPrefs.GetInt("ClothOn_" + clothCategories[i].clothType, -1);
                if (index > -1)
                {
                    clothCategories[i].magicClothInfos[index].TryToDress(index, this);
                }
            }
        }
        else
        {
            Randomize();//временно
        }

        if (MagicSpellsManager.Instance) //если мы в игре
        {
            if (MagicFatalityManager.Instance.fatalityNum != 3)
            {
                Destroy(gameObject, 1.0f);
            }
        }
    }



    public void ApplyChanges(ClothType type, int index)
    {
        //сделать временный выбор куррентом
        //записать в префсы
        if (isPlayer)
        {
            PlayerPrefs.SetInt("ClothOn_" + type, index);
            clothCategories[(int)type].SetSelectTransform();
        }
        clothCategories[(int)type].currentIndex = index;

    }



    public void ChangeGender(int val)
    {
        //0 - ж, 1 - м
        UnDressAll();

        if (val == 0)
        {
            isMale = false;
            if (isPlayer)
            {
                PlayerPrefs.SetInt("ClothOn_" + clothCategories[0].clothType, 11); //одежда
                PlayerPrefs.SetInt("ClothOn_" + clothCategories[1].clothType, 25); //волосы
                PlayerPrefs.SetInt("ClothOn_" + clothCategories[2].clothType, 0); //аксес
            }
        }
        else
        {
            isMale = true;
            if (isPlayer)
            {
                PlayerPrefs.SetInt("ClothOn_" + clothCategories[0].clothType, 0); //одежда
                PlayerPrefs.SetInt("ClothOn_" + clothCategories[1].clothType, 0); //волосы
                PlayerPrefs.SetInt("ClothOn_" + clothCategories[2].clothType, 0); //аксес
            }
        }

        if (isPlayer) PlayerPrefs.SetInt("Gender", val);

        if (isPlayer)
            Start();
    }


    public Material skinMaterial;
    Material newSkinMaterial;
    SkinnedMeshRenderer tmpSmr;
    Material[] tmpMaterials;

    public Texture[] skinTextures;

    public SkinnedMeshRenderer[] smrsWithSkin;

    public void ChangeSkin(int val)
    {
        if (isPlayer)
        {
            skinMaterial.mainTexture = skinTextures[val];

            MagicShopManager msm = MagicShopManager.Instance;
            if (msm)
            {
                msm.shopSkinSelector.transform.SetParent(msm.skinTransforms[val]);
                msm.shopSkinSelector.transform.localPosition = Vector3.zero;
            }

            PlayerPrefs.SetInt("SkinColor", val);
        }
        else
        {
            if (newSkinMaterial == null)
                newSkinMaterial = Instantiate(skinMaterial);

            newSkinMaterial.mainTexture = skinTextures[val];

            //вешаем наш материал на все скинеды и меш рендеры, если совпало с оригинальным материалом игрока
            DollsClothManager pl;
            pl = MagicPlayerContainer.Instance.playerVardrobe;

            for (int i = 0; i < smrsWithSkin.Length; i++)
            {
                tmpSmr = smrsWithSkin[i];
                tmpMaterials = tmpSmr.sharedMaterials;
                
                for (int j = 0; j < tmpMaterials.Length; j++)
                {
                    if (tmpMaterials[j] == pl.skinMaterial)
                    {
                        tmpMaterials[j] = newSkinMaterial;
                    }
                    tmpSmr.materials = tmpMaterials;
                }
            }
        }
    }





    public void UnDressAll()//обычно после выбора пола
    {

        for (int i = 0; i < currentSlot.Length; i++)
        {
            if (i < 3) //трогаем только одежду, прически и аксесы
            {
                if (currentSlot[i])
                {
                    currentSlot[i].UndressMe();
                    currentSlot[i] = null;
                }
                ApplyChanges(clothCategories[i].clothType, -1);
            }
            //обнулить цвет?
        }

        //записать все это в префсы, т.е. по нулям выбранную одежду
    }


    public void Randomize()
    {
        int index;

        index = Random.Range(0, 2);
        ChangeGender(index);

        index = Random.Range(0, skinTextures.Length);
        ChangeSkin(index); //в конце разработки

        //одежда 0 - когда еще добавится - сделать временный массив или список из м или ж шмоток, рандомить из них
        if (isMale)
            index = Random.Range(0, 11); //11шт
        else
            index = Random.Range(11, 21); //10шт
        clothCategories[(int)ClothType.Clothes].magicClothInfos[index].TryToDress(index, this); //переписать под поиск в цикле подходящей под пол одежды

        //волосы 1 - когда еще добавится - сделать временный массив или список из м или ж шмоток, рандомить из них
        if (isMale)
            index = Random.Range(0, 25); //25шт
        else
            index = Random.Range(25, 50); //25шт
        clothCategories[(int)ClothType.Hair].magicClothInfos[index].TryToDress(index, this); //переписать под поиск в цикле подходящих под пол волос

        //аксессы 2
        index = Random.Range(0, clothCategories[(int)ClothType.Acces].magicClothInfos.Length);
        clothCategories[(int)ClothType.Acces].magicClothInfos[index].TryToDress(index, this);

        //пет 3
        //

        //палка 4
        index = Random.Range(0, clothCategories[(int)ClothType.Wand].magicClothInfos.Length);
        clothCategories[(int)ClothType.Wand].magicClothInfos[index].TryToDress(index, this);

    }



    public SkinnedMeshRenderer head;
    public void ChangeBroves(Material mat)
    {
        tmpMaterials = head.sharedMaterials;
        tmpMaterials[1] = mat;
        head.materials = tmpMaterials;

        currentHairMat = mat;

        //аксесс снять-одеть, так надо для смены цвета волос под шапкой
        if (currentSlot[2] != null)
        {
            currentSlot[2].UndressMe();
            currentSlot[2].DressMe();
        }
    }



    public Material currentHairMat;



}
