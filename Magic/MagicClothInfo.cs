using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicClothInfo : MonoBehaviour
{
    public string name;
    public ClothType clothType;
    public DollCloth clothBoy;
    public DollCloth clothGirl;
    public DollsClothManager.Gender gender = DollsClothManager.Gender.Uni; //тип одежды по полу
    public DollsItem dollsItem;//айтем одежды
                               //айтемы цветов. возможно переместить их в класс доллКлос
                               //или скрипт шопклосайтм

    public Material affectedMaterial;
    public SkinnedMeshRenderer affectedSMR;
    public MeshRenderer affectedMR;
    //public Sprite shopIcon;
    //public 

    void ApplyAffectedMaterial()
    {
        if (affectedMaterial)
        {
            if (affectedSMR)
            {
                affectedSMR.material = affectedMaterial;
            }

            if (affectedMR)
            {
                affectedMR.material = affectedMaterial;
            }
        }
    }



    public void TryToDress(int index, DollsClothManager Instance)
    {
        if (Instance.isMale && gender == DollsClothManager.Gender.Female || !Instance.isMale && gender == DollsClothManager.Gender.Male)
        {
            //мы пытаемся надеть женское на мужика и наоборот = реакция
            Debug.Log("Gender WTF");
        }
        else
        {
            if (Instance.isMale && clothBoy)
            {
                //снимаем все из этой категории, или надетую
                if (Instance.currentSlot[(int)clothType])
                {
                    Instance.currentSlot[(int)clothType].UndressMe();
                }
                //надеваем эту одежду
                ApplyAffectedMaterial();
                clothBoy.DressMe();
                Instance.currentSlot[(int)clothType] = clothBoy;//DollsClothManager.Instance.boy.currentSlot[(int)clothType] = clothBoy;
                Instance.ApplyChanges(clothType, index);
                //брови
                if (affectedMaterial && clothType == ClothType.Hair)
                {
                    Instance.ChangeBroves(affectedMaterial);
                }
            }
            if (!Instance.isMale && clothGirl)
            {
                if (Instance.currentSlot[(int)clothType])
                {
                    Instance.currentSlot[(int)clothType].UndressMe();
                }
                ApplyAffectedMaterial();
                clothGirl.DressMe();
                Instance.currentSlot[(int)clothType] = clothGirl;//DollsClothManager.Instance.girl.currentSlot[(int)clothType] = clothGirl;
                Instance.ApplyChanges(clothType, index);
                //брови
                if (affectedMaterial && clothType == ClothType.Hair)
                {
                    Instance.ChangeBroves(affectedMaterial);
                }
            }
        }
    }




}




