using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using TMPro;
using System.Collections.Generic;
using Image = UnityEngine.UI.Image;
using System.Collections;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum eGameState
{
    MainMenu,
    AutoToggle,
    Playing,
    GameOver
}

public class FindShooterManager : MonoBehaviour
{
    public eGameState gameState = eGameState.MainMenu;

    public int currentLevelIndex = 1;

    private PathManager pathManager;
    private LevelManager levelManager;

    public Car[] cars;

    public Bullet[] bullets;

    public Tower[] towers;

    public Sprite[] gibberishSprites;

    public AudioClip[] policeSirensAudioClips;

    private Tower currentSelectedTower;

    private int numTries = 100;

    private Vector3 offset = new Vector3(-0.5f, -0.5f, 0f);

    public SpriteRenderer bk_glow, noise_1, noise_2;

    public Ease noise_1_ease;
    public float noise_1_duration;
    public Vector3 noise_1_moveby;
    public Ease noise_2_ease;
    public float noise_2_duration;
    public Vector3 noise_2_moveby;
    public Ease bk_glow_ease;
    public float bk_glow_duration;
    public float bk_glow_fade_amount;

    private int currentFine = 0;

    public LineRenderer rayFromTower;

    public TMP_Text fineText;
    public Image fineSelector;
    private Vector2 selectorStart = new Vector2(-57f, 50f);
    private Vector2 selectorMoveOffset = new Vector2(57f, -50f);

    public Image mainMenu;
    public Sprite mainMenuOn;
    public Sprite mainMenuOff;

    public CanvasGroup mainMenuScreen;
    public CanvasGroup autoToggleScreen;
    public CanvasGroup creditScreen;

    public AudioSource bgm;
    public AudioSource trafficAmbience;
    public AudioSource micAudioSource;
    public AudioSource laserLoopAudioSource;

    public SpriteRenderer towerOn;

    public Image introImage_1;
    public Image introImage_2;
    public Image introImage_3;

    private bool pressOne = false;

    public class VoilationCarMap
    {
        public Car car;
        public GameObject voilationCard;

        public VoilationCarMap(Car car, GameObject voilationCard)
        {
            this.car = car;
            this.voilationCard = voilationCard;
        }
    }

    private List<VoilationCarMap> spawnedCars = new List<VoilationCarMap>();
    private bool spawningLevel = false;

    public Image crackle;

    private Level currentLevel;

    public Transform voilationCardHolder;

    public Image gibberishImage;

    public LaserModeOnOff laserModeOnOff;
    private bool laserAutoOn = true;

    public GameObject cashStackPrefab;

    public AudioClip voilatorEliminated;

    public void Awake()
    {
        int seed = DateTime.Now.Millisecond;

        pathManager = GetComponentInChildren<PathManager>();
        levelManager = GetComponentInChildren<LevelManager>();

        laserModeOnOff.enabled = false;

        fineSelector.gameObject.SetActive(false);

        Random.InitState(seed);
        
        gameState = eGameState.MainMenu;
        StartCoroutine("MenuMenuAnimation");

        PlayBGM();
    }

    void PlayBGM()
    {
        bgm.DOKill();
        bgm.volume = 0f;

        float bgmVolume = gameState == eGameState.MainMenu ? 0.5f : 0.05f;

        bgm.DOFade(bgmVolume, 3f).SetEase(Ease.InOutQuad);
        bgm.DOFade(0f, 3f).SetEase(Ease.InOutQuad).SetDelay(bgm.clip.length - 5f);
        bgm.Play();
        Invoke("PlayBGM", bgm.clip.length + 2f);
    }

    IEnumerator MenuMenuAnimation()
    {
        bool isMainMenuOffState = true;
        while (true)
        {
            mainMenu.sprite = isMainMenuOffState ? mainMenuOff : mainMenuOn;
            yield return new WaitForSeconds(1f);
            isMainMenuOffState = !isMainMenuOffState;
        }
    }

    public void BeginClicked()
    {
        gameState = eGameState.AutoToggle;

        mainMenuScreen.DOFade(0f, 1f).OnComplete(() => mainMenuScreen.gameObject.SetActive(false));
        autoToggleScreen.DOFade(1f, 1f);

        bgm.DOKill();
        bgm.DOFade(0.05f, 0.3f).SetEase(Ease.InOutQuad);

        trafficAmbience.gameObject.SetActive(true);

        laserModeOnOff.Setup(OnLaserModeOnOffToggleClicked, laserAutoOn);

        rayFromTower.gameObject.SetActive(false);

        StopCoroutine ("MenuMenuAnimation");

        StartCoroutine("TowerOnOff");

        StartCoroutine("AutoToggle");
    }

