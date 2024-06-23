using UnityEngine;

public class BulletBillLauncher : MonoBehaviour
{
    public BulletBill bulletBill;
    public float delay = 3.0f;
    public float activeDistance = 12.0f;

    private float shootTimer;

    private bool canShoot;

    private void Update()
    {
        if(Vector3.Distance(LevelManager.Instance.Mario.transform.position, transform.position) < activeDistance) { 
            shootTimer += Time.deltaTime;
        
            if(shootTimer > delay) {
                shootTimer = 0.0f;

                LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.blockBumpSound);
                BulletBill b = Instantiate(bulletBill, transform.position, Quaternion.identity);

                GameObject player = LevelManager.Instance.Mario;
                int dir = player.transform.position.x < transform.position.x ? -1 : 1;
                b.Launch(dir);
            }
        }
        else {
            shootTimer = 0.0f;
        }
    }
}
