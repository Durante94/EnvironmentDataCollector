using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebEnvironmentDataCollector.Pages
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "WebAdmin")]
    public class ReservedModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
