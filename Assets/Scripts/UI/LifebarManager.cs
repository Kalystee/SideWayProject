using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifebarManager : MonoBehaviour
{
    [Header("Player Lifebar")]
    [SerializeField] private Image playerCharacterPicture;
    [SerializeField] private Image playerLifebar;
    [SerializeField] private Image playerAttackbar;
    [SerializeField] private EntityController playerEntity;

    [Header("Enemy Lifebar")]
    [SerializeField] private GameObject enemyLifeBarGameObject;
    [SerializeField] private Image enemyCharacterPicture;
    [SerializeField] private Image enemyLifebar;
    [SerializeField] private EntityController enemyEntity;

    private float timeSinceLastHit = 0f;
    [SerializeField] private float timeBeforeHidingEnemyBar = 5f;

    public static LifebarManager Instance;
    private const float displaySpeed = 1.5f;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("Duplicates of LifebarManager! Disabling duplicate!", this);
            this.enabled = false;
        }
        Instance = this;
    }

    private void Start()
    {
        enemyLifeBarGameObject.SetActive(false);
    }

    public void SetEnemyFocused(EntityController entityController)
    {
        if (this.enemyEntity != entityController)
        {
            this.enemyEntity = entityController;
            this.enemyLifeBarGameObject.SetActive(true);
            this.timeSinceLastHit = 0f;
            this.enemyLifebar.fillAmount = this.enemyEntity.GetPercentLifePoint();
        }
    }

    private void Update()
    {
        float targetPlayerLifeBarPercent = this.playerEntity.GetPercentLifePoint();
        if (this.playerLifebar.fillAmount != targetPlayerLifeBarPercent)
        {
            this.playerLifebar.fillAmount = Mathf.Lerp(this.playerLifebar.fillAmount, targetPlayerLifeBarPercent, displaySpeed);
        }
        
        this.playerAttackbar.fillAmount = 1f - this.playerEntity.GetPercentAttackTime();

        if (enemyEntity != null)
        {
            float targetEnemyLifeBarPercent = this.enemyEntity.GetPercentLifePoint();
            if (this.enemyLifebar.fillAmount != targetEnemyLifeBarPercent)
            {
                this.enemyLifebar.fillAmount = Mathf.Lerp(this.enemyLifebar.fillAmount, targetEnemyLifeBarPercent, displaySpeed);
            }
        }

        this.timeSinceLastHit += Time.deltaTime;
        if(timeSinceLastHit >= timeBeforeHidingEnemyBar)
        {
            this.enemyEntity = null;
            this.enemyLifeBarGameObject.SetActive(false);
        }
    }
}
