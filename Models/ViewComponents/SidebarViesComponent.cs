using Microsoft.AspNetCore.Mvc;
using MyCompany.Domain;

namespace MyCompany.Models.ViewComponents
{
    public class SidebarViesComponent : ViewComponent
    {
        private readonly DataManager dataManager;

        public SidebarViesComponent(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult((IViewComponentResult) View("Default", dataManager.ServiceItems.GetServiceItems()));
        }
    }
}
