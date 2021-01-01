namespace Engine.Model
{
    /// <summary>
    ///     Represents an object-model used by the application
    /// </summary>
    /// <remarks>
    ///     There's nothing magic about this - it's just a wrapper around the untyped object
    ///     tree so that we can avoid too many internal APIs with 'object' as a parameter
    /// </remarks>
    public class Model
    {
        public readonly object Untyped;

        public Model(object untyped) =>
            //Note that untyped must be EITHER an array of items OR a Dictionary 
            //TODO - put in some error checking here
            Untyped = untyped;
    }
}