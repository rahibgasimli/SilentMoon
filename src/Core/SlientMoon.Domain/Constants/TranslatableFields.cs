using SlientMoon.Domain.Enums;

namespace SlientMoon.Domain.Constants
{
    public static class TranslatableFields
    {
        public static readonly TranslatableField[] Category =
       {
            TranslatableField.Name
        };

        public static readonly TranslatableField[] Track =
        {
            TranslatableField.Title
        };

        public static readonly TranslatableField[] Course =
        {
            TranslatableField.Title,
            TranslatableField.Subtitle,
            TranslatableField.Description
        };
    }
}
