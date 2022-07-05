namespace GitlabSlackNotifier.Core.Domain.Application.Commands;

public class CodeOwnerModel
{
    public string GitlabUsername { get; set; }
    public string SlackId { get; set; }
}

public enum CodeOwnerModelConversionResponse
{
    None,
    Error,
    Success
}

public static class CodeOwnerModelExtensions
{
    public static CodeOwnerModelConversionResponse GetCodeOwnersFromString(this string? input,
        out List<CodeOwnerModel> response)
    {
        response = new();

        if (string.IsNullOrEmpty(input))
            return CodeOwnerModelConversionResponse.None;

        var owners = input.Split(",");
        foreach (var owner in owners)
        {
            var split = owner.Split(":");
            if (split.Length != 2)
                return CodeOwnerModelConversionResponse.Error;
            
            response.Add(new ()
            {
                GitlabUsername = split[0],
                SlackId = split[1]
            });
        }

        return CodeOwnerModelConversionResponse.Success;
    }
}