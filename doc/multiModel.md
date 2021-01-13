# Using multiple models and/or multiple output files

It's sometimes useful to be able to combine information from multiple models and to render output to more than one file.  For example, you might want to use a single template to render both a .h header file and a .cpp source file.

Textrude allows multiple models to be referenced as *model0*, *model1*, *model2* etc.   There is no limit on the number of models that can be supplied although the UI of TextrudeInteractive current only supports 3.

By default, all output is rendered to the main output window but the SCRIBAN *capture* keyword can be used to redirect output to *output1*, *output2* etc.  Currently only 10 output streams are supported.

The *multiInOut.texproj* project in the examples folder demonstrates this feature.