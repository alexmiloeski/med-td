using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum SelectedAction { Nothing, BuildTower1, BuildTower2, BuildTower3, BuildTower4, SellTower, UpgradeTower, SetRallyPoint };

public class Shop : MonoBehaviour
{
    public static Shop instance;

    public Tower tower1;
    public Tower tower2;
    public Tower tower3;
    public Tower tower4;

    public float coughCooldown = 20f;
    private float coughCountdown = 0f;
    private bool coughing = false;

    // needed for changing button appearance when "selected"
    public Sprite spriteButtonRegular;
    public Sprite spriteCheckmark;
    public Sprite spriteButtonX;
    
    public float coughStopDelay = 6f;

    public float shakeMultiplier = 2f;

    /// <summary> Reference(s) to a button's state before it's been clicked and changed to "selected";
    /// Used for changing it back to its original state when another button becomes "selected" </summary>
    private GameObject tempButton;
//#pragma warning disable CS0169 // The field 'Shop.tempButtonText' is never used
    //private string tempButtonText;
//#pragma warning restore CS0169 // The field 'Shop.tempButtonText' is never used
    private Sprite tempButtonSprite;

    public static GameObject infoPanel;

    private void Awake()
    {
        // initialize an instance of this singleton for use in other classes
        if (instance != null)
        {
            Debug.Log("More than one Shop in scene!");
            return;
        }
        instance = this;
    }

    private void Update()
    {
        if (coughCountdown > 0f)
        {
            coughCountdown -= Time.deltaTime;
            UIManager.instance.SetInteractableButtonSpecial1(false, coughCountdown);
        }
        else if (WaveSpawner.instance.IsLevelStarted())
        {
            // this is fine: this method doesn't update the button if it'is already interactable
            UIManager.instance.SetInteractableButtonSpecial1(true, 0f);
        }
    }

