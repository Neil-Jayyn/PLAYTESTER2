using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public SpriteRenderer crosshairSprite;
    public LayerMask interactableLayer;
    public DuckGameManager duckGameManager;

    //public AudioSource sfx;  // Not affected by glitches, so separate audio source used here
    //public AudioClip failureSFX;
    //public AudioClip shootSFX;

    void Start()
    {
        duckGameManager = GameObject.Find("DuckGameManager").GetComponent<DuckGameManager>();

        //sfx = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (duckGameManager.gameOver == false && duckGameManager.isTutorialPlaying == false)
        { 
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            crosshairSprite.transform.position = mousePos;

            if (Input.GetMouseButtonDown(0)) // Left click
            {
                //sfx.clip = shootSFX;
                //sfx.Play();
                if (!duckGameManager.isGlitch)
                {
                    duckGameManager.gunAudioSource.clip = duckGameManager.wholesomeGunSFX;
                }
                else
                {
                    duckGameManager.gunAudioSource.clip = duckGameManager.realGunshotSFX;
                }
                duckGameManager.gunAudioSource.Play();
                DetectObjectUnderCrosshair(mousePos);
            }
        }
    }

    void ChooseSFX(AudioSource audioaudioClipsSource, AudioClip[] audioClips)
    {
        int index = Random.Range(0, audioClips.Length);
        audioaudioClipsSource.clip = audioClips[index];
    }

    void DetectObjectUnderCrosshair(Vector2 mousePos)
    {
        Collider2D hit = Physics2D.OverlapPoint(mousePos, interactableLayer);
        if (hit != null)
        {
            // If the crosshair is over something, perform actions (like hitting a duck or trash)
            if (hit.CompareTag("Duck"))
            {
                // Add 1 point for duck
                duckGameManager.AddPoints(1);
                Debug.Log("Hit Duck +1 Point");
                
                duckGameManager.audioSources[0].Play();
                if (!duckGameManager.isGlitch)
                {
                    duckGameManager.boardSmackAudioSource.Play();
                    ChooseSFX(duckGameManager.successHitAudioSource, duckGameManager.successHitSFX);
                    ChooseSFX(duckGameManager.quackAudioSource, duckGameManager.quackSFX);

                } else
                {
                    ChooseSFX(duckGameManager.successHitAudioSource, duckGameManager.screamSFX);
                }
                duckGameManager.successHitAudioSource.Play();
                duckGameManager.quackAudioSource.Play();
                Destroy(hit.gameObject); // Destroy duck after hit
                
            }
            else if (hit.CompareTag("Trash"))
            {
                // Subtract 1 point for trash
                duckGameManager.AddPoints(-1);
                Debug.Log("Hit Trash -1 Point");
                
                duckGameManager.trashAudioSource.Play();
                duckGameManager.awwAudioSource.Play();
               
                Destroy(hit.gameObject); // Destroy trash after hit

            }
        } else
        {
            duckGameManager.missAudioSource.Play();
        }
    }
}
