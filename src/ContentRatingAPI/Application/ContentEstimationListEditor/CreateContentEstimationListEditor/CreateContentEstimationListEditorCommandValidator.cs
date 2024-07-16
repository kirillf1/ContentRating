using FluentValidation;
using Microsoft.Extensions.Localization;
using System;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.CreateContentEstimationListEditor
{
    public class CreateContentEstimationListEditorCommandValidator : AbstractValidator<CreateContentEstimationListEditorCommand>
    {
        public CreateContentEstimationListEditorCommandValidator()
        {
            RuleFor(editor => editor.Id).NotEqual(Guid.Empty);
            RuleFor(editor => editor.CreatorName).NotEmpty();
            RuleFor(editor => editor.CreatorId).NotEqual(Guid.Empty);
            RuleFor(editor => editor.RoomName).NotEmpty().Length(3, 50);
        }
    }
}
