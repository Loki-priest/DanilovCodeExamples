using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSpell : MonoBehaviour
{


    public Transform from;
    public Transform to;

    //tweentransform

    float speed;

    public Transform artContainer;

    public MagicTarget target;

    public ParticleSystem ps;

    public TweenScale ts;

    bool isCast = false;

    public bool isPlayers = true;

    public Transform particlesContainer;

    public ParticleSystem[] particles;




    private void OnEnable()
    {
        //gameObject.transform.position = from.position;
        //speed = 5.0f;
    }

    public void Cast(Transform _from = null, Transform _to = null)
    {
        if (!isCast)
        {
            artContainer.gameObject.SetActive(true);
            if (ps2 != null)
            {
                ps2.Play();
            }

            gameObject.SetActive(true);
            if (_from == null)
                gameObject.transform.position = from.position;
            else
                gameObject.transform.position = _from.position;

            if (_to != null)
                to = _to;

            speed = 5.0f;//5.0f;
            isCast = true;
            ps.Play();


            if (ps2 != null)
                ts = ps2.GetComponent<TweenScale>();

            if (ts != null)
            {
                ts.ResetToBeginning();
                ts.PlayForward();
            }
        }
    }


    public void Fly()
    {
        //ngui
        transform.LookAt(to);

        transform.position += transform.forward * speed * Time.deltaTime;

        if (Vector3.Distance(to.transform.position, transform.position) < 0.5f) //ОПТИМИЗИРОВАТЬ
        {
            //gameObject.SetActive(false);
            //artContainer.DestroyChildren();//временно
            if (ps2 != null)
            {
                ps2.Stop();
            }

            //HG_Draw.Instance.ResetDraw();//временно
            target.CountDamage(isPlayers);
            //временно
            if (isPlayers && msm.gameType == MagicSpellsManager.GameType.Boss && mbfm.isGameOn)
            {
                mbfm.NextSpellReady();
            }

            isCast = false;
            ps.Stop();
        }
    }


    MagicSpellsManager msm;
    MagicBossFightManager mbfm;

    private void Start()
    {
        msm = MagicSpellsManager.Instance;
        mbfm = MagicBossFightManager.Instance;
    }


    public void Update()
    {
        if (isCast)
            Fly();
    }



    public ParticleSystem ps2;
    public ParticleSystem.Burst burst;
    public Mesh mesh;
    int count;

    public void SetMesh(Transform[] _myT)
    {
        mesh = new Mesh();
        mesh.vertices = ConvertToVector3Array(_myT);

        count = mesh.vertices.Length;

        ParticleSystem.ShapeModule sh = ps2.shape;
        sh.enabled = true;
        sh.shapeType = ParticleSystemShapeType.Mesh;
        sh.mesh = mesh;

        //burst = getbu
        ParticleSystem.EmissionModule em;
        em = ps2.emission;

        burst = new ParticleSystem.Burst();
        burst.time = 0.0f;
        burst.count = count;//Mathf.Clamp(count, 0, 300);// count;
        burst.cycleCount = 1;
        burst.repeatInterval = 0.01f;
        burst.probability = 1.0f;

        em.SetBurst(0, burst);
    }

    Vector3[] ConvertToVector3Array(Transform[] floats)
    {
        Vector3[] newVertices = new Vector3[floats.Length];
        for (int i = 0; i < floats.Length; i++)
        {
            newVertices[i] = floats[i].position;
        }
        return newVertices;
    }


}
