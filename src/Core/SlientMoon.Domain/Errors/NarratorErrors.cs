namespace SlientMoon.Domain.Errors
{
    public static class NarratorErrors
    {
        public static Error NarratorNotFound => Error.NotFound(
            "Narrator.NotFound",
            "Narrator not found."
            );

        public static Error NarratorHasTracks => Error.Conflict(
            "Narrator.HasTracks",
            "Bu narrator-a bağlı track-lar var, əvvəlcə onları silin."
            );
    }
}
