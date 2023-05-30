using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Sras.PublicCoreflow.Web.Pages;

public class IndexModel : PublicCoreflowPageModel
{
    public void OnGet()
    {

    }

    public async Task OnPostLoginAsync()
    {
        await HttpContext.ChallengeAsync("oidc");
    }
}
