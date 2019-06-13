using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControler : EntityControlMaster
{
    public static PlayerControler Instance;
   
    
    [Header("Weapons")]
    [SerializeField] private Weapon[] useableWeapons;
    [SerializeField] private KeyCode switchWeaponClockwise;
    [SerializeField] private KeyCode switchWeaponCounterClockwise;
    [SerializeField] private int currentlyHeldWeapon = 0;

    [Header("Score")]
    [SerializeField] private int score;

    [Header("UI")]
    public Text scoreTextUI;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("Duplicates of PlayerController! Disabling duplicate!", this);
            this.enabled = false;
        }
        Instance = this;
    }

    private void Start()
    {
        this.entityController.SetWeapon(this.useableWeapons[this.currentlyHeldWeapon]);
    }

    private void Update()
    {
        CommandMovement();
        CommandWeaponSwitching();
        CommandAttack();
    }

    private void CommandMovement()
    {
        this.entityController.SetMovement(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        //this.anim.Play("Walk");
    }

    private void CommandWeaponSwitching()
    {
        int oldHeldWeapon = this.currentlyHeldWeapon;

        if (Input.GetKeyDown(this.switchWeaponClockwise))
        {
            this.currentlyHeldWeapon++;
        }
        else if (Input.GetKeyDown(this.switchWeaponCounterClockwise))
        {
            this.currentlyHeldWeapon--;
        }
        
        if(this.currentlyHeldWeapon >= this.useableWeapons.Length)
        {
            this.currentlyHeldWeapon = 0;
        }
        else if(this.currentlyHeldWeapon < 0)
        {
            this.currentlyHeldWeapon = this.useableWeapons.Length - 1;
        }

        if (oldHeldWeapon != this.currentlyHeldWeapon)
        {
            this.entityController.SetWeapon(this.useableWeapons[this.currentlyHeldWeapon]);
        }
    }

    private void CommandAttack()
    {
        this.entityController.SetCommandAttack(Input.GetButtonDown("Fire1"));
    }

    public void AddScore(int scoredAdded)
    {
        this.score += scoredAdded;
        scoreTextUI.text = this.score.ToString();
    }
}
