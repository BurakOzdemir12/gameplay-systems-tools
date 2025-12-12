using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    public List<Target> targets = new List<Target>();

    public Target SelectedTarget { get; private set; }
    [SerializeField] private CinemachineTargetGroup cmTargetGroup;
    [SerializeField] private float targetWeight = 1f;
    [SerializeField] private float targetRadius = 1f;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target)) return;
        if (!targets.Contains(target))
        {
            targets.Add(target);
            target.OnTargetDestroyed += RemoveTarget;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target)) return;
        if (targets.Contains(target))
        {
            RemoveTarget(target);
        }
    }

    public bool SelectTarget()
    {
        if (targets.Count == 0) return false;
        SelectedTarget = targets[0];
        if (cmTargetGroup == null)
        {
            Debug.LogError("Targeter has no CineMachine Target Group! You need to assing it");
        }

        cmTargetGroup.AddMember(SelectedTarget?.transform, targetWeight, targetRadius);

        return true;
    }

    public void DeselectTarget()
    {
        if (SelectedTarget == null) return;

        if (cmTargetGroup == null)
        {
            Debug.LogError("Targeter has no CineMachine Target Group! You need to assing it");
        }

        cmTargetGroup.RemoveMember(SelectedTarget?.transform);
        SelectedTarget = null;
    }

    private void RemoveTarget(Target target)
    {
        if (!targets.Contains(target)) return;
        if (SelectedTarget == target)
        {
            cmTargetGroup.RemoveMember(SelectedTarget?.transform);
            SelectedTarget = null;
        }

        target.OnTargetDestroyed -= RemoveTarget;
        targets.Remove(target);
    }
}