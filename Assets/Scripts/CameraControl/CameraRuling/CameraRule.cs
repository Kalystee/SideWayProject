using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class CameraRule : MonoBehaviour
{
    protected CameraController CameraController { get { return CameraController.Instance; } }

    protected static List<CameraRule> listRules = new List<CameraRule>();

    [SerializeField] private CameraSubRule subRule;

    private bool initialized = false;

    [HideInInspector] private static CameraRule cur_Rule;
    public static CameraRule CurrentRule
    {
        get
        {
            return cur_Rule;
        }
        set
        {
            if (cur_Rule != value)
            {
                if (cur_Rule != null)
                {
                    cur_Rule.enabled = false;
                    cur_Rule.StopRule();
                    cur_Rule.subRule?.StopSubRule();
                }

                if(value != null)
                {
                    cur_Rule = value;
                    cur_Rule.enabled = true;
                    cur_Rule.ApplyRule();
                    cur_Rule.subRule?.ApplySubRule();
                }
                else
                {
                    cur_Rule = listRules.FirstOrDefault();
                    if(cur_Rule != null)
                        cur_Rule.enabled = true;
                }

            }
        }
    }

    [SerializeField] private int priority;
    
    private void OnEnable()
    {
        if (initialized)
        {
            CurrentRule = this;
            Debug.Log(initialized + " / " + gameObject.name);
        }
        else
        {
            int i = 0;
            while (i < listRules.Count && listRules[i].priority > this.priority)
            {
                i++;
            }
            listRules.Insert(i, this);
            if (i == 0)
            {
                CurrentRule = this;
            }
            else
            {
                this.enabled = false; 
            }
            initialized = true;
        }
    }

    private void OnDisable()
    {
        subRule?.StopSubRule();
        StopRule();

        if (listRules.Count > 0)
        {
            CameraRule highestPriorityRule = listRules.FirstOrDefault(cf => cf != this);
            CurrentRule = highestPriorityRule;
        }
        else
        {
            CurrentRule = null;
        }
    }

    private void OnDestroy()
    {
        listRules.Remove(this);
        
        if (this.enabled)
        {
            if (listRules.Count > 0)
            {
                CameraRule highestPriorityRule = listRules.FirstOrDefault(cf => cf != this);
                CurrentRule = highestPriorityRule;
            }
            else
            {
                CurrentRule = null;
            }
        }
    }

    private void Update()
    {
        subRule?.ApplySubRule();
        ApplyRule();
    }

    protected abstract void BeginRule();
    protected abstract void ApplyRule();
    protected abstract void StopRule();
}