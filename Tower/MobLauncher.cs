using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobLauncher : MonoBehaviour
{

    float launchTimer = 0.0f;

    public GameObject mobPrefab;
    GameObject currentMob;
    public List<GameObject> mobs;
    //public LauncherWP[] wayPoints;
    public List<LauncherWP> wayPoints;
    public List<Enemy> enemies;

    public LauncherWP startWP;

    void ResetMe()
    {
        mobs.Clear();
    }

    void LaunchMob()
    {
        currentMob = Instantiate(mobPrefab, GameController.Instance.botContainer);
        currentMob.transform.position = transform.position;
        mobs.Add(currentMob);
        wayPoints.Add(new LauncherWP());
        enemies.Add(currentMob.GetComponent<Enemy>());
        wayPoints[mobs.IndexOf(currentMob)] = startWP;

        //добавлять countMobs?
    }

    float dx;
    float dy;
    float dz;
    float Distance2(Vector3 a, Vector3 b)
    {
        dx = a.x - b.x;
        dx *= dx;
        dy = a.y - b.y;
        dy *= dy;
        dz = a.z - b.z;
        dz *= dz;
        return dx + dy + dz;
    }

    Quaternion targetRotation;
    Vector3 lookAt;
    void ProcessMobs()
    {
        //for (int i = 0; i < mobs.Count; i++)
        for (int i = mobs.Count - 1; i >= 0; i--)
        {
            if (mobs[i] && wayPoints[i])
            {
                //двигаем по вейпоинтам
                targetRotation = Quaternion.LookRotation(/*nextPoint*/wayPoints[i].transform.position - mobs[i].transform.position + transform.up * 0.001f);
                mobs[i].transform.rotation = targetRotation;// Quaternion.Slerp(transform.rotation, targetRotation, 20 * Time.deltaTime); //5
                mobs[i].transform.position += mobs[i].transform.forward * 10f * Time.deltaTime;

                lookAt = Vector3.zero;
                lookAt.y = wayPoints[i].transform.eulerAngles.y;
                enemies[i].pivot.transform.eulerAngles = lookAt;

                if (Distance2(mobs[i].transform.position, wayPoints[i].transform.position) <= 0.04f) //0.2f^2
                {
                    if (wayPoints[i].nextWaypoints.Length == 0)
                    {
                        mobs[i].GetComponent<Enemy>().nextWaypoint = wayPoints[i].whereToStop;
                        enemies.Remove(enemies[i]);
                        wayPoints.Remove(wayPoints[i]);
                        mobs.Remove(mobs[i]);
                    }
                    else
                    {
                        wayPoints[i] = wayPoints[i].nextWaypoints[Random.Range(0, wayPoints[i].nextWaypoints.Length)]; //чекать занятость
                    }
                }
                //если вейпоинт ноль - передаем его стене

            }
        }

        /*
        for (int i = 0; i < mobs.Count; i++)
        {
            if (mobs[i] && wayPoints[i] == null)
            {
                mobs.Remove(mobs[i]);
            }
        }
        */
    }

    private void Update()
    {
        launchTimer += Time.deltaTime;
        if (launchTimer >= 2.0f)
        {
            launchTimer = 0.0f;
            LaunchMob();
        }

        ProcessMobs();

    }


}
