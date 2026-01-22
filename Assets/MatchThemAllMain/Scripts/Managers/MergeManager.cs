using System;
using System.Collections.Generic;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    [Header(" Settings ")] 
    [SerializeField] private int MergingItemIndex;
    
    [Header(" Go Up Settings ")]
    [SerializeField] private float goUpDistance;
    [SerializeField] private float goUpDuration;
    [SerializeField] private LeanTweenType goUpEasing;
    
    [Header(" Smash Settings ")]
    [SerializeField] private float smashDuration;
    [SerializeField] private LeanTweenType smashEasing;
    
    [Header(" Effects ")]
    [SerializeField] private ParticleSystem mergeParticle;
    
    private void Awake()
    {
        ItemSpotsManager.mergeStarted += OnMergeStarted;
    }

    private void OnDestroy()
    {
        ItemSpotsManager.mergeStarted -= OnMergeStarted;
    }

    private void OnMergeStarted(List<Item> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 targetPos = items[i].transform.position + items[i].transform.up * goUpDistance;

            Action callback = null;

            if (i == 0)
                callback = () => SmashItems(items);

            LeanTween.move(items[i].gameObject, targetPos, goUpDuration)
                .setEase(goUpEasing)
                .setOnComplete(callback);
        }
    }

    private void SmashItems(List<Item> items)
    {
        items.Sort((a,b) => a.transform.position.x.CompareTo(b.transform.position.x));

        float targetX = items[MergingItemIndex].transform.position.x;

        List<Item> targetItems = items;

        for (int i = 0; i < targetItems.Count; i++)
        {
            //passes the actor index where all elements will be combined
            if(i == MergingItemIndex)
                continue;
            if (targetItems.Count - 1 == i)
            {
                LeanTween.moveX(targetItems[i].gameObject, targetX, smashDuration)
                    .setEase(smashEasing)
                    .setOnComplete(() => FinishMerge(targetItems));
            }
            else
            {
                LeanTween.moveX(targetItems[i].gameObject, targetX, smashDuration)
                    .setEase(smashEasing);
            }
        }
    }

    private void FinishMerge(List<Item> targetItems)
    {
        for (int i = 0; i < targetItems.Count; i++)
        {
            Destroy(targetItems[i].gameObject);
        }

        ParticleSystem particle = Instantiate(mergeParticle, targetItems[MergingItemIndex].transform.position,
            Quaternion.identity, transform);
        
        particle.Play();
    }
}
