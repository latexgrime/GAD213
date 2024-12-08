using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EntityUIManager : MonoBehaviour
{
    private Canvas healthBarCanvas;
    [SerializeField] private Image healthBarForegroundSprite;
    private Camera cam;

    private void Start()
    {
        // Get them components!!.
        healthBarCanvas = GetComponent<Canvas>();
        cam = Camera.main;
    }

    private GameObject FindChildWithTag(GameObject parent, string objectTag)
    {
        GameObject childToReturn = null;
        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag(objectTag))
            {
                childToReturn = child.gameObject;
                break;
            }
        }

        return childToReturn;
    }
    
    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        healthBarForegroundSprite.fillAmount = currentHealth / maxHealth;
    }

    private void Update()
    {
        // Make it so its always looking at the player.
        if (!gameObject.CompareTag("PlayerUI")) {
            healthBarCanvas.transform.rotation = Quaternion.LookRotation(healthBarCanvas.transform.position - cam.transform.position);
        }
    }
}
