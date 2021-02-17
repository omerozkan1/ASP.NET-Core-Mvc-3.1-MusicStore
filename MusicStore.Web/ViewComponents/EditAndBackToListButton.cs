using Microsoft.AspNetCore.Mvc;

namespace MusicStore.Web.ViewComponents
{
    public class EditAndBackToListButton : ViewComponent
    {
        public IViewComponentResult Invoke(int id)
        {
            return View(id);
        }
    }
}
