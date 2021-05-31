using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    int currentSkinGroup = 0;
    public GameObject[] skinGroups;
    public GameObject[] dots;

    public GameObject activeSkinSelector;

    public ShopItem[] allSkins;
    public Sprite[] iosPortraits;

    [Header("Tab1")]
    public GameObject btnBuy;
    public GameObject btnGetIt;
    public UILabel priceLabel;
    public GameObject block;

    [Header("Categories")]
    public int categoryPrice = 100;
    public int[] categoryPrices;
    public int categoriesBought = 0;
    public GameObject buySelector;
    public GameObject[] categories;
    public ShopItem[] categoryItems;
    public List<ShopItem> categoriesBoughtList;
    public List<GameObject> categoriesLeft;

    private void Awake()
    {
        Instance = this;
    }

    ShopItem ci;
    private void Start()
    {
        categoriesBought = 0;

        categoriesLeft = new List<GameObject>(categories);
        for (int i = 0; i < categories.Length; i++)
        {
            categoryItems[i] = categories[i].GetComponent<ShopItem>();
        }

        categoriesBoughtList = new List<ShopItem>(categoryItems);

        for (int i = 0; i < allSkins.Length; i++)
        {
            allSkins[i].Init(i);
#if (UNITY_IOS || UNITY_IPHONE)
            allSkins[i].icon.sprite2D = iosPortraits[i];
#endif
        }

        for (int i = categoriesLeft.Count - 1; i >= 0; i--)
        {
            ci = categoriesLeft[i].GetComponent<ShopItem>();
            ci.Init(i);
            if (ci.isBought)
            {
                categoriesLeft.Remove(categoriesLeft[i]);
                categoriesBought++;
            }
        }

        categoriesLeft.Shuffle();

        for (int i = categoriesBoughtList.Count - 1; i >= 0; i--)
        {
            if (!categoryItems[i].isBought)
                categoriesBoughtList.Remove(categoriesBoughtList[i]);
        }
        //categoriesBoughtList.Shuffle();

        UpdatePrice();

    }

    void UpdatePrice()
    {
        if (categoriesLeft.Count == 0)
        {
            btnBuy.SetActive(false);
            btnGetIt.SetActive(false);
        }
        //categoryPrice = Mathf.RoundToInt(100.0f * Mathf.Pow(1.6f, categoriesBought - 1));
        categoryPrice = categoryPrices[categoriesBought];
        priceLabel.text = categoryPrice.ToString();
    }

    public void TransformSelection(int i)
    {
        if (i < allSkins.Length)
        {
            activeSkinSelector.transform.SetParent(allSkins[i].transform);
            activeSkinSelector.transform.localPosition = Vector3.zero;
        }
    }

    IEnumerator GoRandom()
    {
        yield return null;
        //отрубить кнопку назад и заблочить купить
        buySelector.SetActive(true); //перенести в цикл?
        block.SetActive(true);

        int j = 0;
        for (int i = 0; i < 2 * categoriesLeft.Count && i < 15; i++) // добавлено <10, а то долго это всё
        {
            yield return new WaitForSeconds(0.25f);//yield return new WaitForSeconds(0.25f);
            SoundManager.Instance.soundClick.Play();
            //меняем перент-трансформ
            j++;
            if (j >= categoriesLeft.Count)
                j = 0;
            buySelector.transform.SetParent(categoriesLeft[j].transform);
            buySelector.transform.localPosition = Vector3.zero;
        }

        //выбралось
        yield return new WaitForSeconds(0.5f);
        //GameController.Instance.soundShopLast.Play();
        buySelector.SetActive(false);
        block.SetActive(false);

        //куплено
        BuyToy(j);
        //ShowToys(j);//index!
    }

    void BuyToy(int index)
    {
        ci = categoriesLeft[index].GetComponent<ShopItem>();
        ci.Unlock();
        categoriesLeft.Remove(categoriesLeft[index]);
        categoriesLeft.Shuffle();

        categoriesBoughtList.Add(ci);
        //categoriesBoughtList.Shuffle();

        categoriesBought++;

        UpdatePrice();
    }

    public void RandomizeCategory() //если осталось 1-2-3 - другое
    {
        //стопнуть корутину?
        //перемешать?
        if (RewardedVideoController.Instance.currency < categoryPrice)
        {
            //no money
            //RewardedVideoController.Instance.noMoneyDialogUI.SetActive(true);
            SoundManager.Instance.soundNot.Play();
            GameController.Instance.Vibrate(30);
        }
        else
        {
            //потратить деньги
            if (categoriesLeft.Count > 0)
            {
                RewardedVideoController.Instance.AddCurrency(-categoryPrice);
                StartCoroutine(GoRandom());
            }
        }
    }



    void SkinsShift(int val)
    {
        skinGroups[currentSkinGroup].SetActive(false);
        dots[currentSkinGroup].SetActive(false);

        currentSkinGroup += val;
        if (currentSkinGroup < 0)
        {
            currentSkinGroup = skinGroups.Length - 1;
        }
        if (currentSkinGroup >= skinGroups.Length)
        {
            currentSkinGroup = 0;
        }

        skinGroups[currentSkinGroup].SetActive(true);
        dots[currentSkinGroup].SetActive(true);
    }

    public void SkinsLeft()
    {
        SkinsShift(-1);
    }

    public void SkinsRight()
    {
        SkinsShift(1);
    }


}
