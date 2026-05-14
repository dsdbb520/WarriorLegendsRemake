using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Centralized action permission system using a named-lock model.
///
/// Usage:
///   Open a panel  → LockActions("myPanel", "myPanel")  (only "myPanel" action stays on)
///   Close a panel → UnlockActions("myPanel")
///   Block all     → LockActions("dialogue")
///   Unblock all   → UnlockActions("dialogue")
///
/// An action is allowed only when NO active lock blocks it.
/// Multiple systems can each hold their own independent lock; actions re-enable
/// automatically once the last lock that blocks them is released — no save/restore needed.
/// </summary>
public class PlayerActionManager : SingletonMono<PlayerActionManager>
{
    // owner string → set of actions allowed while this lock is held (empty = nothing allowed)
    private readonly Dictionary<string, HashSet<string>> _locks = new Dictionary<string, HashSet<string>>();

    private static readonly string[] AllActions =
        { "move", "jump", "attack", "interact", "task", "backpack", "dodge" };

    // ── Computed action states ──────────────────────────────────────────────
    public bool canMove     => IsAllowed("move");
    public bool canJump     => IsAllowed("jump");
    public bool canAttack   => IsAllowed("attack");
    public bool canInteract => IsAllowed("interact");
    public bool canTask     => IsAllowed("task");
    public bool canBackpack => IsAllowed("backpack");
    public bool canDodge    => IsAllowed("dodge");

    private bool IsAllowed(string action)
    {
        foreach (var kv in _locks)
            if (!kv.Value.Contains(action)) return false;
        return true;
    }

    // ── Lock API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Holds a named lock that blocks all actions except those listed in allowedActions.
    /// Calling again with the same owner updates the lock in place.
    /// </summary>
    public void LockActions(string owner, params string[] allowedActions)
    {
        _locks[owner] = new HashSet<string>(allowedActions);
    }

    /// <summary>Releases the named lock. Actions re-enable when no locks remain.</summary>
    public void UnlockActions(string owner)
    {
        _locks.Remove(owner);
    }

    /// <summary>Clears every active lock. Call on scene load to prevent stale locks.</summary>
    public void ResetAllLocks() => _locks.Clear();

    // ── Legacy API (backward compatible) ───────────────────────────────────

    public void DisableAll()                     => LockActions("__global__");
    public void EnableAll()                      => UnlockActions("__global__");
    public void EnableOnlyAction(string action)  => LockActions("__global__", action);

    // ── Timed per-action disable ────────────────────────────────────────────

    private readonly Dictionary<string, Coroutine> _tempCoroutines = new Dictionary<string, Coroutine>();

    public void DisableActionTemporary(string actionName, float duration)
    {
        if (_tempCoroutines.TryGetValue(actionName, out var existing) && existing != null)
            StopCoroutine(existing);

        _tempCoroutines[actionName] = StartCoroutine(TempDisable(actionName, duration));
    }

    private IEnumerator TempDisable(string actionName, float duration)
    {
        string owner = "temp_" + actionName;
        var allowed = new List<string>(AllActions);
        allowed.Remove(actionName);
        LockActions(owner, allowed.ToArray());

        yield return new WaitForSeconds(duration);

        UnlockActions(owner);
        _tempCoroutines.Remove(actionName);
    }
}
