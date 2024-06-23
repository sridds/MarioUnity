using System.Collections;
using UnityEngine;

public class Flagpole : MonoBehaviour
{
    public Transform flag;
    public Transform poleBottom;
    public float speed = 6f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            StartCoroutine(MoveTo(flag, poleBottom.position));
            StartCoroutine(LevelCompleteSequence(collision.transform));
        }
    }

    private IEnumerator LevelCompleteSequence(Transform player) 
    {
        player.GetComponent<MarioMovement>().enabled = false;
        player.transform.position = new Vector2(poleBottom.position.x, player.transform.position.y);
        LevelManager.Instance.LevelComplete();

        Animator animator = player.GetComponent<Animator>();

        animator.SetFloat("absSpeed", 0.0f);
        animator.SetBool("isClimbing", true);
        LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.flagpoleSound);

        yield return MoveTo(player, poleBottom.position);
        animator.SetBool("isClimbing", false);

        LevelManager.Instance.AddPoints(2000);

        LevelManager.Instance.musSource.clip = LevelManager.Instance.levelCompleteMusic;
        LevelManager.Instance.musSource.loop = false;
        LevelManager.Instance.musSource.Play();
        LevelManager.Instance.AddTimeToScore();

        yield return MoveTo(player, player.position + Vector3.right);
        yield return MoveTo(player, player.position + Vector3.right + new Vector3(0, -1.25f, 0));


        animator.SetBool("isJumping", false);
        animator.SetBool("isFalling", false);
        animator.SetFloat("absSpeed", 5.0f);
        yield return MoveTo(player, player.position + (Vector3.right * 4));

        player.gameObject.SetActive(false);

        yield return new WaitForSeconds(7.0f);
        LevelManager.Instance.RestartLevel();
    }

    private IEnumerator MoveTo(Transform subject, Vector3 destination) 
    { 
        while(Vector3.Distance(subject.position, destination) > 0.125f) {
            subject.position = Vector3.MoveTowards(subject.position, destination, speed * Time.deltaTime);
            yield return null;
        }

        subject.position = destination;
        yield return null;
    }
}
