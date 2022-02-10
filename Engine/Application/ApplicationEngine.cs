using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Engine.Extensions;
using Engine.Model;
using Engine.Model.Helpers;
using Engine.TemplateProcessing;
using Scriban.Runtime;

namespace Engine.Application;

public class RuntimeInfo
{
    public ModelInfo[] Models { get; set; } = Array.Empty<ModelInfo>();

    public void AddModel(string name, ModelFormat format)
    {
        Models = Models.Append(new ModelInfo { Name = name, Format = format.ToString() }).ToArray();
    }

    public class ModelInfo
    {
        public string Name { get; set; }
        public string Format { get; set; }
    }
}

/// <summary>
///     Abstract ApplicationEngine callable by UI and CLI
/// </summary>
/// <remarks>
///     Both the UI and CLI applications perform the same task.
///     The only significant difference is that the CLI marshals
///     arguments from the command line and then writes the
///     output streams to files.
///     Some logic is delegated to the TemplateManager; the intention
///     is that all application policy decisions (such as the naming
///     conventions for the various names-spaces, output streams etc
///     are encapsulated in this class.
///     IMPORTANT - it is expected that each instance of an ApplicationEngine
///     will only be used to provide  a single render pass for a singe template.
/// </remarks>
public class ApplicationEngine
{
    private readonly CancellationToken _cancel;
    private readonly RunTimeEnvironment _environment;

    private readonly RuntimeInfo _info = new();

    /// <summary>
    ///     provides generic scriban Template operations
    /// </summary>
    private readonly TemplateManager _templateManager;

    /// <summary>
    ///     Create a new application engine
    /// </summary>
    public ApplicationEngine(RunTimeEnvironment environment, CancellationToken cancel = new())
    {
        _environment = environment;
        _templateManager = new TemplateManager(environment.FileSystem);
        //we always add the location of the application executable as an include path for
        //scripts. This allows us to easily ship a library of standard scripts
        _templateManager.AddIncludePath(environment.ApplicationFolder());
        _cancel = cancel;
    }

    /// <summary>
    ///     The accumulated errors from the application
    /// </summary>
    public ImmutableArray<string> Errors { get; private set; } = ImmutableArray<string>.Empty;

    /// <summary>
    ///     The final top-level text rendered by the engine
    /// </summary>
    /// <remarks>
    ///     Templates can generate multiple output streams - the GetOutput method is more flexible and should
    ///     be preferred.
    /// </remarks>
    public string Output { get; private set; } = string.Empty;

    /// <summary>
    ///     True if the engine has generated errors at any point
    /// </summary>
    public bool HasErrors => Errors.Any();

    public string ErrorOrOutput => HasErrors ? string.Join(Environment.NewLine, Errors) : Output;
    public string RenderToErrorOrOutput() => Render().ErrorOrOutput;
    public ImmutableArray<ModelPath> ModelPaths() => _templateManager.ModelPaths();


    public ApplicationEngine WithModel(string name, object obj)
    {
        var model = ModelDeserializerFactory.Serialise(obj, ModelFormat.Json);
        return WithModel(name, model, ModelFormat.Json);
    }

    public ApplicationEngine WithModel(string name, string modelText, ModelFormat format)
    {
        try
        {
            _cancel.ThrowIfCancellationRequested();
            //parse the text and create a model
            var serializer = ModelDeserializerFactory.Fetch(format);
            var model = serializer.Deserialize(modelText);
            _templateManager.AddVariable(name, model.Untyped);
            _info.AddModel(name, model.SourceFormat);
        }
        catch (Exception e)
        {
            Errors = Errors.Add($"Model error: {e.Message}");
        }

        return this;
    }

    /// <summary>
    ///     Makes the current environment variable set available to the template engine
    /// </summary>
    public ApplicationEngine WithEnvironmentVariables()
    {
        var environmentVariables =
            Environment.GetEnvironmentVariables()
                .Cast<DictionaryEntry>()
                .ToDictionary(kv => kv.Key.ToString(), kv => kv.Value);
        //Add run-time information
        environmentVariables[ScribanNamespaces.TextrudeExe] = _environment.ApplicationPath();
        _templateManager.AddVariable(ScribanNamespaces.EnvironmentNamespace, environmentVariables);
        return this;
    }

