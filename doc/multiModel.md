# Using multiple models and/or multiple output files

It's sometimes useful to be able to combine information from multiple models and to render output to more than one file.  For example, you might want to use a single template to render both a .h header file and a .cpp source file.

## Models

By default, models are named *model,model1,model2* etc.  However, these names can be changed in TextrudeInteractive using the *Inputs/Change name of model* menu.

Textrude (CLI) allows model names to be specified using '=' syntax.  For example:

```
textrude.exe render --models "model_a=d:/data/errs.csv" "model_b=c:/work/h.yaml"
```

## Output

By default, outputs are named *output,output1,output2* etc.  However, these names can be changed in TextrudeInteractive using the *Outputs/Change name of output* menu.

By default, all output is rendered to the main output window but the SCRIBAN *capture* keyword can be used to redirect output to other output streams. 

The *multiInOut.texproj* project in the examples folder demonstrates this feature.

Textrude (CLI) allows output names to be specified using '=' syntax.  For example:

```
textrude.exe render --output  "mainoutput.cpp"  "output_a=d:/header.h" "output_b=main.cpp"
```