    IEnumerator AutoToggle()
    {
        introImage_1.DOKill();
        introImage_1.DOFade(1f, 1f);

        yield return new WaitForSeconds(4f);

        introImage_1.DOFade(0f, 1f);
        introImage_2.DOFade(1f, 1f);

        yield return new WaitForSeconds(1f);

        pressOne = true;

        yield return new WaitUntil(() => currentFine == 100);

        pressOne = false;

        laserModeOnOff.enabled = true;

        introImage_2.DOFade(0f, 1f);
        introImage_3.DOFade(1f, 1f);

        fineSelector.gameObject.SetActive(true);
        SetCurrentFine();

        yield return new WaitUntil(() => !laserAutoOn);

        laserModeOnOff.enabled = false;

        introImage_3.DOFade(0f, 0.2f);
    }

    public void BackClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator TowerOnOff()
    {
        while (true)
        {
            towerOn.gameObject.SetActive(!towerOn.gameObject.activeSelf);

            yield return new WaitForSeconds(1f);
        }
    }

    public void StartPlaying()
    {
        fineSelector.GetComponent<RectTransform>().anchoredPosition = selectorStart;

        //Cursor.SetCursor(cursorTexture, cursorOffset, CursorMode.ForceSoftware);
        Cursor.visible = false;

        StaticAnimations();

        currentSelectedTower = towers[0];
        currentSelectedTower.SelectTower();

        SetCurrentFine();

        SetCurrentLevel();
    }

    void OnLaserModeOnOffToggleClicked()
    {
        this.laserAutoOn = !laserAutoOn;

        this.laserModeOnOff.SetSprite(this.laserAutoOn);

        if (!laserAutoOn)
        {
            gameState = eGameState.Playing;

            autoToggleScreen.DOFade(0f, 0.5f).OnComplete(() => autoToggleScreen.gameObject.SetActive(false));

            StopCoroutine("TowerOnOff");

            laserLoopAudioSource.Play();

            StartCoroutine("PoliceSirens");

            towerOn.gameObject.SetActive(false);

            StartPlaying();
        }
    }

    IEnumerator PoliceSirens()
    {
        while(gameState == eGameState.Playing)
        {
            float waitTime = Random.Range(4f, 10f);

            yield return new WaitForSeconds(waitTime);

            int randomPoliceSiren = Random.Range(0, policeSirensAudioClips.Length);

            AudioManager.instance.PlaySound(policeSirensAudioClips[randomPoliceSiren], 0.1f);

            yield return new WaitForSeconds(policeSirensAudioClips[randomPoliceSiren].length);
        }
    }

    private void LevelComplete()
    {
        print("Level Complete: " + currentLevelIndex);

        currentLevelIndex += 1;

        if (currentLevelIndex <= levelManager.levels.Length)
        {
            SetCurrentLevel();
        }
        else
        {
            gameState = eGameState.GameOver;

            Cursor.visible = true;

            laserLoopAudioSource.Stop();

            StopCoroutine("PoliceSirens");
            
            creditScreen.DOFade(1f, 1f);
        }
    }

    void StaticAnimations()
    {
        bk_glow.DOKill();
        bk_glow.DOFade(bk_glow_fade_amount, bk_glow_duration).SetEase(bk_glow_ease);

        noise_1.DOKill();
        noise_1.transform.DOBlendableLocalMoveBy(noise_1_moveby, noise_1_duration).SetEase(noise_1_ease).SetLoops(-1, LoopType.Yoyo);

        noise_2.DOKill();
        noise_2.transform.DOBlendableLocalMoveBy(noise_2_moveby, noise_2_duration).SetEase(noise_2_ease).SetLoops(-1, LoopType.Yoyo);
    }

    Bullet GetFreeBullet()
    {
        foreach (Bullet bullet in bullets)
        {
            if (!bullet.Fired)
            {
                return bullet;
            }
        }

        return null;
    }

    Car GetRandomCar()
    {
        for (int i = 0; i < numTries; i++)
        {
            int randomIndex = Random.Range(0, cars.Length);

            if (cars[randomIndex].isFree)
                return cars[randomIndex];
        }

        return null;
    }