    /// <summary>
    ///     Adds a set of definitions to the engine
    /// </summary>
    /// <remarks>
    ///     Each definition is of the form "def=value".
    ///     This method should only be called once.
    /// </remarks>
    public ApplicationEngine WithDefinitions(IEnumerable<string> definitionAssignments)
    {
        try
        {
            var definitions = DefinitionParser.CreateDefinitions(definitionAssignments)
                .ToDictionary(kv => kv.Key, kv => (object)kv.Value);
            _templateManager.AddVariable(ScribanNamespaces.DefinitionsNamespace, definitions);
        }
        catch (ArgumentException e)
        {
            Errors = Errors.Add($"Definition error: {e.Message}");
        }

        return this;
    }

    /// <summary>
    ///     Adds a template to the engine
    /// </summary>
    /// <remarks>
    ///     This is really the only "compulsory" call since we can actually render output without
    ///     any models, definitions, or environment.
    /// </remarks>
    public ApplicationEngine WithTemplate(string templateText)
    {
        templateText = TemplateProcessor.ApplyAllTransforms(templateText);
        _templateManager.SetTemplate(templateText);
        Errors = Errors.AddRange(_templateManager.ErrorList);
        return this;
    }

    /// <summary>
    ///     Run the render pass of the engine.
    /// </summary>
    public ApplicationEngine Render()
    {
        try
        {
            _templateManager.AddVariable("_runtime", _info);
            Output = _templateManager.Render(_cancel);
        }
        catch (Exception e)
        {
            Errors = Errors.Add($"Render error: {e.Message}");
        }

        return this;
    }

    /// <summary>
    ///     Adds some helper methods
    /// </summary>
    public ApplicationEngine WithHelpers()
    {
        void Add(ExtensionCache.KnownAssemblies name, ScriptObject scriptObject)
        {
            _templateManager.AddVariable(name.ToString().ToLowerInvariant(),
                scriptObject);
        }

        Add(ExtensionCache.KnownAssemblies.Debug,
            ExtensionCache.GetDebugMethods());

        Add(ExtensionCache.KnownAssemblies.Misc,
            ExtensionCache.GetMiscMethods());

        Add(ExtensionCache.KnownAssemblies.Humanizr,
            ExtensionCache.GetHumanizrMethods());

        Add(ExtensionCache.KnownAssemblies.Textrude,
            ExtensionCache.GetTextrudeMethods());

        Add(ExtensionCache.KnownAssemblies.Group,
            ExtensionCache.GetGroupingMethods());

        Add(ExtensionCache.KnownAssemblies.TimeComparison,
            ExtensionCache.GetTimeComparisonMethods());
        return this;
    }


    /// <summary>
    ///     Retrieve the output of the engine render pass
    /// </summary>
    /// <remarks>
    ///     A template can render to multiple output streams.  By convention they are named
    ///     output,output1,output2 etc but in future we may allow more flexible naming
    /// </remarks>
    public ImmutableArray<string> GetOutput(int count)
    {
        return
            Enumerable.Range(0, count).Select(i =>
                i == 0
                    ? Output
                    : _templateManager.GetStringOrEmpty($"{ScribanNamespaces.OutputPrefix}{i}")
            ).ToImmutableArray();
    }

    public string GetOutputFromVariable(string name)
    {
        if (name == "output")
            return Output;
        return _templateManager.GetStringOrEmpty(name);
    }

    public Dictionary<string, string> GetDynamicOutput()
        => _templateManager.TryGetVariableObject<Dictionary<string, string>>(TextrudeMethods.DynamicOutputName,
            out var d)
            ? d
            : new Dictionary<string, string>();

    /// <summary>
    ///     Adds a set of include paths to the engine
    /// </summary>
    /// <remarks>
    ///     The template loader in the templateManager will search the paths in order to try and find
    ///     files that are included using the scriban include directive
    /// </remarks>
    public ApplicationEngine WithIncludePaths(IEnumerable<string> paths)
    {
        foreach (var inc in paths)
            _templateManager.AddIncludePath(inc);
        return this;
    }

    public bool TryGetVariableAsJsonString(string name, out string res)
    {
        res = default;
        if (!_templateManager.TryGetVariableObject<object>(name, out var v))
            return false;
        res = ModelDeserializerFactory.Serialise(JsonGraph.ToJsonSerialisableTree(v), ModelFormat.Json);
        return true;
    }
}
