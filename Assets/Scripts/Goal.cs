using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Goal : MonoBehaviour {

    public Text victoryText;
    public Text victorySubtext;

    private void Start()
    {
        victoryText.enabled = false;
        victorySubtext.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Trigger victory screen and disable player movement if player touches Goal
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(this.gameObject);
            other.gameObject.GetComponent<FirstPersonController>().enabled = false;
            victoryText.enabled = true;
            victorySubtext.enabled = true;
        }
    }
}
