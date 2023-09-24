using System;

public interface ICurtain
{
    void TriggerCurtain(bool activate, bool instantly, Action callback = null);
}