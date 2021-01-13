# Environment Variables and User-definitions

It's common when automating code-generation to want to pass in information that is not part of the model.  Textrude *automatically* imports the environment variables from the current process and makes these available in the *env* namespace.  

You can also supply *definitions* which appear in the *def* namespace.  Definitions can be added from the "other->definitions" tab in TextrudeInteractive.

See the *enviromentAndDefinitions.texproj* project in the examples folder