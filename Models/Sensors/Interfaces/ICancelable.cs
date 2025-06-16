namespace InterrogationGame.Models.Sensors;

public interface ICancellable
{
    void CancelAbility();
    bool IsAbilityCancelled { get; }
} 