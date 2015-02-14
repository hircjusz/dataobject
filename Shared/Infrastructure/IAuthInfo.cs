namespace SoftwareMind.Base
{
    public interface IAuthInfo
    {
        long LoggedUserId { get; }
        long EffectiveUserId { get; }
        long ProfileId { get; }
        string UserApplication { get; }
        string UserLanguage { get; }
        string SessionId { get; }
    }
}