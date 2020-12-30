namespace Engine.Model
{
    public class Model
    {
        public readonly object Untyped;

        public Model(object untyped) =>
            //Note that untyped must be EITHER an array of items OR a Dictionary 
            //TODO - put in some error checking here
            Untyped = untyped;
    }
}