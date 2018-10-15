using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTriggerController : MonoBehaviour {

    public GameObject mazeStart;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))  // create a Row if player enters the trigger zone
        {
            mazeStart.GetComponent<MazeController>().createRow();
            Destroy(this.gameObject);
        }
        else if (other.gameObject.CompareTag("Bullet")) // Terminate maze if bullet enters trigger zone
        {
            mazeStart.GetComponent<MazeController>().finishMaze();
            Destroy(this.gameObject);
        }
    }
}