    void Fire()
    {
        Bullet bullet = GetFreeBullet();

        if (bullet != null)
        {
            bullet.Fire(currentSelectedTower.transform.position, currentFine);
            currentSelectedTower.Fire();
            laserLoopAudioSource.volume = 0f;
            laserLoopAudioSource.DOFade(0.4f, 1f);
        }
    }

    void SetCurrentFine()
    {
        fineText.text = "" + currentFine;
    }

    IEnumerator SpawnVoilations()
    {
        Car car = null;
        DOTweenPath path = null;
        spawningLevel = true;
        
        yield return new WaitForSeconds(currentLevel.spawnDelay);

        foreach (VoilationMap voilationMap in currentLevel.voilationMaps)
        {
            int fine = levelManager.GetFine(voilationMap.voilation);
            Color fineColor = levelManager.GetFineColor(voilationMap.voilation);
            GameObject voilationPrefab = levelManager.GetVoilationPrefab(voilationMap.voilation);
            AudioClip micAudioClip = levelManager.GetMicAudioClip(voilationMap.voilation);
            AudioClip sfxAudioClip = levelManager.GetRandomSFXClip(voilationMap.voilation);
            car = GetRandomCar();

            switch (voilationMap.voilation)
            {
                case eVoilation.SPEEDING_BETWEEN_20_30:
                    path = pathManager.GetRandomPath();
                    car.StartMoving(path, 0.25f, fine, fineColor, CarMessage);
                    break;

                case eVoilation.SPEEDING_BETWEEN_50_60:
                    path = pathManager.GetRandomPath();
                    car.StartMoving(path, 0.4f, fine, fineColor, CarMessage);
                    break;

                case eVoilation.SPEEDING_GREATER_60:
                    path = pathManager.GetRandomPath();
                    car.StartMoving(path, 0.6f, fine, fineColor, CarMessage);
                    break;

                case eVoilation.OTHER_WRECKLESS:
                    path = pathManager.GetRandomPath();
                    car.StartMoving(path, 0.7f, fine, fineColor, CarMessage);
                    break;

                case eVoilation.SIGNAL_RED_LIGHT:
                    path = pathManager.GetRandomPath();
                    car.StartMoving(path, Random.Range(0.4f, 0.5f), fine, fineColor, CarMessage);
                    break;

                case eVoilation.SIGNAL_PEDESTRIAN:
                    path = pathManager.GetRandomParkingPath();
                    car.ParkedCar(path, fine, fineColor, CarMessage);
                    break;

                case eVoilation.OTHER_MOBILE:
                    path = pathManager.GetRandomPath();
                    car.StartMoving(path, Random.Range(0.4f, 0.5f), fine, fineColor, CarMessage);
                    break;

                case eVoilation.OTHER_NO_SEAT_BELT:
                    path = pathManager.GetRandomPath();
                    car.StartMoving(path, Random.Range(0.4f, 0.5f), fine, fineColor, CarMessage);
                    break;

                case eVoilation.PARKING_ILLEGAL:
                    path = pathManager.GetRandomParkingPath();
                    car.ParkedCar(path, fine, fineColor, CarMessage);
                    break;

                case eVoilation.PARKING_HARD_SHOULDER:
                    path = pathManager.GetRandomParkingPath();
                    car.ParkedCar(path, fine, fineColor, CarMessage);
                    break;
            }

            GameObject spawnedVoilationCard = GameObject.Instantiate(voilationPrefab);
            spawnedVoilationCard.transform.SetParent(voilationCardHolder);
            spawnedVoilationCard.transform.localScale = Vector3.one;
            spawnedVoilationCard.transform.SetAsFirstSibling();

            gibberishImage.sprite = gibberishSprites[Random.Range(0, gibberishSprites.Length)];
            gibberishImage.color = Color.white;
            spawnedCars.Add(new VoilationCarMap(car, spawnedVoilationCard));

            crackle.transform.DOKill();
            crackle.transform.DOBlendableScaleBy(new Vector3(0f, -0.5f, 0f), 0.1f).SetEase(Ease.OutBounce)
                .SetLoops(-1, LoopType.Yoyo);

            micAudioSource.clip = micAudioClip;
            micAudioSource.Play();

            float waitTime = micAudioClip.length;

            if (sfxAudioClip != null)
            {
                AudioManager.instance.PlaySound(sfxAudioClip, 0.4f);

                if (sfxAudioClip.length > waitTime)
                {
                    waitTime = sfxAudioClip.length;
                }
            }

            yield return new WaitForSeconds(waitTime);

            crackle.transform.DOKill();
            crackle.transform.localScale = Vector3.one;

            yield return new WaitForSeconds(voilationMap.voilationDelay);
        }

        spawningLevel = false;
    }

