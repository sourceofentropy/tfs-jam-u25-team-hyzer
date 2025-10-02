using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattle1 : MonoBehaviour
{
    private CameraController cam;
    public Transform camPosition;
    public float camSpeed;

    public int threshold1, threshold2;

    public float activeTime, fadeoutTime, inactiveTime;
    private float activeCounter, fadeCounter, inactiveCounter;

    public Transform[] spawnPoints;
    private Transform targetPoint;
    public float moveSpeed;

    public Animator anim;

    public Transform boss;

    public float timeBetweenShots1, timeBetweenShots2;
    private float shotCounter;
    public GameObject bullet;
    public Transform shotPoint;

    public GameObject winObjects;

    private bool battleEnded;

    public bool isFinalBoss;
    public bool isBoss1 = true;
    public bool isBoss2 = false;

    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<CameraController>();
        cam.enabled = false;

        activeCounter = activeTime;

        shotCounter = timeBetweenShots1;
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.position = Vector3.MoveTowards(cam.transform.position, camPosition.position, camSpeed * Time.deltaTime);

        if(!battleEnded)
        {
            if (BossHealthController.instance.currentHealth > threshold1)
            {
                if (activeCounter > 0)
                {
                    activeCounter -= Time.deltaTime;
                    if (activeCounter <= 0)
                    {
                        fadeCounter = fadeoutTime;
                        anim.SetTrigger("vanish");
                        Debug.Log("trigger vanish");
                    }

                    shotCounter -= Time.deltaTime;
                    if (shotCounter <= 0)
                    {
                        shotCounter = timeBetweenShots1;

                        Instantiate(bullet, shotPoint.position, Quaternion.identity);
                    }
                }
                else if (fadeCounter > 0)
                {
                    fadeCounter -= Time.deltaTime;
                    if (fadeCounter <= 0)
                    {
                        boss.gameObject.SetActive(false);
                        inactiveCounter = inactiveTime;
                    }
                }
                else if (inactiveCounter > 0)
                {
                    inactiveCounter -= Time.deltaTime;
                    if (inactiveCounter <= 0)
                    {
                        boss.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
                        boss.gameObject.SetActive(true);

                        activeCounter = activeTime;

                        shotCounter = timeBetweenShots1;
                    }
                }
            }
            else
            {
                if (targetPoint == null)
                {
                    targetPoint = boss;
                    fadeCounter = fadeoutTime;
                    anim.SetTrigger("vanish");
                    Debug.Log("trigger vanish because target point is null");


                }
                else
                {
                    if (Vector3.Distance(boss.position, targetPoint.position) > 0.02f)
                    {
                        boss.position = Vector3.MoveTowards(boss.position, targetPoint.position, moveSpeed * Time.deltaTime);

                        if (Vector3.Distance(boss.position, targetPoint.position) <= 0.02f)
                        {
                            fadeCounter = fadeoutTime;
                            anim.SetTrigger("vanish");
                        }

                        shotCounter -= Time.deltaTime;
                        if (shotCounter <= 0)
                        {
                            if (PlayerHealthController.instance.currentHealth > threshold2)
                            {
                                shotCounter = timeBetweenShots1;

                            }
                            else
                            {
                                shotCounter = timeBetweenShots2;

                            }

                            Instantiate(bullet, shotPoint.position, Quaternion.identity);
                        }

                    }
                    else if (fadeCounter > 0)
                    {
                        fadeCounter -= Time.deltaTime;
                        if (fadeCounter <= 0)
                        {
                            boss.gameObject.SetActive(false);
                            inactiveCounter = inactiveTime;
                        }
                    }
                    else if (inactiveCounter > 0)
                    {
                        inactiveCounter -= Time.deltaTime;
                        if (inactiveCounter <= 0)
                        {
                            boss.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

                            targetPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                            int whileBreaker = 0;
                            while (targetPoint.position == boss.position && whileBreaker < 100)
                            {
                                targetPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                                whileBreaker++;
                            }

                            boss.gameObject.SetActive(true);

                            if (PlayerHealthController.instance.currentHealth > threshold2)
                            {
                                shotCounter = timeBetweenShots1;
                            }
                            else
                            {
                                shotCounter = timeBetweenShots2;
                            }
                        }
                    }
                }
            }
        } else
        {
            fadeCounter -= Time.deltaTime;
            if (fadeCounter < 0)
            {
                if (winObjects != null)
                {
                    winObjects.SetActive(true);
                    winObjects.transform.SetParent(null);
                }

                cam.enabled = true;
                gameObject.SetActive(false);
            }

        }

    }

    public void EndBattle()
    {
        battleEnded = true;

        fadeCounter = fadeoutTime;
        anim.SetTrigger("vanish");
        boss.GetComponent<Collider2D>().enabled = false;

        BossBullet[] bullets = FindObjectsOfType<BossBullet>();
        if(bullets.Length > 0)
        {
            foreach (BossBullet bullet in bullets)
            {
                Destroy(bullet.gameObject);
            }
        }

        if(isFinalBoss)
        {
            GameManager.Instance.GameWonSequence();
        }
    }


}
