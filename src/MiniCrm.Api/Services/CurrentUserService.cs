using System.Security.Claims;

public interface ICurrentUserService
{
    public Guid? UserId { get; }
}

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid? UserId
    {
        get
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? _httpContextAccessor.HttpContext?.Request.Headers["X-User-Id"].FirstOrDefault();
            return userIdStr is string sub && Guid.TryParse(sub, out var guid) ? guid : null;
        }
    }
}