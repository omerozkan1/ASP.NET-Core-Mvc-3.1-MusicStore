using Microsoft.AspNetCore.Mvc;

namespace MusicStore.Web.ViewComponents
{
    public class CreateAndBackToListButton : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
