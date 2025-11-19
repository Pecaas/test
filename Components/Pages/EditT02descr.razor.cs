using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Medrec.Components.Pages
{
    public partial class EditT02descr
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }
        [Inject]
        public medrecService medrecService { get; set; }

        [Parameter]
        public int Serial { get; set; }

        protected override async Task OnInitializedAsync()
        {
            t02descr = await medrecService.GetT02descrBySerial(Serial);
        }
        protected bool errorVisible;
        protected Medrec.Models.medrec.T02descr t02descr;

        protected async Task FormSubmit()
        {
            try
            {
                await medrecService.UpdateT02descr(Serial, t02descr);
                DialogService.Close(t02descr);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}