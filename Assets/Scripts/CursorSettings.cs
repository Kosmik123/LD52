using UnityEngine;

[CreateAssetMenu]
public class CursorSettings : ScriptableObject
{
    [field: SerializeField]
    public Texture2D DefaultCursorIcon { get; private set; }

    [field: SerializeField]
    public Texture2D BuildCursorIcon { get; private set; }

    [field: SerializeField]
    public Texture2D DestroyCursorIcon { get; private set; }
}
