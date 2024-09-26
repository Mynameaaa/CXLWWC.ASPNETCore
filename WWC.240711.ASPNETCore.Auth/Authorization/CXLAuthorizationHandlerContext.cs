using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WWC._240711.ASPNETCore.Auth;

//
// 摘要:
//     Contains authorization information used by Microsoft.AspNetCore.Authorization.IAuthorizationHandler.
public class CXLAuthorizationHandlerContext
{

    private readonly HashSet<IAuthorizationRequirement> _pendingRequirements;

    private List<AuthorizationFailureReason> _failedReasons;

    private bool _failCalled;

    private bool _succeedCalled;

    //
    // 摘要:
    //     The collection of all the Microsoft.AspNetCore.Authorization.IAuthorizationRequirement
    //     for the current authorization action.
    public virtual IEnumerable<IAuthorizationRequirement> Requirements { get; }

    //
    // 摘要:
    //     The System.Security.Claims.ClaimsPrincipal representing the current user.
    public virtual ClaimsPrincipal User { get; }

    //
    // 摘要:
    //     The optional resource to evaluate the Microsoft.AspNetCore.Authorization.AuthorizationHandlerContext.Requirements
    //     against.
    public virtual object? Resource { get; }

    //
    // 摘要:
    //     Gets the requirements that have not yet been marked as succeeded.
    public virtual IEnumerable<IAuthorizationRequirement> PendingRequirements => _pendingRequirements;

    //
    // 摘要:
    //     Gets the reasons why authorization has failed.
    public virtual IEnumerable<AuthorizationFailureReason> FailureReasons
    {
        get
        {
            IEnumerable<AuthorizationFailureReason> failedReasons = _failedReasons;
            return failedReasons ?? Array.Empty<AuthorizationFailureReason>();
        }
    }

    //
    // 摘要:
    //     Flag indicating whether the current authorization processing has failed due to
    //     Fail being called.
    public virtual bool HasFailed => _failCalled;

    //
    // 摘要:
    //     Flag indicating whether the current authorization processing has succeeded.
    public virtual bool HasSucceeded
    {
        get
        {
            if (!_failCalled && _succeedCalled)
            {
                return !PendingRequirements.Any();
            }

            return false;
        }
    }

    public CXLAuthorizationHandlerContext(IEnumerable<IAuthorizationRequirement> requirements, ClaimsPrincipal user, object? resource)
    {
        Requirements = requirements;
        User = user;
        Resource = resource;
        _pendingRequirements = new HashSet<IAuthorizationRequirement>(requirements);
    }

    public virtual void Fail()
    {
        _failCalled = true;
    }

    public virtual void Fail(AuthorizationFailureReason reason)
    {
        Fail();
        if (reason != null)
        {
            if (_failedReasons == null)
            {
                _failedReasons = new List<AuthorizationFailureReason>();
            }

            _failedReasons.Add(reason);
        }
    }

    public virtual void Succeed(IAuthorizationRequirement requirement)
    {
        _succeedCalled = true;
        _pendingRequirements.Remove(requirement);
    }
}