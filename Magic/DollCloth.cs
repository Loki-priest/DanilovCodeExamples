using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ClothType
{
    Clothes,
    Hair, //ничего не отключаем
    Acces, //ничего не отключаем
    Pet, //ничего не отключаем
    Wand, //ничего не отключаем
    hz1,
    hz2,
    hz3,
    Fatality
}



public class DollCloth : MonoBehaviour {

    //здесь делать части тела которые вкл и выкл?

    public ClothType type = ClothType.Clothes;

    public GameObject[] toHide; //части тела которые скрыть (целые руки, тело,...)
    public GameObject[] toShow; //части тела которые показать (огрызки ног)

    public void DressMe()
    {
        gameObject.SetActive(true);

        for (int i = 0; i < toHide.Length; i++)
        {
            toHide[i].SetActive(false);
        }

        for (int i = 0; i < toShow.Length; i++)
        {
            toShow[i].SetActive(true);
        }
    }

    public void UndressMe()
    {
        gameObject.SetActive(false);

        for (int i = 0; i < toHide.Length; i++) //вернуть спрятанное
        {
            toHide[i].SetActive(true);
        }

        for (int i = 0; i < toShow.Length; i++) //скрыть огрызки
        {
            toShow[i].SetActive(false);
        }
    }




}
