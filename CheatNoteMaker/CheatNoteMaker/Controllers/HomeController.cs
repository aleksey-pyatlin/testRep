using System;
using System.Web.Mvc;

namespace CheatNoteMaker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Делаем шпоры!";

            return View();
        }

        public ActionResult Error(string errorId)
        {
            if (string.IsNullOrWhiteSpace(errorId))
            {
                ViewBag.ErrorMessage = "Что-то тут не так, а что - не понятно :(";
            }
            else
            {
                if (errorId.Equals("PermissionDenied", StringComparison.CurrentCultureIgnoreCase))
                {
                    ViewBag.ErrorMessage = "У Вас нет прав для выполнения этого действия. Обратитесь к администратору.";
                }

                if (errorId.Equals("VkAuthDenied", StringComparison.InvariantCultureIgnoreCase))
                {
                    ViewBag.ErrorMessage = "Не удалось войти через В Контакте.";
                }
            }

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
