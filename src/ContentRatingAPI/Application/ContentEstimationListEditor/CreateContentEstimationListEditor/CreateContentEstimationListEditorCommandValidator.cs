// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentValidation;

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
