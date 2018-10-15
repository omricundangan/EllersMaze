using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wall : MonoBehaviour {

    public int hp;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            Destroy(col.gameObject);    // Remove the bullet upon impact
            if (hp > 0)      // Only reduce HP if above 0
            {
                hp--;
            }
            if (hp == 0)    // Destroy the wall at 0 hp
            {
                Destroy(this.gameObject);
            }
        }
    }
}
