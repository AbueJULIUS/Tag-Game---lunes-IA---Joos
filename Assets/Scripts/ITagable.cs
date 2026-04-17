using UnityEngine;

public interface ITagable
{
    bool Tagged { get; }
    void ToggleTagged(bool state);
}
