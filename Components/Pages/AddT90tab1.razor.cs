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
    public partial class AddT90tab1
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

        protected override async Task OnInitializedAsync()
        {
            t90tab1 = new Medrec.Models.medrec.T90tab1();
        }
        protected bool errorVisible;
        protected Medrec.Models.medrec.T90tab1 t90tab1;

        protected async Task FormSubmit()
        {
            try
            {
                await medrecService.CreateT90tab1(t90tab1);
                DialogService.Close(t90tab1);
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