# Snippets

The Monaco editor used by TextrudeInteractive supports *snippets* in the Scriban editing window.

## Adding Snippets.

TextrudeInteractive loads snippets from a file called "snippets.json" which it expects to locate in the  %AppDataLocal%/TextrudeInteractive folder.  For example:
`C:\Users\<your user name>\AppData\Local\TextrudeInteractive\snippets.json`

An example snippets file is shown below...

```json
{
   "Scriban": [
    {
      "label": "mysnippet",
      "documentation": "this an example snippet",
      "insertText" :"the text to insert"
    },
    {
      "label": "secondsnipet",
      "documentation": "another snippet",
      "insertText" :"more text"
    }
  ]
}
```
