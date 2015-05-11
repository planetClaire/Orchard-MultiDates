using System;
using System.Linq;
using MultiDates.Fields;
using MultiDates.Settings;
using MultiDates.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.Localization.Models;
using Orchard.Localization.Services;

namespace MultiDates.Drivers
{
    public class MultiDateFieldDriver: ContentFieldDriver<MultiDateField> {
        private const string TemplateName = "Fields/MultiDate.Edit"; 

        public MultiDateFieldDriver(IOrchardServices services, IDateLocalizationServices dateLocalizationServices)
        {
            Services = services;
            DateLocalizationServices = dateLocalizationServices;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public IDateLocalizationServices DateLocalizationServices { get; set; }
        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(ContentField field, ContentPart part) {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, MultiDateField field, string displayType, dynamic shapeHelper) {
            return ContentShape("Fields_MultiDate", 
                GetDifferentiator(field, part),
                () => {
                    ParseDates(field, field.Value);
                    return shapeHelper.Fields_MultiDate(Field: field);
                }
            );
        }

        protected override DriverResult Editor(ContentPart part, MultiDateField field, dynamic shapeHelper) {
            var settings = field.PartFieldDefinition.Settings.GetModel<MultiDateFieldSettings>();

            var viewModel = new MultiDateFieldViewModel {
                Name = field.DisplayName,
                Hint = settings.Hint,
                IsRequired = settings.Required,
                Dates = field.Value
            };

            return ContentShape("Fields_MultiDate_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: viewModel, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, MultiDateField field, IUpdateModel updater, dynamic shapeHelper)
        {
            var viewModel = new MultiDateFieldViewModel();

            if (updater.TryUpdateModel(viewModel, GetPrefix(field, part), null, null)) {

                var settings = field.PartFieldDefinition.Settings.GetModel<MultiDateFieldSettings>();

                if (settings.Required && !viewModel.Dates.Any()) {
                    updater.AddModelError(GetPrefix(field, part), T("{0} is required.", field.DisplayName));
                }
                else {
                    try {
                        ParseDates(field, viewModel.Dates);
                        field.Value = viewModel.Dates;
                    }
                    catch {
                        updater.AddModelError(GetPrefix(field, part), T("{0} could not be parsed as a valid date.", field.DisplayName));
                    }
                }
            }

            return Editor(part, field, shapeHelper);
        }

        private void ParseDates(MultiDateField field, string dates) {
            var options = new DateLocalizationOptions {EnableTimeZoneConversion = false};
            var dateStrings = dates.Split(',');
            field.Dates = new DateTime[dateStrings.Length];
            for (var i = 0; i < dateStrings.Length; i++) {
                field.Dates[i] = DateLocalizationServices.ConvertFromLocalizedString(dateStrings[i], "00:00:00 AM", options).GetValueOrDefault();
            }
        }

        protected override void Importing(ContentPart part, MultiDateField field, ImportContentContext context)
        {
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Value", v => field.Value = v);
        }

        protected override void Exporting(ContentPart part, MultiDateField field, ExportContentContext context)
        {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Value", field.Value);
        }

        protected override void Describe(DescribeMembersContext context)
        {
            context
                .Member(null, typeof(string), T("Value"), T("The date values of the field."))
                .Enumerate<MultiDateField>(() => field => field.Dates);
        }
    }
}
