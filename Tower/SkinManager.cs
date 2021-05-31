using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;
    //здесь инфа о материалах, цветах, скинах, флагах и прочее

    [System.Serializable]
    public class TowerLayer
    {
        public Mesh[] layers;//7шт
    }

    //
    [Header("Sky")]
    public Material skyMaterial;
    public Texture[] skyTextures;

    [Header("Ground")]
    public Material groundMaterial;
    public float[] groundColors;

    [Header("Tower")]
    public Material towerMaterial;
    public TowerLayer[] towerLayers;//5шт
    public MeshFilter[] towers;//7шт


    [Header("Player")]
    public Material playerMaterial;
    public Material[] playerTextures;
    public Material[] iosTextures;
    public int currentPlayer;

    [Header("Flag")]
    public SkinnedMeshRenderer[] flags;
    public Material[] flagMaterials;
    public Sprite[] flagSprites;
    public int availableFlag;
    public int currentFlag;
    //public Texture[] flagTextures;

    [Header("Enemy")]
    public Material enemyMaterial;
    public Color[] enemyColors;

    [Header("ParticlesCrash")]
    public Material crashMaterial;
    public float[] crashColors;

    [Header("Helpers")]
    public Texture[] texturesMage;
    public Texture[] texturesArcher;
    public Material matWL;
    public Material matWR;
    public Material matAL;
    public Material matAR;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentFlag = PlayerPrefs.GetInt("FlagSkin", 0);
        SetFlag(currentFlag);

        availableFlag = PlayerPrefs.GetInt("FlagNext", (currentFlag + 1) % flagSprites.Length);

        currentPlayer = PlayerPrefs.GetInt("PlayerSkin", 0);
        SetPlayer(currentPlayer);//Random.Range(0, playerTextures.Length));

        RandomLocation();
    }

    public void RandomLocation()
    {
        //int r = Random.Range(0, skyTextures.Length);

        int r = 1;
        if (GameController.Instance)
            r = (GameController.Instance.currentLevel) % skyTextures.Length;
        //int r = 1;
        SetSky(r);
        SetGround(r);
        SetTower(r);
    }

    public void SetSky(int i)
    {
        skyMaterial.SetTexture("_MainTex", skyTextures[i]);
    }

    public void SetGround(int i)
    {
        groundMaterial.mainTextureOffset = new Vector2(groundColors[i], 0);
    }

    public void SetTower(int i)
    {
        //towerMaterial.mainTextureOffset = new Vector2(groundColors[i], 0);

        for (int j = 0; j < towers.Length; j++)
        {
            towers[j].mesh = towerLayers[i].layers[j];
        }

        crashMaterial.mainTextureOffset = new Vector2(crashColors[i], 0.5f);

    }



    public void SetPlayer(int i)
    {
        //playerMaterial.SetTexture("_MainTex", playerTextures[i]);
        currentPlayer = i;
        PlayerPrefs.SetInt("PlayerSkin", currentPlayer);

#if (UNITY_IOS || UNITY_IPHONE)
        playerMaterial.CopyPropertiesFromMaterial(iosTextures[i]);
#else
        playerMaterial.CopyPropertiesFromMaterial(playerTextures[i]);
#endif

        ShopManager.Instance.TransformSelection(i);
    }

    

    public void NextFlag()
    {
        currentFlag++;
        if (currentFlag >= flagMaterials.Length)
            currentFlag = 0;
        //выше - забудь

        currentFlag = availableFlag;

        PlayerPrefs.SetInt("FlagSkin", currentFlag); //available...
        SetFlag(currentFlag); //available...

        availableFlag = currentFlag + 1; //перенести в другое место (наверное некст флаг), т.к. флаг мы можем потерять
        if (availableFlag >= flagSprites.Length)
            availableFlag = 1;
        PlayerPrefs.SetInt("FlagNext", availableFlag);
    }

    public void LoseFlag()
    {
        availableFlag++;
        if (availableFlag >= flagSprites.Length)
            availableFlag = 1;
        PlayerPrefs.SetInt("FlagNext", availableFlag);
    }

    public void SetFlag(int i)
    {
        //flagMaterial.SetTexture("_MainTex", flagTextures[i]);
        for (int j = 0; j < flags.Length; j++)
            flags[j].materials[1].CopyPropertiesFromMaterial(flagMaterials[i]);  //=flagMaterials[i];
    }



    public void RandomEnemy()
    {
        int r = 1;
        r = Random.Range(0, enemyColors.Length);//длина цветов
        SetEnemy(r);
    }

    public void SetEnemy(int i)
    {
        enemyMaterial.color = enemyColors[i];
    }

}
