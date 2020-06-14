using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveAway : MonoBehaviour
{
    [SerializeField]
    Transform targetDriveAway;
    [SerializeField]
    Transform targetComeback;

    public AudioClip awayClip;
    public AudioClip comebackClip;

    private AudioSource audioSource;


    [SerializeField]
    float speed = 0.2f; // Speed in units per second

    public bool triggerDrive;

    private bool isOut = false;

    public void triggerDriveAway()
    {
        //currentTarget = targetDriveAway;
        //triggerDrive = true;
        isOut = true;
        StartCoroutine(ComeBackInTimer());
    }

    private IEnumerator ComeBackInTimer()
    {
        yield return new WaitForSeconds(2f);
        isOut = false;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = awayClip;
    }

    void Update()
    {
        if (!isOut)
        {
            if (audioSource.clip != comebackClip)
            {
                audioSource.clip = comebackClip;
                audioSource.Play();
            }
        }
        else {
            if (audioSource.clip != awayClip)
            {
                audioSource.clip = awayClip;
                audioSource.Play();
            }
        }


        var targetVector = isOut ? targetDriveAway : targetComeback;

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetVector.position, step);

        if(Vector3.Distance(transform.position, targetVector.position) <= 0.1)
        {
            audioSource.Stop();
        }
        
    }
}
