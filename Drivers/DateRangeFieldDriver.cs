﻿using System.Linq;
using MultiDates.Fields;
using MultiDates.Services;
using MultiDates.Settings;
using MultiDates.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.Localization.Services;

namespace MultiDates.Drivers
{
    public class DateRangeFieldDriver: ContentFieldDriver<DateRangeField> {
        private readonly IMultiDateService _multiDateService;
        private const string TemplateName = "Fields/DateRange.Edit";

        public DateRangeFieldDriver(IOrchardServices services, IDateLocalizationServices dateLocalizationServices, IMultiDateService multiDateService)
        {
            Services = services;
            DateLocalizationServices = dateLocalizationServices;
            _multiDateService = multiDateService;
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

        protected override DriverResult Display(ContentPart part, DateRangeField field, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Fields_DateRange", 
                GetDifferentiator(field, part),
                () => shapeHelper.Fields_DateRange(Field: field));
        }

        protected override DriverResult Editor(ContentPart part, DateRangeField field, dynamic shapeHelper)
        {
            var settings = field.PartFieldDefinition.Settings.GetModel<DateRangeFieldSettings>();

            var viewModel = new DateRangeFieldViewModel {
                Name = field.DisplayName,
                Hint = settings.Hint,
                IsRequired = settings.Required,
                DatesString = field.Value
            };

            return ContentShape("Fields_DateRange_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: viewModel, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, DateRangeField field, IUpdateModel updater, dynamic shapeHelper)
        {
            var viewModel = new MultiDateFieldViewModel();

            if (updater.TryUpdateModel(viewModel, GetPrefix(field, part), null, null)) {

                var settings = field.PartFieldDefinition.Settings.GetModel<MultiDateFieldSettings>();

                if (settings.Required && string.IsNullOrEmpty(viewModel.DatesString)) {
                    updater.AddModelError(GetPrefix(field, part), T("{0} is required.", field.DisplayName));
                }
                else {
                    try {
                        _multiDateService.ParseDateRange(viewModel.DatesString);
                        field.Value = viewModel.DatesString;
                    }
                    catch {
                        updater.AddModelError(GetPrefix(field, part), T("{0} could not be parsed as a valid date.", field.DisplayName));
                    }
                }
            }

            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, DateRangeField field, ImportContentContext context)
        {
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Value", v => field.Value = v);
        }

        protected override void Exporting(ContentPart part, DateRangeField field, ExportContentContext context)
        {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Value", field.Value);
        }

        protected override void Describe(DescribeMembersContext context)
        {
            context
                .Member(null, typeof(string), T("Value"), T("The date values of the field."))
                .Enumerate<DateRangeField>(() => field => field.Value);
        }
    }
}
