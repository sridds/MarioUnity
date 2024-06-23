using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public enum PowerupEffect
    {
        SuperSize = 1,
        FirePower = 2,
        StarPower
    }

    public PowerupEffect effect;

    public void OnPowerupEmerged()
    {
        if(effect == PowerupEffect.StarPower){
            GetComponent<Rigidbody2D>().velocity = new Vector2(3, 10);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player")) {
            if(effect != PowerupEffect.StarPower) {
                LevelManager.Instance.Powerup((int)effect);
            }
            else {
                LevelManager.Instance.StarPower();
            }
            Destroy(gameObject);
        }
    }
}