    void SetCurrentLevel()
    {
        currentLevel = levelManager.levels[currentLevelIndex - 1]; 

        spawnedCars.Clear();

        StartCoroutine(SpawnVoilations());
    }

    void DrawRayFromTower()
    {
        rayFromTower.SetPosition(0, currentSelectedTower.transform.position);
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f;

        float currentLength = (pos - currentSelectedTower.transform.position).magnitude;

        if(currentLength > 1f)
        {
            pos = currentSelectedTower.transform.position +
                          (pos - currentSelectedTower.transform.position).normalized * 1f;
        }

        rayFromTower.SetPosition(1, pos);
    }

    public void CarMessage(int message, Car car)
    {
        switch (message)
        {
            case 1:
                ReleaseCar(car);
                AudioManager.instance.PlaySound(voilatorEliminated, 0.4f);
                break;

            case 2:
                AssignNewPath(car); 
                break;
        }
    }

    void AssignNewPath(Car car)
    {
        DOTweenPath path = pathManager.GetRandomPath();
        car.AssignPath(path);
    }

    void ReleaseCar(Car car)
    {
        foreach (VoilationCarMap voilationCarMap in spawnedCars)
        {
            if (voilationCarMap.car.gameObject.GetInstanceID() == car.gameObject.GetInstanceID())
            {
                GameObject spawnedCardInstance = voilationCarMap.voilationCard;

                if (spawnedCardInstance != null)
                {
                    GameObject.Instantiate(cashStackPrefab, car.transform.position, Quaternion.identity);

                    GameObject.Destroy(spawnedCardInstance);

                    return;
                }
            }
        }
    }

    void ManageLevel()
    {
        foreach (VoilationCarMap voilationCarMap in spawnedCars)
        {
            if (!voilationCarMap.car.isFree)
            {
                return;
            }
        }

        LevelComplete();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }

        if ( gameState == eGameState.MainMenu ||
            gameState == eGameState.GameOver )
        {
            return;
        }

        if (gameState == eGameState.AutoToggle)
        {
            if (pressOne && Input.anyKeyDown)
            {
                string inputString = Input.inputString.ToLower();

                int value;
                if (int.TryParse(inputString, out value))
                {
                    currentFine = value * 100;
                }
            }

            return;
        }

        Vector3 viewPointPosition = (Camera.main.ScreenToViewportPoint(Input.mousePosition));

        if (!Cursor.visible)
        {
            if (viewPointPosition.x < 0.16f || viewPointPosition.x > 0.85f)
            {
                Cursor.visible = true;
                laserLoopAudioSource.Stop();
                rayFromTower.gameObject.SetActive(false);
            }
        }
        else 
        {
            if (viewPointPosition.x > 0.16f && viewPointPosition.x < 0.85f)
            {
                Cursor.visible = false;
                laserLoopAudioSource.Play();
                rayFromTower.gameObject.SetActive(true);
            }
        }

        if (!spawningLevel)
        {
            ManageLevel();
        }

        if (!Cursor.visible)
        {
            DrawRayFromTower();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }

        if (Input.anyKeyDown)
        {
            string inputString = Input.inputString.ToLower();

            int value;
            if(int.TryParse(inputString, out value))
            {
                currentFine = value * 100;
                currentFine = currentFine == 0 ? 1000 : currentFine;

                int x = 1;
                int y = 3;
                if (value > 0)
                {
                    x = (value - 1) % 3;
                    y = (value - 1) / 3;
                }

                Vector2 moveOffset = selectorMoveOffset;
                moveOffset.x *= x;
                moveOffset.y *= y;

                Vector2 currentPos = selectorStart + moveOffset;

                fineSelector.GetComponent<RectTransform>().anchoredPosition = currentPos;
            }
            else
            {
                switch (inputString)
                {
                    case " ":
                        Fire();
                        break;

                    case "":
                        break;

                    default:
                        Tower newTower = currentSelectedTower.GetTower(inputString);
                        if (newTower != null)
                        {
                            currentSelectedTower.DeselectTower();
                            currentSelectedTower = newTower;
                            currentSelectedTower.SelectTower();
                        }
                        break;
                }
            }

            SetCurrentFine();
        }
    }
}
