using System.Collections.Generic;
using System.Globalization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace MultiDates.Settings
{
    public class MultiDateFieldEditorEvents : ContentDefinitionEditorEventsBase
    {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition)
        {
            if (definition.FieldDefinition.Name == "MultiDateField" || definition.FieldDefinition.Name == "DateRangeField")
            {
                var model = definition.Settings.GetModel<MultiDateFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel)
        {
            if (builder.FieldType != "MultiDateField" || builder.FieldType != "DateRangeField")
            {
                yield break;
            }

            var model = new MultiDateFieldSettings();
            if (updateModel.TryUpdateModel(model, "DateFieldSettings", null, null))
            {
                builder.WithSetting("DateFieldSettings.Hint", model.Hint);
                builder.WithSetting("DateFieldSettings.Required", model.Required.ToString(CultureInfo.InvariantCulture));

                yield return DefinitionTemplate(model);
            }
        }
    }
}