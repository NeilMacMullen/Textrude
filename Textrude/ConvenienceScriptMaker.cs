namespace Textrude;

public static class ConvenienceScriptMaker
{
    private static readonly string Start = "{{";
    private static readonly string End = "}}";

    private static readonly string ImportAll = @"import string 
import math 
import date 
import regex 
import timespan
import html 
import object 
import array
import textrude
import timecomparison
";

    public static string ModelPipedToArrayProcessing(string expression, string arrayFunc) =>
        BareExpression(
            $@"# query code goes inside this function
func query(i)
{expression}
end
m
|> array.{arrayFunc} @query 
|> textrude.serialize _runtime.models[0].format");


    public static string ModelPipedToExpression(string expression) =>
        BareExpression($@"m
|> {expression}");

    public static string BareExpression(string expression) =>
        $@"{Start}
{ImportAll}
{expression} 
{End}";
}
