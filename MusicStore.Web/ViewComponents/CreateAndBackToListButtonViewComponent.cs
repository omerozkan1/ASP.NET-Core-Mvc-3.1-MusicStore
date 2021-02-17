using Microsoft.AspNetCore.Mvc;

namespace MusicStore.Web.ViewComponents
{
    public class CreateAndBackToListButtonViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
