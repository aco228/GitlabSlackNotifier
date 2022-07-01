using System.Collections.Concurrent;
using GitlabSlackNotifier.Core.Domain.Gitlab.Projects;
using GitlabSlackNotifier.Core.Services.Gitlab;
using Microsoft.Extensions.Logging;

namespace GitlabSlackNotifier.Core.Infrastructures.Gitlab;

public class GitlabProjectsCache : IGitlabProjectsCache
{
    private readonly IGitlabProjectsClient _projectsClient;
    private readonly ILogger<IGitlabProjectsCache> _logger;
    private ConcurrentDictionary<string, GitlabProjectResponse> _projects = new();

    public GitlabProjectsCache (
        ILogger<IGitlabProjectsCache> logger,
        IGitlabProjectsClient projectsClient)
    {
        _projectsClient = projectsClient;
        _logger = logger;
    }


    public async Task<GitlabProjectResponse?> GetProjectByNamespace(string namespacePath)
    {
        if (_projects.TryGetValue(namespacePath, out var project))
            return project;

        var projects = await _projectsClient.GetProjects(new ()
        {
            Search = namespacePath
        });

        var firstProject = projects.FirstOrDefault(x => x.PathWithNamespace.Equals(namespacePath));
        if (firstProject == null || !firstProject.PathWithNamespace.Equals(namespacePath))
            return null;

        _logger.LogInformation($"Loading project from API {namespacePath}, projectId={firstProject.Id}");
        _projects.TryAdd(firstProject.PathWithNamespace, firstProject);
        return firstProject;
    }
}
