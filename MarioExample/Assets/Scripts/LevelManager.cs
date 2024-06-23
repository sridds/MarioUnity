using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    [Header("Sources - DO NOT CHANGE")]
    public AudioSource sfxSource;
    public AudioSource musSource;

    [Header("Level Changes")]
    public int levelTime = 400;
    public int hurryUpTime = 100;
    public float timePassMultiplier = 1.25f;

    [Header("Music")]
    public AudioClip stageMusic;
    public AudioClip hurryUpStageMusic;
    public AudioClip hurryUpJingle;
    public AudioClip starmanMusic;
    public AudioClip hurryUpStarmanMusic;
    public AudioClip levelCompleteMusic;

    [Header("Sound Effects")]
    public AudioClip flagpoleSound;
    public AudioClip blockBumpSound;
    public AudioClip blockBreakSound;
    public AudioClip coinSound;
    public AudioClip fireThrowSound;
    public AudioClip jumpSmallSound;
    public AudioClip jumpSuperSound;
    public AudioClip powerUpSound;
    public AudioClip powerUpAppearSound;
    public AudioClip powerDownSound;
    public AudioClip kickSound;
    public AudioClip stompSound;
    public AudioClip deathClip;

    [Header("Additional Bonuses")]
    public int coinBonus = 200;
    public int powerupBonus = 1000;
    public int starmanBonus = 1000;


    private GameObject mario;
    public GameObject Mario { 
        get {
            if(mario == null) {
                mario = GameObject.FindWithTag("Player");
            }
            return mario;
        }
    }
    public int marioSize { get; private set; }
    public int coins { get; private set; }
    public int score { get; private set; }

    public int timeRemaining { get; private set; }
    private float timeFloat;

    public bool isInvincibleStarman { get; private set; }

    private bool isHurrying;
    public bool isDead { get; private set; }
    public bool levelComplete { get; private set; }

    private IEnumerator hurryUpCoroutine;

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        if(timeFloat > 0 && !levelComplete && !isDead) { 
            timeFloat -= Time.deltaTime * timePassMultiplier;
            timeRemaining = Mathf.RoundToInt(timeFloat);
        }

        if(!isHurrying && timeRemaining <= hurryUpTime && !levelComplete) {
            hurryUpCoroutine = HurryUp();
            StartCoroutine(hurryUpCoroutine);
            isHurrying = true;
        }

        if (timeRemaining <= 0 && !isDead && !levelComplete) InstantDeath();
    }

    public void RestartLevel()
    {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }

    private IEnumerator HurryUp()
    {
        musSource.clip = hurryUpJingle;
        musSource.loop = false;
        musSource.Play();

        yield return new WaitForSeconds(hurryUpJingle.length);

        musSource.clip = isInvincibleStarman ? hurryUpStarmanMusic : hurryUpStageMusic;
        musSource.loop = true;
        musSource.Play();
    }

    public void LevelComplete()
    {
        levelComplete = true;
        if (hurryUpCoroutine != null) StopCoroutine(hurryUpCoroutine);
        musSource.Stop();
    }

    public void AddTimeToScore()
    {
        StartCoroutine(AddTimeToScoreCo());
    }

    private IEnumerator AddTimeToScoreCo()
    {
        while (timeRemaining > 0) {
            if (timeRemaining - 5 >= 0)
            {
                timeRemaining -= 5;
                score += 250;
            }
            else {
                timeRemaining -= 1;
                score += 50;
            }

            sfxSource.clip = coinSound;
            sfxSource.Play();
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void NewGame()
    {
        timeFloat = levelTime;
        marioSize = 0;
        coins = 0;

        musSource.Stop();
        musSource.clip = stageMusic;
        musSource.Play();
    }

    public void AddCoin()
    {
        sfxSource.PlayOneShot(coinSound);
        AddPoints(coinBonus);
        coins++;
    }

    public void TakeHit()
    {
        if (marioSize < 0) return;
        
        if((marioSize - 1) < 0) {
            marioSize--;

            isDead = true;
            sfxSource.PlayOneShot(deathClip);
            musSource.Stop();
            musSource.volume = 0;

            Mario.GetComponent<Animator>().SetBool("isStarman", false);
            Mario.GetComponent<DeathAnimation>().enabled = true;
        }
        else {
            StartCoroutine(StartPowerdown(marioSize - 1));
        }
    }

    public void InstantDeath()
    {
        marioSize = 0;
        TakeHit();
    }

    public void Powerup(int powerLevel) {

        AddPoints(powerupBonus);
        if(powerLevel <= marioSize) {
            sfxSource.PlayOneShot(coinSound);
            return;
        }

        StartCoroutine(StartPowerup(powerLevel));
    }

    private IEnumerator StartPowerdown(int powerLevel)
    {
        sfxSource.PlayOneShot(powerDownSound);

        Animator marioAnimator = Mario.GetComponent<Animator>();
        marioAnimator.SetBool("isPoweringDown", true);
        Time.timeScale = 0;

        marioAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        yield return new WaitForSecondsRealtime(1f);
        marioAnimator.SetBool("isPoweringDown", false);

        Time.timeScale = 1;
        marioAnimator.updateMode = AnimatorUpdateMode.Normal;
        marioSize = powerLevel;
        marioAnimator.SetInteger("marioSize", powerLevel);

        StartCoroutine(Invincible());

        yield return null;
    }

    private IEnumerator Invincible()
    {
        SpriteRenderer renderer = Mario.GetComponentInChildren<SpriteRenderer>();
        Mario.layer = LayerMask.NameToLayer("Ignore Enemy Collision");

        for (int i = 0; i < 20; i++) {
            Color temp = renderer.color;
            temp.a = 0;
            renderer.color = temp;
            yield return new WaitForSeconds(0.05f);
            temp.a = 255;
            renderer.color = temp;
            yield return new WaitForSeconds(0.05f);
        }

        Mario.layer = LayerMask.NameToLayer("Player");

        yield return null;
    }

    private IEnumerator StartPowerup(int powerLevel)
    {
        sfxSource.PlayOneShot(powerUpSound);

        if (powerLevel == 2) marioSize = 1;

        Animator marioAnimator = Mario.GetComponent<Animator>();
        marioAnimator.SetBool("isPoweringUp", true);
        Time.timeScale = 0;

        marioAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        yield return new WaitForSecondsRealtime(1f);
        marioAnimator.SetBool("isPoweringUp", false);

        Time.timeScale = 1;
        marioAnimator.updateMode = AnimatorUpdateMode.Normal;
        marioSize = powerLevel;
        marioAnimator.SetInteger("marioSize", powerLevel);

        yield return null;
    }

    public void StarPower()
    {
        if (isInvincibleStarman) return;

        AddPoints(starmanBonus);
        StartCoroutine(StarmanPowerup());
    }

    public void AddPoints(int pointAmt)
    {
        score += pointAmt;
    }

    private IEnumerator StarmanPowerup()
    {
        sfxSource.PlayOneShot(powerUpSound);

        isInvincibleStarman = true;
        Mario.GetComponent<Animator>().SetBool("isStarman", true);

        musSource.Stop();
        musSource.clip = isHurrying ? hurryUpStarmanMusic : starmanMusic;
        musSource.Play();

        yield return new WaitForSeconds(12);
        isInvincibleStarman = false;
        Mario.GetComponent<Animator>().SetBool("isStarman", false);

        musSource.Stop();
        musSource.clip = isHurrying ? hurryUpStageMusic : stageMusic;
        musSource.Play();
    }
}
