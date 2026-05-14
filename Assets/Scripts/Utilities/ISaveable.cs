/// <summary>
/// Implement this on any MonoBehaviour that owns persistent data.
/// SaveSystemJSON collects all registered saveables when saving/loading.
/// </summary>
public interface ISaveable
{
    /// <summary>Returns a serializable snapshot of this object's state.</summary>
    object CaptureState();

    /// <summary>Restores state from a previously captured snapshot.</summary>
    void RestoreState(object state);
}
