using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public sealed class EntityController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rigid2D;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float depthSpeed = 0.5f;

    [Header("Stats")]
    [SerializeField] private int maxLifePoint = 20;
    [SerializeField] private int lifePoint = 20;
    [SerializeField] private DamageEvent OnDamageTaken;
    [SerializeField] private UnityEvent OnDeath;
    [SerializeField] private EntityEvent OnCollision;

    [Header("Weapon")]
    [SerializeField] private Weapon equipedWeapon;
    [SerializeField] private WeaponEvent OnWeaponChanged;
    [SerializeField] private LayerMask damageableLayers;
    [SerializeField] private Transform attackTransform;

    //Controls
    private Vector2 movement;
    private bool commandAttack = false;

    //Visual Values
    private bool isLookingRight = true;
    private float depth = 0f;

    //Running Values
    private Vector2 velocityAcceleration = Vector2.zero;
    private float depthAcceleration = 0f;

    private float targetVerticalMovement;
    private float targetDepthMovement;

    private float timeUntilAttacking = 0f;

    //Constants
    private const float smoothTime = 0.025f;
    private const float depthLimit = 3f;

    #region Callbacks
    public void RegisterOnDeath(UnityAction onDeathAction)
    {
        if(onDeathAction != null)
            OnDeath.AddListener(onDeathAction);
    }

    public void UnregisterOnDeath(UnityAction onDeathAction)
    {
        if (onDeathAction != null)
            OnDeath.RemoveListener(onDeathAction);
    }

    public void RegisterOnDamageTaken(UnityAction<Weapon, int, int> onDamageTakenAction)
    {
        if (onDamageTakenAction != null)
            OnDamageTaken.AddListener(onDamageTakenAction);
    }

    public void UnregisterOnDamageTaken(UnityAction<Weapon, int, int> onDamageTakenAction)
    {
        if (onDamageTakenAction != null)
            OnDamageTaken.RemoveListener(onDamageTakenAction);
    }

    public void RegisterOnWeaponChanged(UnityAction<Weapon> onWeaponChangedAction)
    {
        if (onWeaponChangedAction != null)
            OnWeaponChanged.AddListener(onWeaponChangedAction);
    }

    public void UnregisterOnWeaponChanged(UnityAction<Weapon> onWeaponChangedAction)
    {
        if (onWeaponChangedAction != null)
            OnWeaponChanged.RemoveListener(onWeaponChangedAction);
    }

    public void RegisterOnCollision(UnityAction<EntityController> onCollisionAction)
    {
        if (onCollisionAction != null)
            OnCollision.AddListener(onCollisionAction);
    }

    public void UnregisterOnCollision(UnityAction<EntityController> onCollisionAction)
    {
        if (onCollisionAction != null)
            OnCollision.RemoveListener(onCollisionAction);
    }
    #endregion

    #region Controls
    /// <summary>
    /// What is the command given to the entity?
    /// </summary>
    public void SetMovement(Vector2 movement)
    {
        this.movement = movement;
    }

    /// <summary>
    /// Should the entity try to attack?
    /// </summary>
    public void SetCommandAttack(bool commandAttack)
    {
        this.commandAttack = commandAttack;
    }
    #endregion

    #region Updates
    private void FixedUpdate()
    {
        TranscriptCommandIntoMovement();
        UpdateVelocity();
        UpdateVisual();
    }

    private void Update()
    {
        AttackIfWanted();
    }

    private void TranscriptCommandIntoMovement()
    {
        //TODO : Ajouter un movement vertical pour la profondeur

        //We translate the command given to actual velocity for the display
        this.targetVerticalMovement = this.movement.x * moveSpeed;
        this.targetDepthMovement = Mathf.Clamp(this.targetDepthMovement + this.movement.y * depthSpeed * Time.fixedDeltaTime, 0, depthLimit);
    }

    private void UpdateVelocity()
    {
        //Horizontal Movement
        this.rigid2D.velocity = Vector2.SmoothDamp(this.rigid2D.velocity, Vector2.right * targetVerticalMovement, ref velocityAcceleration, smoothTime);

        //Depth Movement
        Vector3 position = this.transform.position;
        position.z = Mathf.SmoothDamp(position.z, targetDepthMovement, ref depthAcceleration, smoothTime);
        this.transform.position = position;
        this.depth = position.z;
    }

    private void UpdateVisual()
    {
        //If the direction of movement change we flip the object to make the sprite
        //look the correct direction
        if ((this.rigid2D.velocity.x > 0 && !isLookingRight)
            || (this.rigid2D.velocity.x < 0 && isLookingRight))
        {
            FlipObject();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //When colliding
        if(collision.transform != null)
        {
            //If there is an entityController
            EntityController entityController = collision.transform.GetComponent<EntityController>();
            if(entityController != null)
            {
                if (entityController.GetRoundedDepth() == this.GetRoundedDepth())
                {
                    OnCollision?.Invoke(entityController);
                }
            }
        }
    }

    private void FlipObject()
    {
        //We juste have to invert xScale to make it flip
        Vector3 scale = this.transform.localScale;
        scale.x *= -1;
        this.transform.localScale = scale;

        //We invert the bool to make sure it goes according to the visual
        this.isLookingRight = !this.isLookingRight;
    }

    private void AttackIfWanted()
    {
        //Reducing the cooldown
        if(this.timeUntilAttacking > 0f)
            this.timeUntilAttacking -= Time.deltaTime;

        //If it's given order to attack
        if (this.commandAttack)
        {
            //Verifying if a weapon is equiped
            if (this.equipedWeapon != null)
            {
                //And if it recover from last time we attacked
                if (this.timeUntilAttacking <= 0f)
                {
                    //Searching every object at range
                    Collider2D[] entitiesAffected =
                        Physics2D.OverlapCircleAll(this.attackTransform.position, this.equipedWeapon.range,
                        this.damageableLayers);

                    for (int i = 0; i < entitiesAffected.Length; i++)
                    {
                        EntityController entity = entitiesAffected[i].GetComponent<EntityController>();
                        //Checking if there is an EntityController attached
                        if (entity != null)
                        {
                            //Checking it's on the same depth as the enemy
                            if (entity.GetRoundedDepth() == this.GetRoundedDepth())
                            {
                                entity.ApplyDamage(this.equipedWeapon);
                            }
                        }
                    }
                    //Reseting the timer
                    this.timeUntilAttacking = this.equipedWeapon.timeBetweenAttack;

                    //Reseting the commandAttack
                    this.commandAttack = false;
                }
            }
        }
    }
    #endregion

    #region Setters
    /// <summary>
    /// Set the new equiped weapon
    /// </summary>
    /// <param name="weapon">New equiped weapon</param>
    public void SetWeapon(Weapon weapon)
    {
        this.equipedWeapon = weapon;
        this.timeUntilAttacking = weapon.timeBetweenAttack;

        OnWeaponChanged?.Invoke(weapon);
    }
    #endregion

    #region Getters
    /// <summary>
    /// Is the character looking right?
    /// </summary>
    /// <returns>true/false</returns>
    public bool IsLookingRight()
    {
        return this.isLookingRight;
    }

    /// <summary>
    /// Get current life point
    /// </summary>
    public int GetLifePoint()
    {
        return this.lifePoint;
    }

    /// <summary>
    /// Get max life point
    /// </summary>
    public int GetMaxLifePoint()
    {
        return this.maxLifePoint;
    }
    
    /// <summary>
    /// Get max life point
    /// </summary>
    public float GetPercentLifePoint()
    {
        return (float)this.lifePoint / maxLifePoint;
    }

    /// <summary>
    /// Get the depth of the entity (rounded)
    /// </summary>
    /// <returns>Current depth of the entity (rounded)</returns>
    public int GetRoundedDepth()
    {
        return Mathf.RoundToInt(this.depth);
    }

    /// <summary>
    /// Get the depth of the entity
    /// </summary>
    /// <returns>Current depth of the entity</returns>
    public float GetDepth()
    {
        return this.depth;
    }

    /// <summary>
    /// Get time before being able to attack
    /// </summary>
    public float GetTimeUntilAttacking()
    {
        return this.timeUntilAttacking;
    }

    /// <summary>
    /// Get the percentage of the attack timing (time remaining / time needed)
    /// </summary>
    public float GetPercentAttackTime()
    {
        if(this.equipedWeapon == null)
        {
            return 0f;
        }
        if(this.equipedWeapon.timeBetweenAttack == 0f)
        {
            return 1f;
        }
        return this.timeUntilAttacking / this.equipedWeapon.timeBetweenAttack;
    }

    /// <summary>
    /// Get current equiped weapon
    /// </summary>
    public Weapon GetEquipedWeapon()
    {
        return this.equipedWeapon;
    }
    #endregion

    #region Others
    /// <summary>
    /// Apply damage to the entity.
    /// Call OnDamageTaken(life, damage).
    /// If the health drop bellow 0 call OnDeath;
    /// </summary>
    /// <param name="damage">Damage taken</param>
    public void ApplyDamage(int damage)
    {
        this.lifePoint -= damage;
        OnDamageTaken?.Invoke(null, this.lifePoint, damage);
        if(this.lifePoint <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    /// <summary>
    /// Apply damage to the entity.
    /// Call OnDamageTaken(life, damage).
    /// If the health drop bellow 0 call OnDeath;
    /// </summary>
    /// <param name="damage">Damage taken</param>
    public void ApplyDamage(Weapon weapon)
    {
        this.lifePoint -= weapon.damage;
        OnDamageTaken?.Invoke(weapon, this.lifePoint, weapon.damage);
        if (this.lifePoint <= 0)
        {
            OnDeath?.Invoke();
        }
    }
    #endregion
}

#region Events
[Serializable] public class WeaponEvent : UnityEvent<Weapon> { }

[Serializable] public class EntityEvent : UnityEvent<EntityController> { }

[Serializable] public class DamageEvent : UnityEvent<Weapon, int, int> { }
#endregion
