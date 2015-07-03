using System.Collections.Generic;
using System.Globalization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace MultiDates.Settings
{
    public class DateRangeFieldEditorEvents : ContentDefinitionEditorEventsBase
    {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition)
        {
            if (definition.FieldDefinition.Name == "DateRangeField")
            {
                var model = definition.Settings.GetModel<DateRangeFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel)
        {
            if (builder.FieldType != "DateRangeField")
            {
                yield break;
            }

            var model = new DateRangeFieldSettings();
            if (updateModel.TryUpdateModel(model, "DateRangeFieldSettings", null, null))
            {
                builder.WithSetting("DateRangeFieldSettings.Hint", model.Hint);
                builder.WithSetting("DateRangeFieldSettings.Required", model.Required.ToString(CultureInfo.InvariantCulture));

                yield return DefinitionTemplate(model);
            }
        }
    }
}