    public void ButtonPauseOrResumeAction()
    {
        //Debug.Log("ButtonPauseOrResume pressed");

        BuildManager.instance.DeselectLymphNode();

        GameManager.instance.TogglePauseGame();
    }
    public void ButtonNextOrRestartLevelAction()
    {
        //Debug.Log("ButtonNextOrRestartLevel pressed");
        GameManager.instance.NextOrRestartLevel();
    }
    public void ButtonBottomCenterAction()
    {
        //if (Scroller.instance.IsDragging()) return; // don't do button action while scrolling
        if (Scroller.IsDragging()) return; // don't do button action while scrolling

        //if (GameManager.instance.IsSettingRallyPoint()) return; // don't do button action if setting rally point on a melee tower
        GameManager.instance.StopSettingRallyPoint();

        BuildManager buildManager = BuildManager.instance;

        buildManager.DeselectLymphNode();

        if (buildManager.IsFinishedWithSS())
        {
            if (WaveSpawner.instance.IsFinishedSpawning()) return;

            // if waves are already spawning, spawn next wave early
            if (WaveSpawner.instance.IsLevelStarted())
            {
                WaveSpawner.instance.EarlyNextWave();
            }
            else // else, start spawning waves
            {
                // start spawning waves of enemies
                WaveSpawner.instance.StartLevel();

                // show UI elements that should be visible once the waves have started
                UIManager.instance.SetEnabledButtonBottomCenter(false);
                UIManager.instance.ShowPostSSUIElements();
            }
        }
        else
        {
            buildManager.FinishWithSS();
        }
    }
    public void ButtonBuildTower1Action(GameObject buttonObject)
    {
        ButtonAction(buttonObject, SelectedAction.BuildTower1, tower1);
    }
    public void ButtonBuildTower2Action(GameObject buttonObject)
    {
        ButtonAction(buttonObject, SelectedAction.BuildTower2, tower2);
    }
    public void ButtonBuildTower3Action(GameObject buttonObject)
    {
        ButtonAction(buttonObject, SelectedAction.BuildTower3, tower3);
    }
    public void ButtonBuildTower4Action(GameObject buttonObject)
    {
        ButtonAction(buttonObject, SelectedAction.BuildTower4, tower4);
    }
    public void ButtonSellTowerAction(GameObject buttonObject)
    {
        ButtonAction(buttonObject, SelectedAction.SellTower, null);
    }
    public void ButtonUpgradeTowerAction(GameObject buttonObject)
    {
        ButtonAction(buttonObject, SelectedAction.UpgradeTower, null);
    }
    public void ButtonSetRallyPointAction()
    {
        ButtonAction(null, SelectedAction.SetRallyPoint, null);
    }
    public void ButtonSpecial1Action()
    {
        //Debug.Log("ButtonSpecial1Action");

        //if (GameManager.instance.IsSettingRallyPoint()) return; // don't do button action if setting rally point on a melee tower
        GameManager.instance.StopSettingRallyPoint();

        BuildManager.instance.DeselectLymphNode();

        UIManager.instance.SetInteractableButtonSpecial1(false, coughCooldown);
        coughCountdown = coughCooldown;
        coughing = true;

        // cough
        // shake camera
        float duration1 = 0.3f;
        float delay2 = 0.3f;
        float duration2 = 0.2f;
        float delay3 = 0.8f;
        float duration3 = 0.3f;
        float delay4 = 1f;
        float lastDuration = 0.1f;
        StartCoroutine(ShakeCamera(0f, duration1, 0.08f));
        StartCoroutine(ShakeCamera(delay2, duration2, 0.12f));
        StartCoroutine(ShakeCamera(delay3, duration3, 0.06f));
        StartCoroutine(ShakeCamera(delay4, lastDuration, 0.02f));

        float totalCoughDuration = delay4 + lastDuration;

        // slow down all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Constants.EnemyTag);
        foreach (GameObject enemyObj in enemies)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null) enemy.StartCough(coughStopDelay);
        }

        // reenable AP pulse animation
        // todo: reenable AP pulse animation

        Invoke("StopCough", coughStopDelay);
        Invoke("StopCameraShaker", totalCoughDuration);
    }
    private IEnumerator ShakeCamera(float delay, float duration, float intensity)
    {
        yield return new WaitForSeconds(delay);
        CameraShaker.StartShaking(duration, intensity *= shakeMultiplier);
    }
    private void StopCough()
    {

        // return all enemies' speeds to their regular speed
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Constants.EnemyTag);
        foreach (GameObject enemyObj in enemies)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null) enemy.StopCough();
        }

        // reenable AP pulse animation
        // todo: reenable AP pulse animation

        coughing = false;
    }
    private void StopCameraShaker()
    {
        // stop the camera shaker
        CameraShaker.StopShaking();
    }
    internal bool IsCoughing()
    {
        return coughing;
    }

    private void SetButtonAsSelected(GameObject buttonObject, bool possible)
    {
        if (buttonObject == null) return;

        //Image image = buttonObject.GetComponent<Image>();
        Transform iconT = buttonObject.transform.Find(Constants.ButtonIcon);
        if (iconT != null)
        {
            Image image = iconT.GetComponent<Image>();
            
            //Text text = buttonObject.transform.GetChild(0).GetComponent<Text>();

            // save this button's default values
            tempButton = buttonObject;
            if (image != null) tempButtonSprite = image.sprite;
            //if (text != null) tempButtonText = text.text;

            // change this button's values so as to appear "selected"
            if (image != null)
            {
                if (possible)
                    image.sprite = spriteCheckmark;
                else
                    image.sprite = spriteButtonX;
            }
            //if (text != null) text.text = "";
        }
    }
    private void ResetPreviouslySelectedButton()
    {
        if (tempButton != null)
        {
            Transform iconT = tempButton.transform.Find(Constants.ButtonIcon);
            if (iconT != null)
            {
                Image image = iconT.GetComponent<Image>();

                if (image != null)
                    image.sprite = tempButtonSprite;
                //if (tempButton.transform.GetChild(0) != null && tempButton.transform.GetChild(0).GetComponent<Text>() != null)
                //    tempButton.transform.GetChild(0).GetComponent<Text>().text = tempButtonText;
            }
        }
    }
    private void ButtonAction(GameObject buttonObject, SelectedAction sa, Tower tower)
    {
        if (Scroller.IsDragging()) return; // don't do button action while scrolling

        // destroy the info panel, if it was present
        if (infoPanel != null)
        {
            Destroy(infoPanel);
        }

        BuildManager buildManager = BuildManager.instance;
        
        if (sa == SelectedAction.SetRallyPoint)
        {
            buildManager.StartRallyPointSelector();
            // destroy the menu; deselecting the lymph node also destroys the menu, so that works fine
            // todo: if a design decision is made to not deselect the node after starting the action,
            // then this has to be redone, because not destroying the menu leaves the selected button
            buildManager.DeselectLymphNode();
            return;
        }

        if (buildManager.GetSelectedAction() == sa) // if this button is already selected, do the action
        {
            tempButton = null;

            buildManager.DoSelectedAction(tower);
            // destroy the menu; deselecting the lymph node also destroys the menu, so that works fine
            // todo: if a design decision is made to not deselect the node after starting the action,
            // then this has to be redone, because not destroying the menu leaves the selected button
            buildManager.DeselectLymphNode();
        }
        else // if this button isn't selected, just select it (if the action is possible)
        {
            ResetPreviouslySelectedButton();
            bool enoughMoney = buildManager.EnoughMoneyForAction(sa, tower);
            SetButtonAsSelected(buttonObject, enoughMoney);
            if (enoughMoney)
            {
                buildManager.SelectAction(sa);
            }
            else
            {
                buildManager.SelectAction(SelectedAction.Nothing);
                UIManager.instance.FlashNotEnoughMoney(1f);
            }
            // show info panel when selecting a button
            infoPanel = UIManager.instance.ShowInfoPanel(buttonObject.transform.parent, sa, tower);
        }
    }
}
