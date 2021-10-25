using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrchardCore.ContentManagement.Display.ContentDisplay
{
    public abstract class SingleViewModelContentPartDisplayDriver<TPart, TViewModel> : ContentPartDisplayDriver<TPart>
        where TPart : ContentPart, new()
        where TViewModel : class, new()
    {
        /// <summary>
        /// Performs the update activity and returns any model state errors.
        /// </summary>
        protected abstract Task<IEnumerable<(string Key, string Error)>> UpdateAsync(
            TPart part,
            TViewModel viewModel,
            UpdatePartEditorContext context);

        public override async Task<IDisplayResult> UpdateAsync(
            TPart part,
            IUpdateModel updater,
            UpdatePartEditorContext context)
        {
            var viewModel = new TViewModel();
            if (await updater.TryUpdateModelAsync(viewModel, Prefix))
            {
                foreach (var (key, error) in await UpdateAsync(part, viewModel, context))
                {
                    updater.ModelState.AddModelError(key, error);
                }
            }

            return await EditAsync(part, context);
        }
    }
}
