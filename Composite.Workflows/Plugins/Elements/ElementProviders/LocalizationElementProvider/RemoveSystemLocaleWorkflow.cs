using System;
using System.Globalization;
using System.Workflow.Activities;
using Composite.C1Console.Actions;
using Composite.C1Console.Events;
using Composite.Data;
using Composite.Data.Types;
using Composite.Core.Localization;
using Composite.C1Console.Workflow;


namespace Composite.Plugins.Elements.ElementProviders.LocalizationElementProvider
{
    [EntityTokenLock()]
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public sealed partial class RemoveSystemLocaleWorkflow : Composite.C1Console.Workflow.Activities.FormsWorkflow
    {
        public RemoveSystemLocaleWorkflow()
        {
            InitializeComponent();
        }



        private CultureInfo CreateCultureInfo()
        {
            DataEntityToken dataEntityToken = (DataEntityToken)this.EntityToken;

            ISystemActiveLocale systemActiveLocale = (ISystemActiveLocale)dataEntityToken.Data;

            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(systemActiveLocale.CultureName);
            return cultureInfo;
        }



        private void IsLastLocale(object sender, ConditionalEventArgs e)
        {
            e.Result = LocalizationFacade.IsLastLocale();
        }



        private void IsTypesUsingLocalization(object sender, ConditionalEventArgs e)
        {            
            e.Result = LocalizationFacade.IsTypesUsingLocalization();
        }



        private void IsOnlyActiveLocaleForSomeUsers(object sender, ConditionalEventArgs e)
        {
            CultureInfo cultureInfo = CreateCultureInfo();            

            e.Result = LocalizationFacade.IsOnlyActiveLocaleForSomeUsers(cultureInfo);
        }



        private void IsCurrentDefaultLocale(object sender, ConditionalEventArgs e)
        {
            CultureInfo cultureInfo = CreateCultureInfo();

            e.Result = LocalizationFacade.IsDefaultLocale(cultureInfo);
        }



        private void finalizeCodeActivity_Finalize_ExecuteCode(object sender, EventArgs e)
        {
            DeleteTreeRefresher deleteTreeRefresher = this.CreateDeleteTreeRefresher(this.EntityToken);
            
            ISystemActiveLocale systemActiveLocale = this.GetDataItemFromEntityToken<ISystemActiveLocale>();

            LocalizationFacade.RemoveLocale(systemActiveLocale.CultureName);                       

            ConsoleMessageQueueFacade.Enqueue(new CollapseAndRefreshConsoleMessageQueueItem(), null);
            ConsoleMessageQueueFacade.Enqueue(new BroadcastMessageQueueItem { Name = "LocalesUpdated", Value = "" }, null);

            deleteTreeRefresher.PostRefreshMesseges();
        }
    }
}
