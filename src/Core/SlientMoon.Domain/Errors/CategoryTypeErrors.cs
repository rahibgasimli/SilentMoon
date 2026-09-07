namespace SlientMoon.Domain.Errors
{
    public static class CategoryTypeErrors
    {
        public static Error CategoryTypeNotFound => Error.NotFound(
            "CategoryType.NotFound",
            "Category type not found."
            );
    }
}
