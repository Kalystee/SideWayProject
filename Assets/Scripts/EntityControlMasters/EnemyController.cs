using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityControlMaster
{
    [Header("Score")]
    [SerializeField] private int killScoreValue;
    public EntityController target;
    public float moveSpeed = 1.3f;

    [Header("Particles")]
    public GameObject OnDeathParticlesPrefab;

    private void Start()
    {

        this.entityController.RegisterOnDeath(() => OnDeath());

        this.entityController.RegisterOnDamageTaken((weapon, health, damage) => Debug.Log($"Vie : {health} (-{damage}) | " + (weapon != null ? $"Source : {weapon.name}" : "Inconnue") , this));
        this.entityController.RegisterOnDamageTaken((weapon, health, damage) => LifebarManager.Instance.SetEnemyFocused(this.entityController));
    }

    private void Update()
    {
        CommandMovement();
        if(this.entityController.GetTimeUntilAttacking() <= 0)
        {
            CommandAttack();
        }
        
    }

    private void CommandMovement()
    {
        Vector2 direction = (target.transform.position - transform.position).normalized;
        direction.y = Mathf.Sign(target.GetDepth() - entityController.GetDepth());
        this.entityController.SetMovement(direction* moveSpeed);
        if (Vector2.Distance(target.transform.position, transform.position) < this.entityController.GetEquipedWeapon().range)
        {
            this.entityController.SetMovement(direction * 0);
        }
    }

    private void CommandAttack()
    {
        if(Vector2.Distance(target.transform.position, transform.position) <= this.entityController.GetEquipedWeapon().range)
        {
            this.entityController.SetCommandAttack(true);
            Debug.Log("Attack the player");
        }
    }

    public void OnDeath()
    {
        PlayerControler.Instance.AddScore(killScoreValue);
        GameObject clone = Instantiate(OnDeathParticlesPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
        Destroy(clone, 3f);
    }
}
